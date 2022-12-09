using System.Runtime.InteropServices;
using WidgetsHelpers;

namespace MyManagedWidgetDemo;

[GuidAttribute("B346D60C-42FC-4D9F-AB57-DC263E64F243")]
internal class WidgetProvider:WidgetProviderBase
{
    static WidgetProvider()
    {
        RegisterWidget<BackwardCountingWidget>("BackwardCounting");
    }
}
