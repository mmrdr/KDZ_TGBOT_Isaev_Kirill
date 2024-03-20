using Telegram.Bot.Types.ReplyMarkups;

namespace ClassLib
{
    public partial class HelpingMethods
    {
        /// <summary>
        /// This method makes text more beauty.
        /// </summary>
        /// <param name="text">This text will be modified</param>
        /// <param name="color">Text will be decorated by this color</param>
        internal static void PrintWithColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Displays buttons in the bot.
        /// </summary>
        /// <returns> Keyboard.</returns>
        internal static ReplyKeyboardMarkup ShowButtons()
        {
            var replyKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Выборка по StationStart"),
                    new KeyboardButton("Выборка по StationEnd"),
                    new KeyboardButton("Выборка по StationStart,StationEnd")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Сортировка по TimeStart(в порядке увеличения времени)"),
                    new KeyboardButton("Сортировка по TimeEnd(в порядке увеличения времени)")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Выгрузить файл в формате CSV"),
                    new KeyboardButton("Выгрузить файл в формате JSON"),
                }
            })
            { ResizeKeyboard = true };
            return replyKeyboard;
        }

        internal static InlineKeyboardMarkup ShowMagic()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithUrl("Click", "https://www.youtube.com/watch?v=5yb2N3pnztU&ab_channel=TOHOanimation%E3%83%81%E3%83%A3%E3%83%B3%E3%83%8D%E3%83%AB"),
                },
            });
            return inlineKeyboard;
        }
    }
}