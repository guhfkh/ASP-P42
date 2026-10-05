using ASP_P42.Data;
using ASP_P42.Data.Entities;
using ASP_P42.Models.Admin;
using ASP_P42.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class AdminController(IStorageService storageService, DataAccessor dataAccessor) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataAccessor _dataAccessor = dataAccessor;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product()
        {
            AdminGroupViewModel viewModel = new()
            {
                Groups = _dataAccessor.GetAllProductGroups(isIncludeHidden: true),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AdminAddProductFormModel formModel)
        {
            // if(ModelState.IsValid) { }
            // else { ModelState.Er}
            try
            {
                await _dataAccessor.IsProductFormModelValidAsync(formModel);

                String? imageUrl = null;
                if (formModel.Image != null)
                {
                    imageUrl = _storageService.Save(formModel.Image);
                }

                await _dataAccessor.AddNewProduct(formModel, imageUrl);

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
                // Groups = _dataContext.ProductGroups.OrderBy(g => g.OrderInPrice).ToList(),
                Groups = _dataAccessor.GetAllProductGroups(isIncludeHidden: true),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup(AdminAddGroupFormModel formModel)
        {
            try
            {
                /* Д.З. Реалізувати валідацію моделі форми 
                 * додавання нової товарної групи
                 * - назва (довжина, відсутність спецсимволів)
                 * - опис (довжина)
                 * - Slug (унікальність, url-коректність)
                 */
                Guid newGroupId = await _dataAccessor.AddNewProductGroup(new()
                {
                    ParentId = formModel.ParentId,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                    ImageUrl = "/storage/image/" + _storageService.Save(formModel.Image),
                    OrderInPrice = formModel.Order,
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