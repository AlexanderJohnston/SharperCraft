using JetBrains.Annotations ;

namespace SharperCraft.Runtime.Platform
{
    public interface INativeClass
    {
        Task Initialize <T> ([CanBeNull]T parent);

        Task Terminate(string reason);
    }
}
