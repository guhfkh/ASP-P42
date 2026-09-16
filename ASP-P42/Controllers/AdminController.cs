using ASP_P42.Data;
using ASP_P42.Models.Admin;
using ASP_P42.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class AdminController(
        IStorageService storageService,
        DataContext dataContext,
        DataAccessor dataAccessor
    ) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataContext _dataContext = dataContext;
        private readonly DataAccessor _dataAccessor = dataAccessor;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product()
        {
            AdminGroupViewModel viewModel = new()
            {
                Groups = _dataAccessor.GetAllProductGroups(isIncludeHidden:true),
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddProduct(AdminAddProductFormModel formModel)
        {
            // Валидация модели
            if (string.IsNullOrWhiteSpace(formModel.Name) ||
                formModel.Name.Length < 2 ||
                formModel.Name.Length > 100 ||
                !System.Text.RegularExpressions.Regex.IsMatch(
                    formModel.Name, @"^[\p{L}\p{N} ]+$"))
            {
                return BadRequest(
                    "Название должно содержать от 2 до 100 символов и не содержать специальных символов");
            }

            if (formModel.Description != null &&
                formModel.Description.Length > 1000)
            {
                return BadRequest(
                    "Описание не должно превышать 1000 символов");
            }

            if (!string.IsNullOrWhiteSpace(formModel.Slug))
            {
                if (formModel.Slug.Length > 100 ||
                    !System.Text.RegularExpressions.Regex.IsMatch(
                        formModel.Slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$"))
                {
                    return BadRequest(
                        "Slug має некоректний формат");
                }

                bool slugExists = _dataAccessor.IsSlugExists(
                    formModel.Slug,
                    formModel.ProductId);

                if (slugExists)
                {
                    return BadRequest(
                        "Товар з таким Slug вже існує");
                }
            }

            if (formModel.Stock != -1 && formModel.Stock <= 0)
            {
                return BadRequest(
                    "Кількість повинна бути позитивним числом або -1");
            }

            if (formModel.Price <= 0.01m)
            {
                return BadRequest(
                    "Ціна повинна бути більшою за 0.01");
            }

            try
            {
                Data.Entities.ProductGroup group = _dataContext
                    .ProductGroups
                    .FirstOrDefault(g => g.Id == formModel.GroupId)
                ?? throw new Exception(
                    $"Group not found with id='{formModel.GroupId}'");

                string? imageUrl = null;

                if (formModel.Image != null)
                {
                    imageUrl = _storageService.Save(formModel.Image);
                }

                if (formModel.ProductId != null)
                {
                    Data.Entities.Product? product = _dataContext
                        .Products
                        .FirstOrDefault(p => p.Id == formModel.ProductId)
                    ?? throw new Exception(
                        $"Product not found with id='{formModel.ProductId}'");

                    Guid productId = product.Id;

                    _dataContext.ProductVersions.Add(new()
                    {
                        Id = _dataAccessor.GetDbIdentity(),
                        ProductId = productId,
                        ImageUrl = imageUrl,
                        Price = (decimal)formModel.Price,
                        Stock = formModel.Stock,
                        OrderInPrice = 1,
                        Slug = formModel.Slug,
                        IsHidden = formModel.IsHidden,
                        Version = formModel.Name
                    });
                }
                else
                {
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
                }

                _dataContext.SaveChanges();

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
                Groups = _dataAccessor.GetAllProductGroups(isIncludeHidden: true),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupAsync(
            AdminAddGroupFormModel formModel
        )
        {
            try
            {
                /* Д.З. Реалізувати валідацію моделі форми 
                 * додавання нової товарної групи
                 * - назва (довжина, відсутність спецсимволів)
                 * - опис (довжина)
                 * - Slug (унікальність, url-коректність)
                 */
               Guid newGroupId = await 
                    _dataAccessor.AddNewProductGroup(new()
                {
                    ParentId = formModel.ParentId,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                    ImageUrl = "/storage/image/" + _storageService.Save(formModel.Image)
                });
                
                return Ok(newGroupId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}