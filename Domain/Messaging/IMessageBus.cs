using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Messaging
{
    public interface IMessageBus
    {
        void Send<T>(T command);
        Task SendAsync<T>(T command);
    }
}
