using Microsoft.Extensions.DependencyInjection;
using System;

using System.Windows;
using VowScriptHelper.Interfaces;
using VowScriptHelper.Services;

namespace VowScriptHelper
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IDialogueHandler, DialogueService>();
        }
    }

}
