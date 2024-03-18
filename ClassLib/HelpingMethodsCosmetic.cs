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
    }
}