using BankApp.Models;

namespace BankApp.Services
{
    public class BalanceManager
    {
        private readonly BankService _bankService;
        private readonly ClientManager _clientManager;
        public BalanceManager()
        {
            _bankService = new BankService();
            _clientManager = new ClientManager();

        }
        public void Add(Client client, List<BankAccount> bankAccountData)
        {
            Console.Write("Please specify the amount you want to add to your account balance: ");
            var amount = Convert.ToDecimal(Console.ReadLine());
            var currentClientBankAccount = _clientManager.GetClientAccount(client, bankAccountData);
            AddBalance(amount, currentClientBankAccount);
            Console.WriteLine("Your account balance has been updated");
            Console.WriteLine($"Your new balance is now {currentClientBankAccount.Balance} USD");
        }

        public void AddBalance(decimal amount, BankAccount account)
        {
            account.Balance = account.Balance + amount;
        }
        public decimal SubtractClientBalance(Client clientToFind, List<BankAccount> bankAccounts, decimal amount)
        {
            foreach (var bankAccount in bankAccounts)
            {
                if (clientToFind.Id == bankAccount.ClientId)
                {
                    bankAccount.Balance = bankAccount.Balance - amount;
                    return bankAccount.Balance;
                }
            }
            throw new Exception("Bank account does not exist");
        }
    }
}
