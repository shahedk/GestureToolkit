using System;

namespace Gestures.Exceptions
{
    public class UnexpectedDataFormatException : Exception
    {
        public UnexpectedDataFormatException(string message) : base(message) { }
    }
}
