namespace ASP_P42.Models.Admin
{
    public class AdminEditGroupFormModel
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public IFormFile? Image { get; set; }

        public int IsHidden { get; set; }

        public int OrderInPrice { get; set; }
    }
}