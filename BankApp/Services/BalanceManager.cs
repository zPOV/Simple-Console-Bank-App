using BankApp.Models;

namespace BankApp.Services
{
    public class BalanceManager
    {
        private readonly ClientManager _clientManager;
        public BalanceManager()
        {
            _clientManager = new ClientManager();

        }
        public void Add(Client client, List<BankAccount> bankAccountData)
        {
            Console.Write("Please specify the amount you want to add to your account balance: ");
            var amount = Convert.ToDecimal(Console.ReadLine());
            amount = Math.Round(amount, 2);
            var currentClientBankAccount = _clientManager.GetClientBankAccount(client, bankAccountData);
            AddBalance(amount, currentClientBankAccount);
            Console.WriteLine("Your account balance has been updated");
            Console.WriteLine($"Your new account balance is now {currentClientBankAccount.Balance} USD");
        }

        public void ShowBalance(Client client, List<BankAccount> bankAccountData)
        {
            var currentClientBankAccount = _clientManager.GetClientBankAccount(client, bankAccountData);
            Console.WriteLine($"Your account balance is: {currentClientBankAccount.Balance}");
        }

        public void AddBalance(decimal amount, BankAccount account)
        {
            account.Balance = account.Balance + amount;
        }

        public void AddBalanceToDeposit(decimal amount, Deposit account)
        {
            account.Amount = account.Amount + amount;
        }

        public void SubtractClientBalance(Client clientToFind, List<BankAccount> bankAccounts, decimal amount)
        {
            foreach (var bankAccount in bankAccounts)
            {
                if (clientToFind.Id == bankAccount.ClientId)
                {
                    bankAccount.Balance = bankAccount.Balance - amount;
                }
            }
        }
    }
}
