using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SapphireDb_Net;
using SapphireDb_Net.Options;

namespace SapphireDbWpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();

            SapphireDb sapphireDb = new SapphireDb(new SapphireDbOptions()
            {
                ServerBaseUrl = "localhost:5000",
                UseSsl = false,
                ApiKey = "net_client",
                ApiSecret = "pw1234"
            });
            services.AddSingleton(sapphireDb);
        }
    }
}