using Telegram.Bot.Types.ReplyMarkups;

namespace ClassLib
{
    /// <summary>
    /// Эта часть partial-класса HelpingMethods добавляет красивый вывод для пользователя.
    /// </summary>
    public partial class HelpingMethods
    {
        /// <summary>
        /// Метод делает текст цветным.
        /// </summary>
        /// <param name="text">Редактирует этот текст.</param>
        /// <param name="color">Меняет цвет текста, на этот.</param>
        internal static void PrintWithColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Метод создает клавиатуру для взаимодействия с пользователем в боте.
        /// </summary>
        /// <returns> Клавиатуру.</returns>
        internal static ReplyKeyboardMarkup ShowButtons()
        {
            var replyKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton(ConstStrings.funcSelStationStart),
                    new KeyboardButton(ConstStrings.funcSelStationEnd),
                    new KeyboardButton(ConstStrings.funcSelBoth)
                },
                new KeyboardButton[]
                {
                    new KeyboardButton(ConstStrings.funcSortTimeStart),
                    new KeyboardButton(ConstStrings.funcSortTimeEnd)
                },
                new KeyboardButton[]
                {
                    new KeyboardButton(ConstStrings.funcUploadCsv),
                    new KeyboardButton(ConstStrings.funcUploadJson),
                }
            })
            { ResizeKeyboard = true };
            return replyKeyboard;
        }
        /// <summary>
        /// Метод-пасхалка.
        /// </summary>
        /// <returns> Инлайн кнопку, с пасхалкой.</returns>
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