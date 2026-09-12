using ASP_P42.Data;
using ASP_P42.Models.Admin;
using ASP_P42.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class AdminController(IStorageService storageService, DataContext dataContext) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataContext _dataContext = dataContext;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product()
        {
            AdminGroupViewModel viewModel = new()
            {
                Groups = _dataContext.ProductGroups
                    .OrderBy(g => g.OrderInPrice).ToList(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddProduct(AdminAddProductFormModel formModel)
        {
            try
            {
                Data.Entities.ProductGroup group = _dataContext
                    .ProductGroups
                    .FirstOrDefault(g => g.Id == formModel.GroupId)
                ?? throw new Exception($"Group not found with id='{formModel.GroupId}'");

                if(formModel.ProductId != null)
                {
                    Data.Entities.Product? product = _dataContext
                        .Products
                        .FirstOrDefault(p => p.Id == formModel.ProductId)
                    ?? throw new Exception($"Product not found with id='{formModel.ProductId}'");
                    
                }
                else
                {
                    string? imageUrl = null;
                    if(formModel.Image != null)
                    {
                        imageUrl = _storageService.Save(formModel.Image);
                    }

                    Guid productId = Guid.NewGuid();
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
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        ImageUrl = imageUrl,
                        Price = (decimal)formModel.Price,
                        Stock = formModel.Stock,
                        OrderInPrice = 1,
                        Slug = formModel.Slug,
                        IsHidden = formModel.IsHidden,
                    });

                    _dataContext.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Group()
        {
            AdminGroupViewModel viewModel = new()
            {
                Groups = _dataContext.ProductGroups.ToList(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddGroup(AdminAddGroupFormModel formModel)
        {
            try
            {
                /* Д.З. Реалізувати валідацію моделі форми 
                 * додавання нової товарної групи
                 * - назва (довжина, відсутність спецсимволів)
                 * - опис (довжина)
                 * - Slug (унікальність, url-коректність)
                 */
                _dataContext.ProductGroups.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ParentId = formModel.ParentId,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                    ImageUrl = "/storage/image/" + _storageService.Save(formModel.Image)
                });
                _dataContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}