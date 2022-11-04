using System.Text;
using System.Security.Cryptography;
using BankApp.Models;

namespace BankApp.Services
{
    public class BankService
    {
        public BankAccount GetRecipientAccount(List<Client> clients, List<BankAccount> bankAccounts, string guid)
        {
            if (Guid.TryParse(guid, out Guid result))
            {
                var clientId = clients.Where(client => result == client.Guid).FirstOrDefault()?.Id;
                if (clientId == null)
                {
                    return null;
                }
                return bankAccounts.Where(bankAccount => clientId == bankAccount.ClientId).FirstOrDefault();
            }
            return null;
        }

        //static string passwordHasher(string password)
        //{
        //    SHA256 crypt = new SHA256Managed();

        //    string hash = String.Empty;
        //    byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(password));
        //    foreach (byte theByte in crypto)
        //    {
        //        hash += theByte.ToString("x2");
        //    }
        //    return hash;
        //}
        public string StringtoHashString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using (var sha = new SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
