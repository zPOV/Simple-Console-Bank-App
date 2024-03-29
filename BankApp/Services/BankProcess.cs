﻿using BankApp.Models;
using System.Reflection;

namespace BankApp.Services
{
    public enum Language
    {
        English = 1,
        Polish = 2,
    }

    public class BankProcess
    {
        private readonly FileManager _fileManager;
        private readonly IClientManager _clientManager;
        private readonly BalanceManager _balanceManager;
        private readonly TransferManager _transferManager;
        private readonly IConsoleTexts _consoleTexts;
        private const string BankClientCsvName = "BankClientData.csv";
        private const string BankAccountCsvName = "BankAccountData.csv";
        private const string BankTransactionCsvName = "TransactionHistory.csv";
        private const string DataFolder = "data";
        public BankProcess(Language language)
        {
            _fileManager = new FileManager();
            _clientManager = new ClientManager();
            _balanceManager = new BalanceManager();
            _transferManager = new TransferManager();

            if (language == Language.English)
            {
                _consoleTexts = new EnglishConsoleTexts();
            }
            if (language == Language.Polish)
            {
                _consoleTexts = new PolishConsoleTexts();
            }
        }

        public void Process(string username, string password)
        {
            string bankAppExePath = Assembly.GetExecutingAssembly().Location;
            string bankAppFolderPath = Path.GetDirectoryName(bankAppExePath);
            string folderName = Path.Combine(bankAppFolderPath, DataFolder);
            Directory.CreateDirectory(folderName);
            string clientsPath = Path.Combine(folderName, BankClientCsvName);
            string bankAccountPath = Path.Combine(folderName, BankAccountCsvName);
            string transactionHistoryPath = Path.Combine(folderName, BankTransactionCsvName);

            var clientData = _fileManager.ReadData<Client>(clientsPath);

            var bankAccountData = _fileManager.ReadData<BankAccount>(bankAccountPath);

            var TransactionHistoryData = _fileManager.ReadData<Transaction>(transactionHistoryPath);

            Console.WriteLine(_consoleTexts.GetWelcomingMessage());

            Client client = null;

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                client = _clientManager.ClientLogIn(clientData, username, password);
            }

            if (client == null)
            {
                while (true)
                {
                    Console.WriteLine("Would you like to log in or create a new account?\n 1. Log in\n 2. Create a new account");
                    string loginUserInput = Console.ReadLine();
                    if (loginUserInput == "1")
                    {
                        break;
                    }
                    else if (loginUserInput == "2")
                    {
                        _clientManager.Create(clientData, bankAccountData, clientsPath, bankAccountPath);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input\n");
                    }
                }
                Console.Write("Please type in your username: ");
                var usernameTemp = Console.ReadLine();
                Console.Write("Please type in your password: ");
                var passwordTemp = Console.ReadLine();
                client = _clientManager.ClientLogIn(clientData, usernameTemp, passwordTemp);
            }
            Console.WriteLine("\nYou are logged in!\n");
            Console.WriteLine($"Your bank account Identifier is: {client.Guid}");

            while (true)
            {
                Console.WriteLine("What would you like to do?\n 1. Add balance\n 2. Make a transfer\n 3. Show history of transfers\n 4. Log out");
                string userInput = Console.ReadLine();
                if (userInput == "1")
                {
                    _balanceManager.Add(client, bankAccountData);
                }
                else if (userInput == "2")
                {
                    _transferManager.Transfer(clientData, bankAccountData, TransactionHistoryData, client, transactionHistoryPath);
                }
                else if (userInput == "3")
                {
                    _transferManager.ShowHistory(TransactionHistoryData, client);
                }
                else if (userInput == "4")
                {
                    Console.WriteLine("You have been logged out");
                    break;
                }
                _fileManager.WriteData(clientData, clientsPath);
                _fileManager.WriteData(bankAccountData, bankAccountPath);
                _fileManager.WriteData(TransactionHistoryData, transactionHistoryPath);
            }
        }
    }
}
