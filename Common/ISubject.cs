/*************************************************************************
 *                                                                       *
 *  File:        ISubject.cs                                             *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *               Cojocaru Valentin                                        *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Declares the ISubject interface for implementing the    *
 *               Observer design pattern, allowing registration,         *
 *               deregistration, and notification of observers.          *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

/// <summary>
/// Defines a subject that can notify registered observers with updates.
/// </summary>
public interface ISubject
{
    /// <summary>
    /// Attaches an observer to the subject so it can receive updates.
    /// </summary>
    /// <param name="observer">The observer to attach.</param>
    void Attach(IObserver observer);

    /// <summary>
    /// Detaches an observer from the subject so it no longer receives updates.
    /// </summary>
    /// <param name="observer">The observer to detach.</param>
    void Detach(IObserver observer);

    /// <summary>
    /// Notifies all attached observers with a given message.
    /// </summary>
    /// <param name="message">The message to send to the observers.</param>
    void Notify(string message);
}
