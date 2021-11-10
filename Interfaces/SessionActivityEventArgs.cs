using ToolFrameworkPackage;

namespace Interfaces
{
    public class SessionManagerActivityEventArgs
    {
        /// <summary>
        /// The event to fire to communicate state manager state
        /// </summary>
        /// <param name="state">The tool that has made progress</param>
        /// <param name="session">When defined, the session the state relates to</param>
        public SessionManagerActivityEventArgs(int queueSize, SessionData activeSession = null)
        {
            QueueSize = queueSize;
            ActiveSession = activeSession;
        }
        public int QueueSize { get; }
        public SessionData ActiveSession { get; }
    }
}
