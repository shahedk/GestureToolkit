using System;

namespace Gestures.Exceptions
{
    public class LanguageSyntaxErrorException : Exception
    {
        public LanguageSyntaxErrorException(string message)
            : base(message)
        { }
    }
}
