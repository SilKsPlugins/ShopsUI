﻿using System;

namespace ShopsUI.Ranges
{
    internal class RangeParseException : Exception
    {
        public RangeParseException()
        {
        }

        public RangeParseException(string message) : base(message)
        {
        }

        public RangeParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
