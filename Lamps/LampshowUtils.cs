using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetPinProc.Domain
{
    /// <summary>
    /// LampShow Utilities
    /// </summary>
    public static class LampshowUtils
    {
        /// <summary>
        /// Expands special characters &lt;&gt; [] within 'str' and returns the dots-and-spaces representation.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExpandLine(string str)
        {
            str = Regex.Replace(str, @"(\[[\- ]*\])", delegate (Match match)
            {
                return new string('.', match.Groups[1].Length);
            });

            str = Regex.Replace(str, @"(\<[\- ]*\])", delegate (Match match)
            {
                string f = FadeIn(match.Groups[1].Length);
                return f.Substring(0, f.Length - 1) + ".";
            });

            str = Regex.Replace(str, @"(\[[\- ]*\>)", delegate (Match match)
            {
                string f = FadeOut(match.Groups[1].Length);
                return "." + f.Substring(1);
            });

            str = Regex.Replace(str, @"(\<[\- ]*\>)", delegate (Match match)
            {
                return FadeInOut(match.Groups[1].Length);
            });

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string FadeInOut(int length)
        {
            string a = FadeIn(length / 2);
            string b = FadeOut(length / 2);
            if ((length % 2) == 0)
                return a.Substring(0, a.Length - 1) + " " + b;
            else
                return a + " " + b;
        }

        /// <summary>
        /// Uses <see cref="MakePatternOfLength(int)"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string FadeIn(int length) => MakePatternOfLength(length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string FadeOut(int length)
        {
            string s = FadeIn(length);
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Creates a pattern of length, if length is to long then uses default pattern length
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string MakePatternOfLength(int l)
        {
            string pattern = ".  .  . . .. .. ... ... .... .... ..... .....";
            if (l > pattern.Length) l = pattern.Length;
            string s = pattern.Substring(0, l);
            if (s.Length < l)
            {
                s += String.Concat(Enumerable.Repeat(".", l - s.Length));
            }
            return s;
        }
    }
}
