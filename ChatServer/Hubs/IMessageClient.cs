using System.Threading.Tasks;

namespace ChatServer.Hubs
{
    public interface IMessageClient
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveMoodMessage(string user, string caption, byte[] imageData);
    }
}
