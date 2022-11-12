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
            List<Transaction> transactionHistoryData,
            Client client,
            string transactionHistoryPath)
        {
            Console.Write("Bank account of the person to whom you want to transfer your funds: ");
            string recipientGuid = Console.ReadLine();
            Console.Write("Please specify the amount(USD) you want to transfer: ");
            var amount = Console.ReadLine();

            if (!decimal.TryParse(amount, out var result))
            {
                Console.WriteLine("Wrong input given\n");
                return;
            }

            DateTime date = DateTime.Now;
            var recipientBankAccount = _bankService.GetRecipientAccount(clientData, bankAccountData, recipientGuid);


            if (recipientBankAccount == null)
            {
                Console.WriteLine("\nThis person doesn't exist\n");
                return;
            }
            var currentClientBankAccount = _clientManager.GetClientBankAccount(client, bankAccountData);
            var currentRecipient = _clientManager.FindClientByGuid(clientData, recipientGuid);
            if (result <= currentClientBankAccount.Balance && result > 0)
            {
                Console.WriteLine($"You will send {amount} USD to {currentRecipient.FirstName} {currentRecipient.LastName}. Is that correct? y/n\n");
                var answer = Console.ReadLine();
                if (answer == "y")
                {

                    _balanceManager.SubtractClientBalance(client, bankAccountData, result);
                    _balanceManager.AddBalance(result, recipientBankAccount);
                    AddToTransactionHistory(transactionHistoryData, client.FirstName, recipientGuid, result, date);
                    Console.WriteLine($"You have transfered {result} USD to {currentRecipient.FirstName} {currentRecipient.LastName}!\n");
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
        public void ShowHistory(List<Transaction> transactionHistoryData, Client client)
        {
            var currentClientTransactions = GetTransactionHistory(transactionHistoryData, client);
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
            Console.WriteLine("\n");
        }
        public void AddToTransactionHistory(List<Transaction> transactionHistory, string sender, string recipient, decimal amount, DateTime transactionDate)
        {
            var transaction = new Transaction(sender, recipient, amount, transactionDate);

            transactionHistory.Add(transaction);
        }
        public List<Transaction> GetTransactionHistory(List<Transaction> transactionHistory, Client currentClient)
        {
            var currentClientTransactions = transactionHistory
                .Where(transactionHistory => currentClient.FirstName == transactionHistory.Sender
                || currentClient.FirstName == transactionHistory.Recipient).ToList();

            return currentClientTransactions;
        }
    }
}
