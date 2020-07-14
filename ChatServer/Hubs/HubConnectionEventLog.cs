using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ChatServer.Hubs
{
    public class HubConnectionEventLog : IHubConnectionEventLog
    {
        public HubConnectionEventLog()
        {
            Events = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Events { get; private set; }

        public void AddEvent(string newEvent)
        {
            Events.Add(newEvent);
        }

        public Task AddEventAsync(string newEvent)
        {
            return Task.Run(() => AddEvent(newEvent));
        }
    }
}
