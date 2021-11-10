using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IPlatformInfo
    {
        bool HpPlatformDetected { get; }
        string Product { get; }
        string SerialNumber { get; }
        bool SupportsBiosFlash { get; }
    }
}
