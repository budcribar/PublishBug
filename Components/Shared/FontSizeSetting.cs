using ClientInterfaces;

namespace Components.Shared
{
    public class FontSizeSetting : IFontSizeSetting
    {
        public int fontSizeMod { get; set; }
        public int baseFontSizePx { get; set; }
        public int baseFontStepPx { get; set; }
        public int zoomScaledFontSize { get; set; }
        public int fontZoomMod { get; set; }
    }
}
