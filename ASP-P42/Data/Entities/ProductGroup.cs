namespace ASP_P42.Data.Entities
{
    public class ProductGroup
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int IsHidden { get; set; } = 0;
    }
}
