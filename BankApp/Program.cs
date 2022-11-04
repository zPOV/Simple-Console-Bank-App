using BankApp.Services;

namespace BankApp
{
    public class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("1. Ang 2. Pol");
            Language language;
            string answer = Console.ReadLine();

            if (answer == "1")
            {
                language = Language.English;
            }
            else if (answer == "2")
            {
                language = Language.Polish;
            }
            else
            {
                throw new Exception("o ty gnojku");
            }

            var Bank1 = new BankProcess(language);
            Bank1.Process();
        }
    }
}