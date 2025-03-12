using System;
using System.Text.RegularExpressions;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    /// <summary>
    /// Auxiliary methods based on regular expressions
    /// </summary>
    public static partial class Core
    {
      
        public static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(3);

        private static readonly Regex _matchAllTags =
            new Regex(@"<(.|\n)*?>", options: RegexOptions.Compiled | RegexOptions.IgnoreCase
#if !NET40
                , matchTimeout: MatchTimeout
#endif
            );

        private static readonly Regex _matchArabicHebrew =
            new Regex(@"[\u0600-\u06FF,\u0590-\u05FF,«,»]", options: RegexOptions.Compiled | RegexOptions.IgnoreCase
#if !NET40
                , matchTimeout: MatchTimeout
#endif
            );

        private static readonly Regex _matchOnlyPersianNumbersRange =
            new Regex(@"^[\u06F0-\u06F9]+$", options: RegexOptions.Compiled | RegexOptions.IgnoreCase
#if !NET40
                , matchTimeout: MatchTimeout
#endif
            );

        private static readonly Regex _matchOnlyPersianLetters =
            new Regex(@"^[\\s,\u06A9\u06AF\u06C0\u06CC\u060C,\u062A\u062B\u062C\u062D\u062E\u062F,\u063A\u064A\u064B\u064C\u064D\u064E,\u064F\u067E\u0670\u0686\u0698\u200C,\u0621-\u0629\u0630-\u0639\u0641-\u0654]+$",
                options: RegexOptions.Compiled | RegexOptions.IgnoreCase
#if !NET40
                , matchTimeout: MatchTimeout
#endif
            );

        internal static readonly Regex _hasHalfSpaces =
            new Regex(@"\u200B|\u200C|\u200E|\u200F",
                options: RegexOptions.Compiled | RegexOptions.IgnoreCase
#if !NET40
                , matchTimeout: MatchTimeout
#endif
            );

        public static bool ContainsFarsi(this string txt)
        {
            return !string.IsNullOrEmpty(txt) &&
                   _matchArabicHebrew.IsMatch(txt.StripHtmlTags().Replace(",", ""));
        }

     
        public static bool ContainsOnlyFarsiLetters(this string txt)
        {
            return !string.IsNullOrEmpty(txt) &&
                   _matchOnlyPersianLetters.IsMatch(txt.StripHtmlTags().Replace(",", ""));
        }

        public static string StripHtmlTags(this string text)
        {
            return string.IsNullOrEmpty(text) ?
                string.Empty :
                _matchAllTags.Replace(text, " ").Replace("&nbsp;", " ");
        }

        /// <summary>
        ///
        /// <div style='text-align: right; font-family:{fontFamily}; font-size:{fontSize};' dir='rtl'>{body}</div>
        /// 
        /// <div style='text-align: left; font-family:{fontFamily}; font-size:{fontSize};' dir='ltr'>{body}</div>
        /// </summary>
        public static string WrapInDirectionalDiv(this string body, string fontFamily = "tahoma", string fontSize = "9pt")
        {
            if (string.IsNullOrWhiteSpace(body))
                return string.Empty;

            if (ContainsFarsi(body))
                return $"<div style='text-align: right; font-family:{fontFamily}; font-size:{fontSize};' dir='rtl'>{body}</div>";
            return $"<div style='text-align: left; font-family:{fontFamily}; font-size:{fontSize};' dir='ltr'>{body}</div>";
        }

        /// <summary>
        /// </summary>
        public static bool ContainsOnlyPersianNumbers(this string text)
        {
            return !string.IsNullOrEmpty(text) &&
                   _matchOnlyPersianNumbersRange.IsMatch(text.StripHtmlTags());
        }

        /// <summary>
        /// </summary>
        public static bool ContainsHalfSpace(this string text)
            => _hasHalfSpaces.IsMatch(text);
        //=> text.Contains((char) 8203) || text.Contains((char) 8204) || 
        //   text.Contains((char) 8206) || text.Contains((char) 8207);

        /// <summary>
        /// </summary>
        public static bool ContainsThinSpace(this string text)
            => ContainsHalfSpace(text);
    }
}