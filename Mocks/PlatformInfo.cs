using Interfaces;

namespace Mocks
{
    public class PlatformInfo : IPlatformInfo
    {
        public bool HpPlatformDetected => true;

        public string Product => "EliteBook";

        public string SerialNumber => "SN123";

        public bool SupportsBiosFlash => true;
    }
}
