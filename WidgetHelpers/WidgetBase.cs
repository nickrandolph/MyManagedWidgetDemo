using Microsoft.Windows.Widgets.Providers;
using System;
using Windows.Storage;

namespace WidgetsHelpers;

public abstract class WidgetBase : IWidget
{
    public string? Id { get; init; }
    public virtual string? State { get; set; }

    public bool IsActivated { get; private set; }

    public virtual void Activate()
    {
        IsActivated = true;
    }

    public virtual void Deactivate()
    {
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
}
