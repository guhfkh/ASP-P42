namespace ASP_P42.Data.Entities
{
    public class UserAccess
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string Login { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string Dk { get; set; } = null!;

        public UserData UserData { get; set; } = null!;
        public UserRole UserRole { get; set; } = null!;
    }
}
