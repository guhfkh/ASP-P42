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

        public IActionResult EditGroup(Guid id)
        {
            ProductGroup? group = _dataAccessor.GetProductGroupById(id);

            if (group == null)
            {
                return NotFound();
            }

            ViewBag.Groups = _dataAccessor.GetAllProductGroups(
                isIncludeHidden: true);

            return View(group);
        }

        [HttpPost]
        public IActionResult EditGroup(AdminEditGroupFormModel formModel)
        {
            try
            {
                ProductGroup? existingGroup =
                    _dataAccessor.GetProductGroupById(formModel.Id);

                if (existingGroup == null)
                {
                    return NotFound();
                }

                string imageUrl = existingGroup.ImageUrl;

                if (formModel.Image != null)
                {
                    imageUrl = "/storage/image/" +
                               _storageService.Save(formModel.Image);
                }

                bool result = _dataAccessor.UpdateProductGroup(new()
                {
                    Id = formModel.Id,
                    ParentId = formModel.ParentId,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    ImageUrl = imageUrl,
                    IsHidden = formModel.IsHidden,
                    OrderInPrice = formModel.OrderInPrice
                });

                if (!result)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Group));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpPost]
        public IActionResult DeleteGroup(Guid id)
        {
            try
            {
                bool result = _dataAccessor.DeleteProductGroup(id);

                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

/* Екзаменаційне завдання:
 * Реалізувати повний перелік операції з товарними групами:
 * - створення (вже є)
 * - перегляд
 * - оновлення
 * - видалення
 * 1) на сторінці /Admin/Group перед формою додавання групи 
 *    вивести перелік наявних груп
 *    * неактивні (видалені) групи виділяти (наприклад, сірим фоном)
 * 2) до кожної з груп у переліку додати кнопки
 *    "редагувати" та "видалити"
 * 3) натиснення кнопки редагування групи автоматично заповнює
 *    форму, призначену для нової групи, поточними даними.
 *    Назва кнопки "додати" змінюється на "зберегти"
 *    Реалізувати роботу форми.
 * 4) натиснення кнопки видалення запитує підтвердження дії
 *    та переводить групу до видалених, що відбивається на 
 *    її вигляді у переліку.
 */
