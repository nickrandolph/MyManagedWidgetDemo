using Microsoft.Windows.Widgets.Providers;
using System;
using Windows.Storage;

namespace WidgetsHelpers;

public abstract class WidgetBase : IWidget
{
    public string? Id { get; init; }
    public virtual string? State { get; set; }

    public bool IsActivated { get; private set; }

    // This function will be invoked when widget is Activated.
    public virtual void Activate()
    {

        // Since this widget doesn't update data for any reason
        // except when 'Increment' button was clicked - 
        // there's nothing to do here. However, for widgets that
        // constantly push updates this is the signal to start
        // pushing those updates since widget is now visible.
        IsActivated = true;
    }

    // This function will be invoked when widget is Deactivated.
    public virtual void Deactivate()
    {
        // This is the moment to stop sending all further updates until
        // Activate() was called again.
        IsActivated = false;
    }

    public virtual void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs) { }
    public virtual void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs) { }
    public abstract string GetTemplateForWidget();
    public abstract string GetDataForWidget();

    /// <summary>
    /// Retrieves widget template json from file packaged with the application
    /// </summary>
    /// <param name="packagePath">The path in the package eg "ms-appx:///Templates/WidgetTemplate.json"</param>
    /// <returns>Json AdaptiveCards template - use adaptivecards.io to design</returns>
    protected string GetTemplateFromFile(string packagePath)
    {
        var uri = new Uri(packagePath);
        // Calling Result is usually not recommended but should be safe to do in the context
        // of a widget and this method needs to return synchronously
        var storageFile = StorageFile.GetFileFromApplicationUriAsync(uri).AsTask().Result;
        return FileIO.ReadTextAsync(storageFile).AsTask().Result;
    }
};
