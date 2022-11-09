using BankApp.Models;

namespace BankApp.Services
{
    public interface IClientManager
    {
        void Create(List<Client> clientData, List<BankAccount> bankAccountData, string clientsPath, string bankAccountPath);
        public Client ClientLogIn(List<Client> clients, string username, string password);
        public BankAccount GetClientAccount(Client clientToFind, List<BankAccount> bankAccounts);
        public void AddNewClientToClientData(List<Client> clientsData, string firstName, string lastName, int age, string username, string password);
        public void AddClientBankAccount(List<BankAccount> bankAccounts, int clientId);
    }

    public class ClientManager : IClientManager
    {
        private readonly FileManager _fileManager;
        private readonly BankService _bankService;

        public ClientManager()
        {
            _fileManager = new FileManager();
            _bankService = new BankService();
        }

        public void Create(List<Client> clientData, List<BankAccount> bankAccountData, string clientsPath, string bankAccountPath)
        {
            Console.Write("First name: ");
            string firstNameInput = Console.ReadLine();
            Console.Write("Last name: ");
            string lastNameInput = Console.ReadLine();
            Console.Write("Age: ");
            int age = Convert.ToInt32(Console.ReadLine());
            if (age < 18)
            {
                Console.WriteLine("You are too young");
                return;
            }
            Console.Write("Username: ");
            string usernameInput = Console.ReadLine();
            Console.Write("Password: ");
            string passwordInput = Console.ReadLine();

            var passwordHash = _bankService.StringtoHashString(passwordInput);

            AddNewClientToClientData(clientData, firstNameInput, lastNameInput, age, usernameInput, passwordHash);
            AddClientBankAccount(bankAccountData, clientData.Last().Id);
            _fileManager.WriteData(clientData, clientsPath);
            _fileManager.WriteData(bankAccountData, bankAccountPath);
            Console.WriteLine("Congratulations, you are now registered as a new client!\n");
        }

        public Client ClientLogIn(List<Client> clients, string username, string password)
        {
            var passwordHashed = _bankService.StringtoHashString(password);

            foreach (var client in clients)
            {
                if (client.Username == username && client.Password == passwordHashed)
                {
                    return client;
                }
            }
            return null;
        }
        public BankAccount GetClientAccount(Client clientToFind, List<BankAccount> bankAccounts)
        {
            foreach (var bankAccount in bankAccounts)
            {
                if (clientToFind.Id == bankAccount.ClientId)
                {
                    return bankAccount;
                }
            }
            throw new Exception("Bank account does not exist");
        }
        public void AddNewClientToClientData(List<Client> clientsData, string firstName, string lastName, int age, string username, string password)
        {
            const int idIfNoClients = 1;
            var guid = Guid.NewGuid();
            if (clientsData.Count == 0)
            {
                var clientIfFirst = new Client(idIfNoClients, firstName, lastName, age, username, password, guid);
                clientsData.Add(clientIfFirst);
            }
            else
            {
                int newestClientId = clientsData.Last().Id;
                int id = newestClientId + 1;
                var client = new Client(id, firstName, lastName, age, username, password, guid);
                clientsData.Add(client);
            }
        }
        public void AddClientBankAccount(List<BankAccount> bankAccounts, int clientId)
        {
            var bankAccount = new BankAccount(clientId, 0);
            bankAccounts.Add(bankAccount);
        }
    }
}
