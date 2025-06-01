/*************************************************************************
 *                                                                       *
 *  File:        IObserver.cs                                            *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *               Cojocaru Valentin                                        *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Declares the IObserver interface for the Observer       *
 *               design pattern, representing an entity that receives    *
 *               updates from a subject.                                 *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

/// <summary>
/// Defines an observer that can receive updates from a subject.
/// </summary>
public interface IObserver
{
    /// <summary>
    /// Gets the unique identifier of the observer.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Receives an update message from the subject.
    /// </summary>
    /// <param name="message">The message sent by the subject.</param>
    bool Update(string message);
}
