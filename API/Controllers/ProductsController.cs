using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;
using API.Helpers;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;
    private readonly IMapper _mapper;

    public ProductsController(
        IGenericRepository<Product> productRepo,
        IGenericRepository<ProductBrand> productBrandRepo,
        IGenericRepository<ProductType> productTypeRepo,
        IMapper mapper
        )
    {
        _productRepo = productRepo;
        _productBrandRepo = productBrandRepo;
        _productTypeRepo = productTypeRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
    {
        var spec = new ProductsWithBrandsAndTypesSpecification(productParams);
        
        var countSpec = new ProductWithFiltersForCountSpecification(productParams);

        var totalItems = await _productRepo.CountAsync(countSpec);

        var products = await _productRepo.ListAsync(spec);

        var data = (_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));

        return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
        var spec = new ProductsWithBrandsAndTypesSpecification(id);

        var product = await _productRepo.GetEntityWithSpecification(spec);

        if (product == null) return NotFound(new ApiResponse(404));

        return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
    }

    [HttpGet("brands")]
    public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
    {
        var productsBrand = await _productBrandRepo.ListAllAsync();
        return Ok(productsBrand);
    } 
    
    [HttpGet("types")]
    public async Task<ActionResult<List<ProductType>>> GetProductTypes()
    {
        var productsType = await _productTypeRepo.ListAllAsync();
        return Ok(productsType);
    }
}
