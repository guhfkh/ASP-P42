namespace ASP_P42.Data.Entities
{
    public class UserRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CreateLevel { get; set; }
        public int ReadLevel { get; set; }
        public int UpdateLevel { get; set; }
        public int DeleteLevel { get; set; }
    }
}
