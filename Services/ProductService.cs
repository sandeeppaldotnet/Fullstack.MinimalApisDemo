using Fullstack.MinimalApisDemo.Models;

namespace Fullstack.MinimalApisDemo.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly List<Product> _products =
        [
            new Product
        {
            Id = 1,
            Name = "Mechanical Keyboard",
            Description = "RGB mechanical keyboard",
            Price = 3499m,
            Stock = 25,
            IsActive = true
        },
        new Product
        {
            Id = 2,
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse",
            Price = 1499m,
            Stock = 40,
            IsActive = true
        },
        new Product
        {
            Id = 3,
            Name = "USB-C Hub",
            Description = "Seven-port USB-C hub",
            Price = 2299m,
            Stock = 15,
            IsActive = true
        }
        ];

        private int _nextId = 4;

        public IReadOnlyList<Product> GetAll()
        {
            return _products
                .Select(Clone)
                .ToList();
        }

        public Product? GetById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            return product is null ? null : Clone(product);
        }

        public Product Create(CreateProductRequest request)
        {
            var product = new Product
            {
                Id = _nextId++,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Price = request.Price,
                Stock = request.Stock,
                IsActive = true
            };

            _products.Add(product);

            return Clone(product);
        }

        public Product? Update(
            int id,
            UpdateProductRequest request)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product is null)
            {
                return null;
            }

            product.Name = request.Name.Trim();
            product.Description = request.Description.Trim();
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.IsActive = request.IsActive;

            return Clone(product);
        }

        public Product? Patch(
            int id,
            PatchProductRequest request)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product is null)
            {
                return null;
            }

            if (request.Name is not null)
            {
                product.Name = request.Name.Trim();
            }

            if (request.Description is not null)
            {
                product.Description = request.Description.Trim();
            }

            if (request.Price.HasValue)
            {
                product.Price = request.Price.Value;
            }

            if (request.Stock.HasValue)
            {
                product.Stock = request.Stock.Value;
            }

            if (request.IsActive.HasValue)
            {
                product.IsActive = request.IsActive.Value;
            }

            return Clone(product);
        }

        public bool Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product is null)
            {
                return false;
            }

            _products.Remove(product);

            return true;
        }

        private static Product Clone(Product product)
        {
            return new Product
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive
            };
        }
    }


}
