using Core.Entities;

namespace Core.Specification
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams productParams)
         : base(x => (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) && 
                (!productParams.BrandId.HasValue || x.ProductBrandId == Guid.Parse(productParams.BrandId.ToString())) &&
                (!productParams.TypeId.HasValue || x.ProductTypeId == Guid.Parse(productParams.TypeId.ToString())))
        {

        }
    }
}