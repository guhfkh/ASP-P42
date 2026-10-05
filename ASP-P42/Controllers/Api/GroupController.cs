using ASP_P42.Data;
using ASP_P42.Data.Entities;
using ASP_P42.Models.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ASP_P42.Controllers.Api
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        private String? FullImageUrl(String? url)
        {
            if (url == null) return null;
            if (url.StartsWith('/'))
            {
                return $"{Request.Scheme}://{Request.Host}{url}";
            }
            if (!url.StartsWith("http"))
            {
                return $"{Request.Scheme}://{Request.Host}/Storage/Item/{url}";
            }
            return url;
        }

        [HttpGet]
        public RestResponse GetAllGroups(int page = 1, int pageSize = 10)
        {
            var query = _dataContext
                .ProductGroups
                .Include(g => g.Children)
                .Where(g => g.IsHidden == 0 && g.ParentId == null)
                .OrderBy(g => g.OrderInPrice);

            int cnt = query.Count();

            RestMetaPagination pagination = new()
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = cnt,
                TotalPages = (int)Math.Ceiling((float)cnt / pageSize),
            };

            ProductGroup[] groups = query
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToArray();

            ProductGroup[] result = [
                .. groups.Select(group => group with
        {
            ImageUrl = FullImageUrl(group.ImageUrl)!,
            Children = [
                .. group.Children.Select(child => child with
                {
                    ImageUrl = FullImageUrl(child.ImageUrl)!
                })
            ]
        })
            ];

            return new()
            {
                Meta = new()
                {
                    ApiName = "Product Groups",
                    DataType = "json/array",
                    CacheTime = 86_400_000,
                    Manipulations = ["GET"],
                    Links =
            {
                { "self", "/api/group" },
                { "sub", "/api/group/{slug}" },
            },
                    Pagination = pagination,
                },
                Data = result,
            };
        }
        /* Д.З. Змінити реалізацію GetAllGroups з використанням операцій клонування (with {}) 
         * та колекційних ініціалізаторів для об'єктів з повними URL зображень.
         */

        [HttpGet("{id}")]
        public RestResponse GetOneGroup(String id, int page = 1, int pageSize = 10)
        {
            ProductGroup? group = _dataContext
                .ProductGroups
                .Include(g => g.Products.OrderBy(p => p.OrderInPrice))
                    .ThenInclude(p => p.Versions)
                .Where(g => g.IsHidden == 0 && g.Slug == id)
                .FirstOrDefault();

            if (group == null)
            {
                return new()
                {
                    Status = RestStatus.NotFound,
                };
            }

            int cnt = group.Products.Count;

            RestMetaPagination pagination = new()
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = cnt,
                TotalPages = (int)Math.Ceiling((float)cnt / pageSize),
            };

            // group.Products = group.Products.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            var grp = group with
            {
                ImageUrl = FullImageUrl(group.ImageUrl)!,
                Products = [..
                    group
                    .Products
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .Select(p => p with {
                        ImageUrl = FullImageUrl(p.ImageUrl),
                        Versions = [.. p.Versions.Select(v => v with { ImageUrl = FullImageUrl(v.ImageUrl) })]
                    })
                ]
            };


            return new()
            {
                Meta = new()
                {
                    ApiName = "Group Products",
                    DataType = "json/object",
                    CacheTime = 86_400_000,
                    Manipulations = ["GET"],
                    Links = {
                        { "parent", "/api/group" },
                        { "self", "/api/group/{slug}" },
                    },
                    Pagination = pagination,
                },
                Data = grp,
            };
        }

        [HttpPost]   // це запускатиметься запитом POST /api/group
        public bool CreateNewGroup(Data.Entities.ProductGroup group)
        {
            return true;
        }
    }
}
/*
 API контролери, відмінності від MVC

API - Application Program Interface
Проєкт (програмний комплекс) ми умовно поділяємо на 
Program - "центральну" частину, що відповідає за збереження даних
 (на пряму з користувачем-людиною не взаємодіє)
Applications (застосунки) - відокремлені програми, що взаємодіють з
 користувачем та центральною Програмою

                        Program (ASP)
                   API /       |API    \ API
                      /        |        \ 
      React (web-frontend)     Mobile     Desktop

Оскільки Програма на пряму не контактує з людиною, АРІ передбачає 
передачу "сирих" даних, тоді як MVC має представлення (View), призначені для людини.
- MVC повертає IActionResult, серед яких є JSON, але основою є представлення
- API одразу повертає JSON і не може створювати представлення

Маршрутизація:
- MVC поділяє запит за адресою -- pattern: "{controller=Home}/{action=Index}/{id?}"
   незалежно від методу запиту (GET, POST, ...) -- GET /path та POST /path 
   запускають одну і ту саму активність
- АРІ має постійну адресу, а відмінність в активності задається методами запиту
   GET /path та POST /path  запускають різні дії

Д.З. Впровадити REST принципи у власні курсові проєкти.
Прикласти посилання на репозиторії.
 */