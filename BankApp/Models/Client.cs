namespace BankApp.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid Guid { get; set; }

        public Client()
        {

        }

        public Client(int id, string firstName, string lastName, int age, string username, string password, Guid guid)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Username = username;
            Password = password;
            Guid = guid;
        }
    }
}
