using System;
namespace Carlos.Extends
{
    /// <summary>
    /// 终端（Terminal）扩展方法。
    /// </summary>
    public static class TerminalExtends
    {
        /// <summary>
        /// 用指定颜色输出文本（自动恢复原色）。
        /// </summary>
        public static void ColorOutput(string text, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.Write(text);
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
        /// <summary>
        /// 用指定颜色输出文本，并在文本末尾追加一个换行符（自动恢复原色）。
        /// </summary>
        public static void ColorOutputLine(string text, ConsoleColor color) => ColorOutput(text + Environment.NewLine, color);
    }
}
