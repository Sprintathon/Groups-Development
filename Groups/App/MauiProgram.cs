global using App.ViewModels;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Maui;
global using Microsoft.Extensions.Logging;
global using Plugin.LocalNotification;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
              .UseMauiApp<App>()
             .UseMauiCommunityToolkit()
             .UseLocalNotification()
             .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}


