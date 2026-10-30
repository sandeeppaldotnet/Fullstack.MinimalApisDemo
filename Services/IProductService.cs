using Fullstack.MinimalApisDemo.Models;

namespace Fullstack.MinimalApisDemo.Services
{
    public interface IProductService
    {
        IReadOnlyList<Product> GetAll();

        Product? GetById(int id);

        Product Create(CreateProductRequest request);

        Product? Update(int id, UpdateProductRequest request);

        Product? Patch(int id, PatchProductRequest request);

        bool Delete(int id);
    }
}
