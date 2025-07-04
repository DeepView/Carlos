using System;
using System.Text.RegularExpressions;
namespace Carlos.Extends
{
    /// <summary>
    /// 常用的正则表达式集合。
    /// </summary>
    public static class CommonRegexExpression
    {
        /// <summary>
        /// 获取一个表示电子邮件地址的正则表达式。
        /// </summary>
        public static string Email => @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        /// <summary>
        /// 获取一个表示电话号码的正则表达式。
        /// </summary>
        public static string PhoneNumber => @"^\+?[1-9]\d{1,14}$";
        /// <summary>
        /// 获取一个表示中国大陆手机号码的正则表达式。
        /// </summary>
        public static string PhoneNumberCN => @"^1[3-9]\d{9}$";
        /// <summary>
        /// 获取一个表示URL的正则表达式。
        /// </summary>
        public static string Url => @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";
        /// <summary>
        /// 获取一个表示URL的正则表达式（扩展版）。
        /// </summary>
        public static string UrlEx => @"^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+";
        /// <summary>
        /// 获取一个表示IPv4地址的正则表达式。
        /// </summary>
        public static string IPv4 => @"^(25[0-5]|2[0-4]\d|[01]?\d\d?)\.(25[0-5]|2[0-4]\d|[01]?\d\d?)\.(25[0-5]|2[0-4]\d|[01]?\d\d?)\.(25[0-5]|2[0-4]\d|[01]?\d\d?)$";
        /// <summary>
        /// 获取一个表示IPv6地址的正则表达式。
        /// </summary>
        public static string IPv6 => @"^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:))$";
        /// <summary>
        /// 获取一个表示日期时间的正则表达式（ISO8601格式）。
        /// </summary>
        public static string DateTime => @"^(\d{4})-(\d{2})-(\d{2})[T\s](\d{2}):(\d{2}):(\d{2})(\.\d+)?([+-]\d{2}:\d{2}|Z)?$";
        /// <summary>
        /// 获取一个表示日期时间的正则表达式（ISO8601格式，支持日期和日期时间）。
        /// </summary>
        public static string DateTimeEx => @"^(\d{4})-(\d{2})-(\d{2})[T\s](\d{2}):(\d{2}):(\d{2})(\.\d+)?([+-]\d{2}:\d{2}|Z)?$|^\d{4}-\d{2}-\d{2}$";
        /// <summary>
        /// 获取一个表示中国身份证号码的正则表达式，支持15位、18位和17位加校验位的格式。
        /// </summary>
        public static string IdNumberCN => @"^\d{15}|\d{18}$|^\d{17}[\dXx]$";
        /// <summary>
        /// 获取一个表示中文字符的正则表达式。
        /// </summary>
        public static string ChineseCharacters => @"[\u4e00-\u9fa5]";
        /// <summary>
        /// 获取一个表示扩展的中文字符的正则表达式，包括CJK统一汉字扩展A、B、C、D区等。
        /// </summary>
        public static string ChineseCharactersEx => @"[\u4e00-\u9fa5\u3400-\u4DBF\u20000-\u2A6DF\u2A700-\u2B73F\u2B740-\u2B81F]";
        /// <summary>
        /// 获取一个表示繁体中文字符的正则表达式，包括CJK统一汉字扩展A、B、C、D区等。
        /// </summary>
        public static string TraditionalChineseCharacters => @"[\u4E00-\u9FFF\u3400-\u4DBF\u20000-\u2A6DF\u2A700-\u2B73F\u2B740-\u2B81F]";
        /// <summary>
        /// 获取一个表示中国邮政编码的正则表达式。
        /// </summary>
        public static string ChinesePostalCode => @"^\d{6}$";
        /// <summary>
        /// 确定指定的输入字符串是否与给定的正则表达式模式匹配。
        /// </summary>
        /// <remarks>此方法使用<see cref="Regex.IsMatch(string, string)"/>方法执行匹配操作。确保模式是有效的正则表达式。</remarks>
        /// <param name="input">要根据模式进行测试的输入字符串。</param>
        /// <param name="pattern">要匹配的正则表达式模式，不能为null或空。</param>
        /// <returns>该操作将会返回一个Boolean类型的数据，表示字符串是否与指定的正则表达式是否匹配，如果匹配则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentException">如果参数给定的正则表达式为空或者空白，则将会抛出这个异常。</exception>
        public static bool Match(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) 
                throw new ArgumentException("The pattern cannot be null or empty.");
            return Regex.IsMatch(input, pattern);
        }
    }
}
