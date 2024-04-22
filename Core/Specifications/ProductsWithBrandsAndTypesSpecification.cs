using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;
public class ProductsWithBrandsAndTypesSpecification: BaseSpecification<Product>
{
    public ProductsWithBrandsAndTypesSpecification()
    {
        AddInclude(x => x.ProductType);
        AddInclude(x => x.ProductBrand);
    }

    public ProductsWithBrandsAndTypesSpecification(int id) : base(x => x.Id == id)
    {
        AddInclude(x => x.ProductType);
        AddInclude(x => x.ProductBrand);
    }
}
