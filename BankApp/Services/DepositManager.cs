using BankApp.Models;
using BankApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Services
{
    public class DepositManager
    {
        private readonly FileManager _fileManager;
        private readonly BalanceManager _balanceManager;
        private readonly ClientManager _clientManager;
        public DepositManager()
        {
            _fileManager = new FileManager();
            _balanceManager = new BalanceManager();
            _clientManager = new ClientManager();
        }
        public void Deposit(List<Deposit> depositsData, string depositsPath, Client client, List<BankAccount> bankAccounts)
        {
            Console.WriteLine("1. Check deposits status\n2. Create a deposit\n3. Withdraw deposit\n");
            string answer = Console.ReadLine();
            if (answer == "1")
            {
                ShowDepositsStatus(depositsData, client);
            }
            else if (answer == "2")
            {
                Console.WriteLine("How much would you like to deposit?");
                Console.WriteLine("Amount: ");
                int amount = Convert.ToInt32(Console.ReadLine());
                var currentClientBankAccount = _clientManager.GetClientBankAccount(client, bankAccounts);
                if (amount > currentClientBankAccount.Balance)
                {
                    Console.WriteLine("Not enough funds");
                    return;
                }
                Console.WriteLine($"You will deposit {amount}, is that correct? y/n\n");
                string answer2 = Console.ReadLine();
                if (answer2 == "y")
                {
                    CreateNewDeposit(client, depositsData, amount);
                    _balanceManager.SubtractClientBalance(client, bankAccounts, amount);
                    _fileManager.WriteData(depositsData, depositsPath);
                    Console.WriteLine($"You have successfully created a new deposit of {amount} USD.\n");
                }
                else if (answer2 == "n")
                {
                    Console.WriteLine("You have cancelled the creation of your deposit");
                }
            }
            else if (answer == "3")
            {
                ShowDepositsAfterWithdrawal(depositsData, client, depositsPath, bankAccounts);
            }
            else
            {
                Console.WriteLine("Wrong input, please try again");
            }
        }

        public List<Deposit> GetDepositHistory(List<Deposit> depositsData, Client currentClient)
        {
            var currentClientDeposits = depositsData
                .Where(deposit => currentClient.Id == deposit.ClientId).ToList();

            return currentClientDeposits;
        }

        public void ShowDepositsAfterWithdrawal(List<Deposit> depositsData, Client client, string depositsPath, List<BankAccount> bankAccounts)
        {
            var currentClientDeposits = GetDepositHistory(depositsData, client);
            if (currentClientDeposits.Any())
            {
                foreach (Deposit deposit in currentClientDeposits)
                {
                    Console.WriteLine($"{deposit.DepositID}. Your deposit contains {deposit.Amount} USD");
                }
            }
            else
            {
                Console.WriteLine("There are no deposits visible on your account\n");
                return;
            }
            Console.WriteLine("\nType the number of your desired deposit to withdraw the deposit and see the updated amount: ");
            var userInput = Console.ReadLine();

            if (!int.TryParse(userInput, out var result))
            {
                Console.WriteLine("Wrong input given\n");
                return;
            }
            var currentClientBankAccount = _clientManager.GetClientBankAccount(client, bankAccounts);
            var currentDeposit = FindDeposit(depositsData, result);
            var amountToPayOut = FindDepositAmount(depositsData, result);
            var checksIfEligible = CheckIfEligible(currentDeposit);
            if(checksIfEligible == Eligibility.WrongInput)
            {
                Console.WriteLine("Wrong input given");
                return;
            }
            else if (checksIfEligible == Eligibility.InsufficientTimePassed)
            {
                Console.WriteLine("It's impossible to withdraw this deposit yet");
                return;
            }
            else if(checksIfEligible == Eligibility.Eligible)
            {
                _fileManager.WriteData(depositsData, depositsPath);
                _balanceManager.AddBalance(amountToPayOut, currentClientBankAccount);
                Console.WriteLine($"You have successfully withdrew {amountToPayOut} USD.");
            }
        }

        public void ShowDepositsStatus(List<Deposit> depositsData, Client client /*decimal amount*/)
        {
            var currentClientDeposits = GetDepositHistory(depositsData, client);
            if (currentClientDeposits.Any())
            {
                foreach (Deposit deposit in currentClientDeposits)
                {
                    Console.WriteLine($"{deposit.DepositID}. Your deposit, that at the start contained {deposit.Amount} USD now contains {GetDepositAmount(deposit)} USD.");
                }
            }
            else
            {
                Console.WriteLine("There are no deposits visible on your account\n");
                return;
            }
            Console.WriteLine(string.Empty);
        }

        private Eligibility CheckIfEligible(Deposit deposit)
        {
            DateTime date = DateTime.Now;
            if(deposit == null)
            {
                return Eligibility.WrongInput;
            }
            var timeSinceCreationOfDeposit = date.Subtract(deposit.StartDate);
            var timeSinceCreationOfDepositInYears = timeSinceCreationOfDeposit.Days / 365;
            if (timeSinceCreationOfDepositInYears < 1)
            {
                return Eligibility.InsufficientTimePassed;
            }
            return Eligibility.Eligible;
        }

        public decimal GetDepositAmount(Deposit deposit)
        {
            if (deposit == default)
            {
                return default;
            }
            decimal currentDeposit = CalculateCurrentDepositAmount(deposit);
            return currentDeposit;
        }

        public decimal FindDepositAmount(List<Deposit> depositsData, int userInput)
        {
            var deposit = depositsData.Where(deposit => deposit.DepositID == userInput).FirstOrDefault();
            decimal amountToAddToClientAccount = GetUpdatedDeposit(depositsData, userInput);
            depositsData.Remove(deposit);
            return amountToAddToClientAccount;
        }

        public Deposit FindDeposit(List<Deposit> depositsData, int userInput)
        {
            var deposit = depositsData.Where(deposit => deposit.DepositID == userInput).FirstOrDefault();
            decimal amountToAddToClientAccount = GetUpdatedDeposit(depositsData, userInput);
            return deposit;
        }

        public void ShowUpdatedDeposit(List<Deposit> depositsData, int userInput)
        {
            var deposit = depositsData.Where(deposit => deposit.DepositID == userInput).FirstOrDefault();
            if(deposit == default)
            {
                return;
            }
            else
            {
                decimal currentDeposit = CalculateCurrentDepositAmount(deposit);
                Console.WriteLine($"Current amount status of this deposit is: {currentDeposit}");
            }
        }

        public decimal GetUpdatedDeposit(List<Deposit> depositsData, int userInput)
        {
            var deposit = depositsData.Where(deposit => deposit.DepositID == userInput).FirstOrDefault();
            if (deposit == default)
            {
                return default;
            }
            decimal currentDeposit = CalculateCurrentDepositAmount(deposit);
            return currentDeposit;
        }

        public decimal CalculateCurrentDepositAmount(Deposit deposit)
        {
            DateTime date = DateTime.Now;
            var percentuale = 5d;
            var periodRate = 12;
            var timeSinceCreationOfDeposit = date.Subtract(deposit.StartDate);
            double baseCalc = 1 + percentuale / (100 * periodRate);
            double powerCalc = timeSinceCreationOfDeposit.Days / 365 * periodRate;
            double currentAmount = Convert.ToDouble(deposit.Amount) * Math.Pow(baseCalc, powerCalc);
            return Convert.ToDecimal(currentAmount);
        }

        public List<Deposit> CreateNewDeposit(Client client, List<Deposit> depositsData, decimal amount)
        {
            int newestDepositId = depositsData.LastOrDefault()?.DepositID ?? 0;
            int id = newestDepositId + 1;
            var deposit = new Deposit(id, amount, client.Id, DateTime.Now);
            depositsData.Add(deposit);
            return depositsData;
        }
    }
}
