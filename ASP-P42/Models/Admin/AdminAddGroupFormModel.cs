using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Models.Admin
{
    public class AdminAddGroupFormModel
    {
        [FromForm(Name = "group-img")]
        public IFormFile Image { get; set; } = null!;
    }
}
