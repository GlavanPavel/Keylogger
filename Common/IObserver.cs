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
    void Update(string message);
}
