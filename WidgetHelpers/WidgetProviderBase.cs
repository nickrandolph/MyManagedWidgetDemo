
using System.Collections.Generic;
using System;
using Microsoft.Windows.Widgets.Providers;

namespace WidgetsHelpers;

public class WidgetProviderBase
{
    private record WidgetCreationInfo
    (
        string WidgetName,
        Func<WidgetContext, string, IWidget> Factory
    );


    public static T CreateWidget<T>(WidgetContext widgetContext, string state)
        where T : WidgetBase, new()
    {
        var widgetId = widgetContext.Id;
        var newWidget = new T() { Id = widgetId, State = state };

        // Call UpdateWidget() to send data/template to the newly created widget.
        WidgetUpdateRequestOptions updateOptions = new WidgetUpdateRequestOptions(widgetId);
        updateOptions.Template = newWidget.GetTemplateForWidget();
        updateOptions.Data = newWidget.GetDataForWidget();
        // You can store some custom state in the widget service that you will be able to query at any time.
        updateOptions.CustomState = newWidget.State;
        // Update the widget
        WidgetManager.GetDefault().UpdateWidget(updateOptions);
        return newWidget;
    }



    // Register all widget types here, optionally specify an is enabled function
    static IDictionary<string, WidgetCreationInfo> _widgetRegistry = new Dictionary<string, WidgetCreationInfo>();

    protected static void RegisterWidget<TWidget>(string widgetName) 
        where TWidget:WidgetBase, new()
    {
        _widgetRegistry[widgetName] = new WidgetCreationInfo(widgetName, (ctx, state) => CreateWidget<TWidget>(ctx, state));
    }

    IDictionary<string, IWidget> _runningWidgets = new Dictionary<string, IWidget>();

    protected WidgetProviderBase()
    {
        RecoverRunningWidgets();
    }

    public IWidget InitializeWidgetInternal(WidgetContext widgetContext, string state)
    {
        var widgetName = widgetContext.DefinitionId;
        var widgetId = widgetContext.Id;
        if (_widgetRegistry.TryGetValue(widgetName, out var creation))
        {
            var widgetImpl = creation.Factory(widgetContext, state);
            _runningWidgets[widgetId] = widgetImpl;
            return widgetImpl;
        }
        return default;
    }

    public IWidget FindRunningWidget(string widgetId)
    {
        return _runningWidgets.TryGetValue(widgetId, out var widget) ? widget : default;
    }

    // Handle the CreateWidget call. During this function call you should store
    // the WidgetId value so you can use it to update corresponding widget.
    // It is our way of notifying you that the user has pinned your widget
    // and you should start pushing updates.
    public void CreateWidget(WidgetContext widgetContext)
    {
        // Since it's a new widget - there's no cached state and we pass an empty string instead.
        _ = InitializeWidgetInternal(widgetContext, "");
    }

    // Handle the DeleteWidget call. This is notifying you that
    // you don't need to provide new content for the given WidgetId
    // since the user has unpinned the widget or it was deleted by the Host
    // for any other reason.
    public void DeleteWidget(string widgetId)
    {
        _runningWidgets.Remove(widgetId);
    }

    // Handle the OnActionInvoked call. This function call is fired when the user's
    // interaction with the widget resulted in an execution of one of the defined
    // actions that you've indicated in the template of the Widget.
    // For example: clicking a button or submitting input.
    public void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
    {
        var widgetId = actionInvokedArgs.WidgetContext.Id;
        if (FindRunningWidget(widgetId) is { } runningWidget)
        {
            runningWidget.OnActionInvoked(actionInvokedArgs);
        }
    }

    // Handle the WidgetContextChanged call. This function is called when the context a widget
    // has changed. Currently it only signals that the user has changed the size of the widget.
    // There are 2 ways to respond to this event:
    // 1) Call UpdateWidget() with the new data/template to fit the new requested size.
    // 2) If previously sent data/template accounts for various sizes based on $host.widgetSize - you can use this event solely for telemtry.
    public void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs)
    {
        var widgetContext = contextChangedArgs.WidgetContext;
        var widgetId = widgetContext.Id;
        if (FindRunningWidget(widgetId) is { } runningWidget)
        {
            runningWidget.OnWidgetContextChanged(contextChangedArgs);
            // Optionally: if the data/template for the new context is different
            // from the previously sent data/template - send an update.
        }
    }

    // Handle the Activate call. This function is called when widgets host starts listening
    // to the widget updates. It generally happens when the widget becomes visible and the updates
    // will be promptly displayed to the user. It's recommended to start sending updates from this moment
    // until Deactivate function was called.
    public void Activate(Microsoft.Windows.Widgets.Providers.WidgetContext widgetContext)
    {
        var widgetId = widgetContext.Id;
        if (FindRunningWidget(widgetId) is { } runningWidget)
        {
            runningWidget.Activate();
        }
    }

    // Handle the Deactivate call. This function is called when widgets host stops listening
    // to the widget updates. It generally happens when the widget is not visible to the user
    // anymore and any further updates won't be displayed until the widget is visible again.
    // It's recommended to stop sending updates until Activate function was called.
    public void Deactivate(string widgetId)
    {
        if (FindRunningWidget(widgetId) is { } runningWidget)
        {
            runningWidget.Deactivate();
        }
    }

    // This function will be called in WidgetProvider Constructor
    // to get information about all the widgets that this provider
    // is currently providing. It's helpful in case of the Provider
    // subsequent launches to restore the previous state of the running widgets.
    void RecoverRunningWidgets()
    {
        try
        {
            WidgetManager widgetManager = WidgetManager.GetDefault();
            foreach (var widgetInfo in widgetManager.GetWidgetInfos())
            {
                var widgetContext = widgetInfo.WidgetContext;
                var widgetId = widgetContext.Id;
                var customState = widgetInfo.CustomState;

                // Check if this widgetId is known
                if (FindRunningWidget(widgetId) is null)
                {
                    // Hook up an instance with this context
                    // Ignore the return result if this widget is not supported
                    InitializeWidgetInternal(widgetContext, customState);
                }
            }
        }
        catch
        {

        }
    }
}
