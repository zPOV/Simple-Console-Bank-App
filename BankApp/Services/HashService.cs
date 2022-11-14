using System.Text;
using System.Security.Cryptography;
using BankApp.Models;

namespace BankApp.Services
{
    public class HashService
    {
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
