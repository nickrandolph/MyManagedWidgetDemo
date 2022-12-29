using COMHelpers;
using Microsoft.Windows.Widgets.Providers;

namespace MyUWPWidget;

public class Program
{
    [STAThread]
    static async Task Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();

        if (args?.Any(x => x.Contains("RegisterProcessAsComServer")) ?? false)
        {
            ComServer<IWidgetProvider, WidgetProvider>.Instance.Run();
        }
    }

}
