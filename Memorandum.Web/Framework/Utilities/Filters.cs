using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Memorandum.Web.Framework.Utilities
{
    internal static class Filters
    {

        public static string StripHtmlReadable(string rawInput)
        {
            // TODO: Implement wise stripping to header / content
            if(string.IsNullOrWhiteSpace(rawInput))
                return rawInput;

            var ws = Regex.Replace(rawInput, @"<.*?>", " ");
            return Regex.Replace(ws, @"\s+", " ");
        }

        public static string UrlEncode(string input)
        {
            return WebUtility.UrlEncode(input);
        }

        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string ReadableSize(Int64 value)
        {
            if (value < 0) { return "-" + ReadableSize(-value); }
            if (value == 0) { return "0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}