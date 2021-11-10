using ClientInterfaces;
using Interfaces;
using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Mocks
{
    public class SystemInformation : ISystemInformation
    {
        public List<ISystemInfoCategory> SystemInfoCategories => new() { new SystemInfoCategory { Name = "System" }, new SystemInfoCategory { Name = "Processor" }, new SystemInfoCategory { Name = "Memory" }, new SystemInfoCategory { Name = "Storage" } };

        public IHpProductFamily HpProductFamily => throw new System.NotImplementedException();

        public string Manufacturer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Model { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string SystemId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string ProductId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public long MemorySize { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string BiosDate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string BiosRevision { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string BiosVendor { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string SerialNumber { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string AssetTag { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string KeyboardControllerRevision { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string OperatingSystemImage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string InstalledOSVersion { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string InstalledOSName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string BuildId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string FeatureByte { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsHpPlatform { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Family { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int MemoryFormFactor { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string BornOnDate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int? ServiceVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BuildVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public class SystemInfoCategory : ISystemInfoCategory
    {
        public string Name { get; init; } = "";
    }

    internal class Version : IApplicationVersion
    {
        public int Major => 2;

        public int Minor => 0;

        public int PatchLevel => 0;

        public int Revision => 0;

        public DateTime CompileTime => DateTime.MinValue;

        public int Sprint => 1;

        public int DaysSinceStartOfSprint => 0;

        public void SetProjectInformation(string startDate, int daysInSprint)
        {

        }
        public override string ToString() { return "2.0.0"; }
    }

    public class PassId : IPassId
    {
        public string GetPassId(Guid id)
        {
            return "";
        }
    }

    public class LocalizedSystemInfo : ILocalizedSystemInfo
    {
        public Dictionary<string, string> LocalizedSystem => new Dictionary<string, string>();

        List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>> ILocalizedSystemInfo.LocalizedSystemInfo => new List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>>();
    }

    public class MockSystemInfo : ISystemInfo
    {
        public string SerialNumber => "TestUnit";

        public string BiosVersion => "HP N75 Ver.01.24";

        public string BiosDate => "01/01/2020";

        public string FamilyCode => "103C_5336AN";

        public bool SupportsBiosFlash => true;

        public bool SupportsBattery => true;

        public string Product => "HP EliteBook 840 G3 (Guy)";

        public bool HpPlatformDetected => true;

        public bool HPPlatformDetected => throw new NotImplementedException();

        public string Model => throw new NotImplementedException();

        public string Family => throw new NotImplementedException();

        public string GPUDevice => throw new NotImplementedException();

        public string CPUsDetected => throw new NotImplementedException();

        public string HardwareVersion => throw new NotImplementedException();

        public string FirmwareVersion => throw new NotImplementedException();

        public string InstalledOSVersion => throw new NotImplementedException();

        public bool Is64bitOperatingSystem => throw new NotImplementedException();

        public DateTime ApplicationStartTime => throw new NotImplementedException();

        public IApplicationVersion ApplicationVersion => new Version();

        public bool IsDeveloperFlag => throw new NotImplementedException();

        public ITrace Trace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string InstallationFolder => throw new NotImplementedException();

        public string WebView2FileName => throw new NotImplementedException();

        public void SetProjectInfo(string projectStartDate, int daysInSprint)
        {
            throw new NotImplementedException();
        }
    }
}
