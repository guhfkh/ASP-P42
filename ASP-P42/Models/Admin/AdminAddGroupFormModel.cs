using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Models.Admin
{
    public class AdminAddGroupFormModel
    {
        [FromForm(Name = "group-parent")]
        public Guid? ParentId { get; set; } = null!;

        [FromForm(Name = "group-name")]
        public string Name { get; set; } = null!;

        [FromForm(Name = "group-description")]
        public string Description { get; set; } = null!;

        [FromForm(Name = "group-slug")]
        public string Slug { get; set; } = null!;

        [FromForm(Name = "group-img")]
        public IFormFile Image { get; set; } = null!;

        [FromForm(Name = "group-hidden")]
        public int IsHidden { get; set; } = 0;

        [FromForm(Name = "product-order")]
        public int Order { get; set; } = 0;
    }
}
