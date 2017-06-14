﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using PNI.EShop.Core;
using PNI.EShop.Core.ProductCatalog.DataAccess;
using ProductActor.Interfaces;
using ProductRepository.Interfaces;

namespace ProductRepository
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class ProductRepository : Actor, IProductRepository
    {
        /// <summary>
        /// Initializes a new instance of ProductRepository
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ProductRepository(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var products = await StateManager.TryGetStateAsync<List<Guid>>("products");

            if (!products.HasValue)
            {
                CreateProducts();
            }
        }

        public async Task<ProductDto[]> RetrieveAllProductsAsync()
        {
            var productIds = await StateManager.TryGetStateAsync<List<Guid>>("products");
            var products = new List<ProductDto>();

            if (productIds.HasValue)
            {
                var productList = productIds.Value.Select(ProductById).ToArray();

                Task.WaitAll(productList);

                products.AddRange(productList.Select(task => task.Result));
            }

            return products.ToArray();
        }
        
        public Task<ProductDto> ProductById(Guid id)
        {
            var productActor = ProductActorFactory.GetProductActor(new ActorId(id));

            return productActor.RetrieveProduct();
        }

        private async Task CreateActor(ProductDto product)
        {
            var productActor = ProductActorFactory.GetProductActor(new ActorId(product.Id));

            await productActor.CreateProduct(product);

            await AddIdToState(product.Id);
        }

        private async Task AddIdToState(Guid id)
        {
            var state = await StateManager.TryGetStateAsync<List<Guid>>("products");
            if (state.HasValue)
            {
                state.Value.Add(id);

                await StateManager.AddOrUpdateStateAsync("products", state.Value, (key, products) => products);

                return;
            }

            var newState = new List<Guid> {id};

            await StateManager.AddStateAsync("products", newState);
        }

        private async void CreateProducts()
        {
            await CreateActor(new ProductDto
            {
                Id = Guid.Parse("89707fc0-1493-4899-a062-e37127ec497b"),
                Name = "Black Box",
                Description = "A simple black box",
                Model = new ProductModelDto
                {
                    Color = ColorDefinition.Black,
                    Type = ModelTypeDefinition.Box,
                    CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                ModifiedAt = DateTimeOffset.UtcNow
            });
            await CreateActor(new ProductDto
            {
                Id = Guid.Parse("5a19ff97-8095-4d43-8d30-26751851a6fe"),
                Name = "White Box",
                Description = "A simple white box",
                Model = new ProductModelDto
                {
                    Color = ColorDefinition.White,
                    Type = ModelTypeDefinition.Box,
                    CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                ModifiedAt = DateTimeOffset.UtcNow
            });
            await CreateActor(new ProductDto
            {
                Id = Guid.Parse("4b4480b3-e368-4f01-931c-67c52eee914a"),
                Name = "Red Cone",
                Description = "A simple red cone",
                Model = new ProductModelDto
                {
                    Color = ColorDefinition.Red,
                    Type = ModelTypeDefinition.Cone,
                    CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                ModifiedAt = DateTimeOffset.UtcNow
            });
            await CreateActor(new ProductDto
            {
                Id = Guid.Parse("718e5ba7-b31c-4655-849b-880948d83cba"),
                Name = "Green Sphere",
                Description = "A simple green sphere",
                Model = new ProductModelDto
                {
                    Color = ColorDefinition.Green,
                    Type = ModelTypeDefinition.Sphere,
                    CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                ModifiedAt = DateTimeOffset.UtcNow
            });
            await CreateActor(new ProductDto
            {
                Id = Guid.Parse("5803710e-92a9-4b08-8788-fccf8a9c8e4a"),
                Name = "Blue Cylinder",
                Description = "A simple blue cylinder",
                Model = new ProductModelDto
                {
                    Color = ColorDefinition.Blue,
                    Type = ModelTypeDefinition.Cylinder,
                    CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                CreatedAt = DateTimeOffset.Parse("Jan 24, 1975 15:00z"),
                ModifiedAt = DateTimeOffset.UtcNow
            });
        }
    }
}
