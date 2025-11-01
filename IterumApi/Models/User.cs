namespace IterumApi.Models
{
    public class User
    {
        public User()
        {
            
        }

        public User(string username, string password, long roleId)
        {
            Username = username;
            Password = password;
            RoleId = roleId;
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
