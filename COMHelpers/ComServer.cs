using static COMHelpers.NativeMethods;

namespace COMHelpers;

public sealed class ComServer<TInterface, TImplementation>
    where TImplementation:TInterface, new()
{
    private ComServer() { }

    private static ComServer<TInterface, TImplementation> _instance = new ComServer<TInterface, TImplementation>();

    public static ComServer<TInterface, TImplementation> Instance
    {
        get { return _instance; }
    }

    // For thread-sync in lock
    private object syncRoot = new object();

    // Whether the server is running
    private bool _bRunning = false;

    /// <summary>
    /// Run the COM server. If the server is running, the function 
    /// returns directly.
    /// </summary>
    /// <remarks>The method is thread-safe.</remarks>
    public void Run()
    {
        lock (syncRoot) // Ensure thread-safe
        {
            // If the server is running, return directly.
            if (_bRunning)
                return;

            // Indicate that the server is running now.
            _bRunning = true;
        }

        try
        {
            //
            // Register the COM class factories.
            // 
            var widget_provider_clsid = HelperMethods.GetGuidFromType(typeof(TImplementation));

            // Register the SimpleObject class object
            int hResult = NativeMethods.CoRegisterClassObject(
                ref widget_provider_clsid,                 
                new TypedClassFactory<TInterface, TImplementation>(),     
                NativeMethods.CLSCTX.LOCAL_SERVER,  
                NativeMethods.REGCLS.MULTIPLEUSE,
                out var widgetProviderFactory);

            if (hResult != 0)
            {
                throw new ApplicationException(
                    "CoRegisterClassObject failed w/err 0x" + hResult.ToString("X"));
            }

            IntPtr evt = NativeMethods.CreateEvent(IntPtr.Zero, true, false, IntPtr.Zero);

            hResult = NativeMethods.CoWaitForMultipleObjects(
                CWMO_FLAGS.CWMO_DISPATCH_CALLS | CWMO_FLAGS.CWMO_DISPATCH_WINDOW_MESSAGES,
                0xFFFFFFFF, 1, new IntPtr[] { evt }, out uint index
                );

            if (hResult != 0)
            {
                // Revoke the registration of SimpleObject on failure
                if (widgetProviderFactory != 0)
                {
                    NativeMethods.CoRevokeClassObject(widgetProviderFactory);
                }
            }

        }
        finally
        {
            lock (syncRoot) // Ensure thread-safe
            {
                _bRunning = false;
            }
        }
    }
}
