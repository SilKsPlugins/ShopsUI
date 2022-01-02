using System;
using System.Text.RegularExpressions;

namespace ShopsUI.UI
{
    public class UIListDetails
    {
        public string Format { get; }

        public string Pattern { get; }

        public string Prefix { get; }

        public UIListDetails(string format)
        {
            Format = format;

            // ReSharper disable once StringLiteralTypo
            const string randomStr = "ASDJUASDUHASDADWDAD";
            var randomFormatted = string.Format(Format, randomStr);
            
            Pattern = Regex.Escape(randomFormatted);
            Pattern = Pattern.Replace(randomStr, @"[0-9]+");

            var index = randomFormatted.IndexOf(randomStr, StringComparison.Ordinal);
            Prefix = index < 0 ? Format : Format.Substring(0, index);
        }

        public int GetIndex(string elementName)
        {
            if (!elementName.StartsWith(Prefix))
            {
                return -1;
            }

            var unparsed = "";

            for (var i = Prefix.Length; i < elementName.Length; i++)
            {
                if (!char.IsNumber(elementName[i])) break;

                unparsed += elementName[i];
            }

            return int.Parse(unparsed);
        }

        public string GetName(int index)
        {
            return string.Format(Format, index);
        }

        public string this[int index] => GetName(index);
    }
}
