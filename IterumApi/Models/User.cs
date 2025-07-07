namespace IterumApi.Models
{
    public class User
    {
        public User()
        {
            
        }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public long Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
