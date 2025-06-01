/*************************************************************************
 *                                                                       *
 *  File:        KeyloggerException.cs                                   *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *               Cojocaru Valentin                                        *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Defines a custom exception type for keylogger-related   *
 *               errors, providing constructors for error messages and  *
 *               inner exceptions, with optional serialization support.  *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

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
