using System;
namespace Gestures.Exceptions
{
    public class InvalidDataSetException : Exception
    {
        public InvalidDataSetException(string message)
            : base(message)
        { }
    }
}
