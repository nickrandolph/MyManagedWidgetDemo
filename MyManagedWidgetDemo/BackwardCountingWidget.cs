using Microsoft.Windows.Widgets.Providers;
using System;
using WidgetsHelpers;
using Windows.Storage;
using Windows.System;

namespace MyManagedWidgetDemo;

internal class BackwardCountingWidget : WidgetBase
{ 
    private const int StartingNumber = 10000;
    private int _clickCount = StartingNumber;

    public override string State { 
        get => base.State;
        set {
            base.State = value;
            if (string.IsNullOrWhiteSpace(value))
            {
                State = StartingNumber.ToString();
            }
            else
            {
                try
                {
                    // This particular widget stores its clickCount
                    // in the state as integer. Attempt to parse the saved state
                    // and convert it to integer.
                    _clickCount = int.Parse(value);
                }
                catch
                {
                    // Parsing the state was not successful: cached state might've been corrupted.
                    State = StartingNumber.ToString();
                }
            }
        }
    }

    // This function wil be invoked when the Increment button was clicked by the user.
    public override void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
    {
        var verb = actionInvokedArgs.Verb;
        if (verb == "dec")
        {
            // Decrement the count
            _clickCount--;
            State = _clickCount + "";

            // Generate template/data you want to send back
            // The template has not changed so it does not neeed to be updated
            var widgetData = GetDataForWidget();

            // Build update options and update the widget
            WidgetUpdateRequestOptions updateOptions = new WidgetUpdateRequestOptions(Id);
            updateOptions.Data = widgetData;
            updateOptions.CustomState = State;

            WidgetManager.GetDefault().UpdateWidget(updateOptions);
        }
    }

    public override string GetTemplateForWidget()
    {
        // This widget has the same template for all the sizes/themes so we load it only once.
        var widgetTemplate = GetTemplateFromFile("ms-appx:///Templates/BackwardCountingWidgetTemplate.json");
        return widgetTemplate;
    }

    public override string GetDataForWidget()
    {
        return "{ \"count\": " + State + " }";
    }

}
