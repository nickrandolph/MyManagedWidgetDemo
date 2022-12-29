using System.Runtime.InteropServices;
using WidgetsHelpers;

namespace MyUWPWidget;

[Guid("29B1E7D7-1CD0-4713-BE54-BBD29F86E8B1")]
internal class WidgetProvider:WidgetProviderBase
{
    static WidgetProvider()
    {
        RegisterWidget<MultiplyingWidget>("Multiplying");
    }
}
