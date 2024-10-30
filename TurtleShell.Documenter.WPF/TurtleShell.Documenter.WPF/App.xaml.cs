using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TurtleShell.Documenter.WPF.Documenter;
using TurtleShell.Documenter.WPF.Tests;
using TurtleShell.Documenter.WPF.ViewModels;


namespace TurtleShell.Documenter.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDocumentEngine, DocumentEngine>();
            services.AddTransient<MainWindowViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
