using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IMainWindow
    {
        void RegisterFramework();
        void IncreaseSize();
        void DecreaseSize();
        void FullScreen();
        void RestoreDown();
        void ShutDown();
        void HideUI();
        void ShowUI();
        void ShutdownApplication();
        void Shutdown();
        void ExportLogs();
    }
}
