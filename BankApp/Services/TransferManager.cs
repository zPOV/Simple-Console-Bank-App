using BankApp.Models;

namespace BankApp.Services
{
    public class TransferManager
    {
        private readonly FileManager _fileManager;
        private readonly BankService _bankService;
        private readonly BalanceManager _balanceManager;
        private readonly IClientManager _clientManager;

        public TransferManager()
        {
            _fileManager = new FileManager();
            _bankService = new BankService();
            _balanceManager = new BalanceManager();
            _clientManager = new ClientManager();
        }
        public void Transfer(
            List<Client> clientData,
            List<BankAccount> bankAccountData,
            List<Transaction> TransactionHistoryData,
            Client client,
            string transactionHistoryPath)
        {
            Console.Write("Bank account of the person to whom you want to transfer your funds: ");
            string recipientGuid = Console.ReadLine();
            Console.Write("Please specify the amount(USD) you want to transfer: ");
            var amount = Convert.ToDecimal(Console.ReadLine());
            DateTime date = DateTime.Now;
            var recipientBankAccount = _bankService.GetRecipientAccount(clientData, bankAccountData, recipientGuid);

            if (recipientBankAccount == null)
            {
                Console.WriteLine("\nThis person doesn't exist\n");
                return;
            }
            var currentClientBankAccount = _clientManager.GetClientAccount(client, bankAccountData);
            if (amount <= currentClientBankAccount.Balance)
            {
                Console.WriteLine($"You will send {amount} USD to {recipientGuid}. Is that correct? y/n\n");
                var answer = Console.ReadLine();
                if (answer == "y")
                {
                    _balanceManager.SubtractClientBalance(client, bankAccountData, amount);
                    _balanceManager.AddBalance(amount, recipientBankAccount);
                    AddToTransactionHistory(TransactionHistoryData, client.FirstName, recipientGuid, amount, date);
                    _fileManager.WriteData(TransactionHistoryData, transactionHistoryPath);
                    Console.WriteLine($"You have transfered {amount} USD to {recipientGuid}! //{recipientBankAccount.Balance} \n");
                }
                else if (answer == "n")
                {
                    Console.WriteLine("The transfer was canceled\n");
                }
                else
                {
                    Console.WriteLine("Wrong answer value\n");
                }
            }
            else
            {
                Console.WriteLine("Insufficient funds");
            }
        }
        public void ShowHistory(List<Transaction> TransactionHistoryData, Client client)
        {
            var currentClientTransactions = AddTransactionHistory(TransactionHistoryData, client);

            if (currentClientTransactions.Any())
            {
                foreach (Transaction transaction in currentClientTransactions)
                {
                    Console.WriteLine($"{transaction.Sender} has sent {transaction.Recipient} {transaction.Amount} USD. Date: {transaction.TransactionDate}");
                }
            }
            else
            {
                Console.WriteLine("\nThere are no transactions visible on your account\n");
            }
        }
        public void AddToTransactionHistory(List<Transaction> TransactionHistory, string sender, string recipient, decimal amount, DateTime transactionDate)
        {
            var Transaction = new Transaction(sender, recipient, amount, transactionDate);

            TransactionHistory.Add(Transaction);
        }
        public List<Transaction> AddTransactionHistory(List<Transaction> TransactionHistory, Client currentClient)
        {
            var currentClientTransactions = TransactionHistory
                .Where(TransactionHistory => currentClient.FirstName == TransactionHistory.Sender
                || currentClient.FirstName == TransactionHistory.Recipient).ToList();

            return currentClientTransactions;
        }
    }
}
