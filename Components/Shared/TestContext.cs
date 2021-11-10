using ClientInterfaces;
using System;

namespace Components.Shared
{
    class TestContext : ITestContext
    {
        public bool isLayoutPortrait { get; set; }
        public IPoint? windowSize { get; set; }
        public IPoint? canvasSize { get; set; }
        public string id { get; set; } = "";
        public IFontSizeSetting? fontSizeSettings { get; set; }
        public object? localize { get; set; } = null;
        public string[] languages { get; set; } = Array.Empty<string>();
        public string selectedLanguage { get; set; } = "English";
    }
}
