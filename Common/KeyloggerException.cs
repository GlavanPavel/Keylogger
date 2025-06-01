using System;

namespace Common
{
    /// <summary>
    /// Represents errors that occur during keylogger operations.
    /// </summary>
    public class KeyloggerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyloggerException"/> class.
        /// </summary>
        public KeyloggerException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyloggerException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public KeyloggerException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyloggerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public KeyloggerException(string message, Exception inner) : base(message, inner) { }
    }
}
