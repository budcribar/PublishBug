using System.Windows;

namespace Interfaces
{
    public interface IWindowProperty
    {
        double Top { get; }
        double Left { get; }
        double ActualHeight { get; }
        double ActualWidth { get; }
        FlowDirection FlowDirection { get; }
    }
}
