namespace BankApp.Models
{
    public class BankAccount
    {
        public int ClientId { get; set; }
        public decimal Balance { get; set; }


        public BankAccount()
        {

        }
        public BankAccount(int clientId, decimal balance)
        {
            ClientId = clientId;
            Balance = balance;
        }
    }
}
