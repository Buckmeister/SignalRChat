using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ChatServer.Hubs
{
    public interface IHubConnectionEventLog
    {
        ObservableCollection<string> Events { get; }

        void AddEvent(string newEvent);

        Task AddEventAsync(string newEvent);
    }
}