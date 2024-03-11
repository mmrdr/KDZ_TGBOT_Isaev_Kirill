using Telegram.Bot;

namespace Smth
{
    internal class Program
    {
        private static TelegramBotClient botClient;

        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("7067595343:AAFGej232xNIbzGt91dXZP-_rMgL0R8BHfQ") { Timeout = TimeSpan.FromSeconds(10) };
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Bot id: {me.Id}, Bot name: {me.FirstName} ");
            Console.ReadKey();
        }
    }
}