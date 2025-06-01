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
