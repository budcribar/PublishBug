using ClientInterfaces;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Mocks
{
    public class FakeHtmlLogger : IHTMLLogger
    {
        public string GetLogAsStandalone(IEnumerable<HistoryRecord> records)
        {
            return "";
        }
    }
}
