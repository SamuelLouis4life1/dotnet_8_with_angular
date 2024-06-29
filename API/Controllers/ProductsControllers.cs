using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastruture.Data;
using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using API.Errors;
using API.Helpers;


namespace API.Controllers
{
    public class ProductsController : BaseApiController
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
        public async Task<ActionResult<Pagination<ProductsToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpecification = new ProductWithFiltersForCountSpecification(productParams);

            var totalItems = await _productRepository.CountAsync(countSpecification);

            var products = await _productRepository.ListAsync(specification);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductsToReturnDto>>(products);

            return Ok(new Pagination<ProductsToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductsToReturnDto>> GetProduct(Guid id)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productRepository.GetEntityWithSpecification(specification);

            if(product == null) return NotFound(new ApiResponse(404));

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