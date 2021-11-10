using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SureCheck.Components
{
    public class AppSettings
    {
        public Uri? ServerUrl { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
        public bool IsRestarting { get; set; } = false;
        public string FunctionalTestBasePath { get; set; } = "";
        public Task<string> FunctionalTestResults()
        {
            return Task.Run<string>(() =>
            {
                try
                {
                    var files = Directory.GetFileSystemEntries(FunctionalTestBasePath, "logFile.html", new EnumerationOptions { RecurseSubdirectories = true });

                    var recent = files.Select(x => new DirectoryInfo(x)).OrderBy(x => x.LastWriteTime).Last().FullName;

                    return File.ReadAllText(recent);
                }
                catch (Exception) { }

                return "";
            });
        }
    }

}
