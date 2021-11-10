using ToolFrameworkPackage;

namespace Interfaces
{
    public interface ITestInstanceComponent
    {
        string Id { get; set; }
        string ErrorCode { get; set; }
        string FailureId { get; set; }
        State State { get; set; }

    }
}
