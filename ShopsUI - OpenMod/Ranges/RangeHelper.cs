using System;
using System.Linq;

namespace ShopsUI.Ranges
{
    internal static class RangeHelper
    {
        /// <summary>
        /// Parses a single range (from one number to another).
        /// </summary>
        /// <remarks>
        /// Valid string representations:
        /// <code>100</code>
        /// <code>100-200</code>
        /// <code>200-100</code>
        /// <code>100 - 200</code>
        /// <code>100 - 100</code>
        /// <code>  100 - 200  </code>
        /// </remarks>
        /// <param name="unparsed">The unparsed, string representation of the range.</param>
        public static Range ParseSingle(string unparsed)
        {
            try
            {
                if (string.IsNullOrEmpty(unparsed))
                {
                    throw new RangeParseException("Given string is empty");
                }

                var parts = unparsed.Split('-').Select(x => x.Trim()).ToArray();

                if (parts.Length > 2)
                {
                    throw new RangeParseException("Too many parts in range");
                }

                var first = ushort.Parse(parts.First());
                var last = ushort.Parse(parts.Last());

                return new Range(first, last);
            }
            catch (Exception ex) when (ex is not RangeParseException)
            {
                throw new RangeParseException("Could not parse range", ex);
            }
        }

        /// <summary>
        /// Parses a multi range (from one number to another, many times).
        /// </summary>
        /// <remarks>
        /// Valid string representations:
        /// <code>100</code>
        /// <code>100-200</code>
        /// <code>100;102</code>
        /// <code>100-200;202-300</code>
        /// <code>100-200,202-300</code>
        /// <code>100-200,202-300;450-600</code>
        /// <code>100-200,300-202;450-600</code>
        /// <code>100-200,300-202;450-600;700</code>
        /// </remarks>
        /// <param name="unparsed">The unparsed, string representation of the multi range.</param>
        public static MultiRange ParseMulti(string unparsed)
        {
            try
            {
                if (string.IsNullOrEmpty(unparsed))
                {
                    throw new RangeParseException("Given string is empty");
                }

                var parts = unparsed.Split(',', ';').Select(x => x.Trim()).ToArray();

                var ranges = new Range[parts.Length];

                for (var i = 0; i < parts.Length; i++)
                {
                    ranges[i] = ParseSingle(parts[i]);
                }

                return new MultiRange(ranges);
            }
            catch (Exception ex) when (ex is not RangeParseException)
            {
                throw new RangeParseException("Could not parse range", ex);
            }
        }
    }
}
