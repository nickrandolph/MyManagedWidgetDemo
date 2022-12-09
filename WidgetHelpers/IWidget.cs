using System.Linq;
using Microsoft.Windows.Widgets.Providers;

namespace WidgetsHelpers;

public interface IWidget
{
    string? Id { get; }
    string? State { get; }

    bool IsActivated { get; }

    void Activate();
    void Deactivate();
    void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs);
    void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs);
    string GetTemplateForWidget();
    string GetDataForWidget();
}