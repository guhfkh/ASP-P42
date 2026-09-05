namespace ASP_P42.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int IsHidden { get; set; } = 0;

        public ProductGroup Group { get; set; } = null!;
        public ICollection<ProductVersion> Versions { get; set; } = [];
    }
}
