using ASP_P42.Data.Entities;
using ASP_P42.Models.Admin;

namespace ASP_P42.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public Guid GetDbIdentity() => Guid.NewGuid();

        public async Task<Guid> AddNewProductGroup(ProductGroup productGroup)
        {
            Guid id = GetDbIdentity();

            productGroup.Id = id;

            _dataContext.ProductGroups.Add(productGroup);

            await _dataContext.SaveChangesAsync();

            return id;
        }

        public List<ProductGroup> GetAllProductGroups(
            bool isIncludeHidden = false)
        {
            IQueryable<ProductGroup> query = _dataContext.ProductGroups;

            if (!isIncludeHidden)
            {
                query = query.Where(g => g.IsHidden == 0);
            }

            return [.. query.OrderBy(g => g.OrderInPrice)];
        }

        public ProductGroup? GetProductGroupById(Guid id)
        {
            return _dataContext.ProductGroups
                .FirstOrDefault(g => g.Id == id);
        }

        public bool UpdateProductGroup(ProductGroup productGroup)
        {
            ProductGroup? existingGroup =
                _dataContext.ProductGroups
                    .FirstOrDefault(g => g.Id == productGroup.Id);

            if (existingGroup == null)
            {
                return false;
            }

            existingGroup.ParentId = productGroup.ParentId;
            existingGroup.Name = productGroup.Name;
            existingGroup.Description = productGroup.Description;
            existingGroup.Slug = productGroup.Slug;
            existingGroup.ImageUrl = productGroup.ImageUrl;
            existingGroup.IsHidden = productGroup.IsHidden;
            existingGroup.OrderInPrice = productGroup.OrderInPrice;

            _dataContext.SaveChanges();

            return true;
        }

        public bool DeleteProductGroup(Guid id)
        {
            ProductGroup? productGroup =
                _dataContext.ProductGroups
                    .FirstOrDefault(g => g.Id == id);

            if (productGroup == null)
            {
                return false;
            }

            _dataContext.ProductGroups.Remove(productGroup);

            _dataContext.SaveChanges();

            return true;
        }

        public async Task<Guid> AddNewProduct(
            AdminAddProductFormModel formModel,
            String? imageUrl)
        {
            Guid id = GetDbIdentity();

            if (formModel.ProductId != null)
            {
                _dataContext.ProductVersions.Add(new()
                {
                    Id = id,
                    ProductId = GetProductById(formModel.ProductId.Value)!.Id,
                    ImageUrl = imageUrl,
                    Price = (decimal)formModel.Price,
                    Stock = formModel.Stock,
                    OrderInPrice = formModel.Order,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                    Version = formModel.Name
                });
            }
            else
            {
                ProductGroup group =
                    GetProductGroupById(formModel.GroupId)
                    ?? throw new Exception(
                        $"Product group not found with id='{formModel.GroupId}'");

                Guid productId = GetDbIdentity();

                _dataContext.Products.Add(new()
                {
                    Id = productId,
                    GroupId = group.Id,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    ImageUrl = imageUrl,
                    IsHidden = formModel.IsHidden,
                    OrderInPrice = formModel.Order,
                    Slug = formModel.Slug,
                });

                _dataContext.ProductVersions.Add(new()
                {
                    Id = id,
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    Price = (decimal)formModel.Price,
                    Stock = formModel.Stock,
                    OrderInPrice = 1,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                });
            }

            await _dataContext.SaveChangesAsync();

            return id;
        }

        public async Task<bool> IsProductFormModelValidAsync(
            AdminAddProductFormModel formModel)
        {
            var group = GetProductGroupById(formModel.GroupId)
                ?? throw new Exception(
                    $"Product group not found with id='{formModel.GroupId}'");

            if (formModel.ProductId != null)
            {
                _ = GetProductById(formModel.ProductId.Value)
                    ?? throw new Exception(
                        $"Product not found with id='{formModel.ProductId}'");
            }

            if (formModel.Slug != null)
            {
                if (_dataContext.Products.Any(p => p.Slug == formModel.Slug))
                {
                    throw new Exception(
                        $"Slug '{formModel.Slug}' is already in use by other product");
                }
            }

            return true;
        }

        public Product? GetProductById(Guid id)
        {
            return _dataContext.Products
                .FirstOrDefault(p => p.Id == id);
        }
    }
}