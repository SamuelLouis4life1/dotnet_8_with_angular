using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastruture.Data;
using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        public IGenericRepository<Product> _productRepository { get; }
        public readonly IGenericRepository<ProductBrand> _productBrandRepository;
        public IGenericRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepository,
        IGenericRepository<ProductBrand> productBrandRepository,
        IGenericRepository<ProductType> productTypeRepository,
        IMapper mapper)
        {
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductsToReturnDto>>> GetProducts()
        {
            var specification = new ProductsWithTypesAndBrandsSpecification();

            var products = await _productRepository.ListAsync(specification);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductsToReturnDto>>(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsToReturnDto>> GetProduct(Guid id)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productRepository.GetEntityWithSpecification(specification);

            return _mapper.Map<Product, ProductsToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepository.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepository.ListAllAsync());
        }
    }
}