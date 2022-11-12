namespace BankApp.Models
{
    public class Deposit
    {
        public int DepositID { get; set; }
        public int ClientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }

        public Deposit()
        {

        }

        public Deposit(int depositsID, decimal amount, int clientId, DateTime startDate)
        {
            Amount = Math.Round(amount, 2);
            ClientId = clientId;
            StartDate = startDate;
            DepositID = depositsID;
        }
    }

}
