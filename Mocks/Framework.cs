using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolFrameworkPackage;
namespace Mocks
{
    public class Framework : IToolFramework
    {
        public string CSOSession { get; set; } = String.Empty;
        public string AgentSession { get; set; } = String.Empty;

        public IToolLoader? ToolLoader => null;

        public IDirector? Director { get; set; }

        public IObservable<TestDataReceived> TestDataReceivedEvent => throw new NotImplementedException();


        public IObservable<TestResultPrompt> FireTestResultPromptEvent => throw new NotImplementedException();

        public IObservable<ITraceData> TraceEvent => throw new NotImplementedException();

        public IObservable<StateChangedEventArgs> StateChanged => throw new NotImplementedException();

        public IObservable<ProgressEventArgs> UpdateProgress => throw new NotImplementedException();

        public IObservable<ExceptionEventArgs> ExceptionRaised => throw new NotImplementedException();

        public IPlatform Platform => throw new NotImplementedException();

        public bool NonVolatileStorageExists => true;

        public BackgroundInit<IHistory>? History { get; set; }

        public ITrace Trace => throw new NotImplementedException();

        public bool ToolsCompletedExecution => true;

        public void Abort()
        {

        }

        public void Cancel(string? msg = null)
        {

        }

        public void Execute(ISessionContext? session = null)
        {

        }

        public void Execute(TimeSpan duration, ISessionContext? session = null)
        {

        }

        public Task<bool> ExecuteAsync(TimeSpan duration, ISessionContext? session = null)
        {
            return Task.FromResult(true);
        }

        public void FireExceptionRaised(string name, string message, string? stackTrace = null)
        {

        }

        public void FireStateChanged(ITool tool, State prev, State curr)
        {

        }

        public void FireTestDataReceived(ITool tool, object data)
        {

        }

        public void FireTestEvent(string data)
        {

        }

        public void FireTestEvent(Guid id, string data)
        {

        }

        public void FireTestResultPrompt(ITool tool, object data)
        {

        }

        public void FireTraceEvent(ITraceData data)
        {

        }

        public void FireUpdateProgress(ITool tool, int percentComplete)
        {

        }

        public double GetCpuTemp() => 0;

        public double GetCpuUtilization() => 0;


        public double GetGpuTemp() => 0;


        public Status GetStatus() => new Status();

        public IEnumerable<ToolStatus> GetToolStatus() => Enumerable.Empty<ToolStatus>();

        public string LogFile(Guid id) => "";

        public void Reboot() { }

        public void RestoreNonVolatileStorage() { }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }

        public bool Stage(Guid id, int sequenceId = 0)
        {
            return true;
        }

        public ITool? Tool(Guid id)
        {
            return null;
        }

        public IEnumerable<ToolPacket> ToolsAbout()
        {
            return Enumerable.Empty<ToolPacket>();
        }

        public ToolState ToolState(StateChangedEventArgs evt)
        {
            return new ToolState();
        }

        public bool UnStage(Guid id, int sequenceId = 0)
        {
            return true;
        }
    }
}
