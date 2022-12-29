using COMHelpers;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.Widgets.Providers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyManagedWidgetDemo;

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
        else
        {
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new DispatcherQueueSynchronizationContext(
                    DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
    }

}
