public interface IObserver
{
    void Update(string message);
    string Id { get; }
}