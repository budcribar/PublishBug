using ToolFrameworkPackage;

namespace Interfaces
{
    public interface ISureCheckDirector : IDirector
    {
        bool HasSystemTests { get; }
    }
}
