namespace IterumApi.DTOs
{
    [Serializable]
    public class RegisterForm
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterForm(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
