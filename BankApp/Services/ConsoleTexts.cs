namespace BankApp.Services
{
    public interface IConsoleTexts
    {
        string GetWelcomingMessage();
    }

    public class EnglishConsoleTexts : IConsoleTexts
    {
        public string GetWelcomingMessage()
        {
            return "Greetings and welcome to Chuj Bank!";
        }
    }
    public class PolishConsoleTexts : IConsoleTexts
    {
        public string GetWelcomingMessage()
        {
            return "Dzień dobry, witamy w banku Chuj!";
        }
    }
}
