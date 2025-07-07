namespace IterumApi.DTOs
{
    [Serializable]
    public class LoginForm
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginForm(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
