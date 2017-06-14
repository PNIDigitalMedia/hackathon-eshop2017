using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using PNI.EShop.API.Models;
using PNI.EShop.Core.Product;
using PNI.EShop.Core._Common;
using ProductRepository.Interfaces;

namespace PNI.EShop.API.Controllers
{
    [Produces("application/json")]
    [Route("Products")]
    public class ProductsController : Controller
    {
        private static readonly ActorId ActorId = new ActorId("EShopProductRepsitory");
        private static readonly Uri RepsitoryActorUrl = new Uri("fabric:/PNI.Services/ProductRepositoryActorService");
        
        [HttpGet]
        [Route("")]
        public Task<ProductDto[]> Products()
        {
            //var productRepository = ActorProxy.Create<IProductRepository>(ActorId, RepsitoryActorUrl);
            
            //var products = await productRepository.RetrieveAllProductsAsync();

            //return products.Select(CreateProductDtoFromProduct).ToArray();

            return Task.FromResult(CreateProducts().ToArray());
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ProductDto> Product(Guid id)
        {
            return Task.FromResult(CreateProducts().First(p => p.Id == id));
        }

        private static ProductDto CreateProductDtoFromProduct(Product proeduct)
        {
            return new ProductDto
            {
                Id = proeduct.Id.Id,
                Name = proeduct.Name.ToString(),
                Color = proeduct.Model.Color.Color,
                Type = proeduct.Model.Type.ModelTypeDefinition,
                CreatedAt = proeduct.CreatedAt.Date,
                UpdatedAt = proeduct.ModifiedAt.Date
            };
        }

        private static IEnumerable<ProductDto> CreateProducts()
        {
            return new[]
            {
                new ProductDto { Id = Guid.Parse("89707fc0-1493-4899-a062-e37127ec497b"), Color = ColorDefinition.Black, Type = ModelTypeDefinition.Box, Name = "Black Box"},
                new ProductDto { Id = Guid.Parse("5a19ff97-8095-4d43-8d30-26751851a6fe"), Color = ColorDefinition.White, Type = ModelTypeDefinition.Box, Name = "White Box"},
                new ProductDto { Id = Guid.Parse("4b4480b3-e368-4f01-931c-67c52eee914a"), Color = ColorDefinition.Red, Type = ModelTypeDefinition.Cone, Name = "Red Cone"},
                new ProductDto { Id = Guid.Parse("718e5ba7-b31c-4655-849b-880948d83cba"), Color = ColorDefinition.Blue, Type = ModelTypeDefinition.Cylinder, Name = "Red Cylinder"},
                new ProductDto { Id = Guid.Parse("5803710e-92a9-4b08-8788-fccf8a9c8e4a"), Color = ColorDefinition.Green, Type = ModelTypeDefinition.Sphere, Name = "Green Sphere"},
            };

        }
    }
}