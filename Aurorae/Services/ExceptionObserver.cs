namespace Aurorae.Services;

public static class ExceptionObserver
{
    public static void UseExceptionObserver(this WebApplication app)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            app.Logger.LogCritical(args.ExceptionObject as Exception, "UnhandledException");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            app.Logger.LogError(args.Exception, "UnobservedTaskException");
            args.SetObserved();
        };
    }
}
