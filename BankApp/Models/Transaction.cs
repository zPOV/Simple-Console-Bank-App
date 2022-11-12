namespace BankApp.Models
{
    public class Transaction
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public Transaction()
        {

        }
        public Transaction(string sender, string recipient, decimal amount, DateTime transactionDate)
        {
            Sender = sender;
            Recipient = recipient;
            Amount = Math.Round(amount, 2);
            TransactionDate = transactionDate;
        }
    }
}
