
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Messaging
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in T> : ICommandHandler
    {
        void HandleAsync(T command);
    }

    public interface IAsyncCommandHandler<in T> : ICommandHandler
    {
        Task HandleAsync(T command);
    }
}
