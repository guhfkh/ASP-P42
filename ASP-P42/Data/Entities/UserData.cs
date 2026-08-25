namespace ASP_P42.Data.Entities
{
    public class UserData
    {
        public Guid Id {  get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; } = null!;
        public DateTime Britdate { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public ICollection<UserAccess> Accesses { get; set; } = [];
    }
}
