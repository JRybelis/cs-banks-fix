using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using BankAccountApp.ViewModels;
using BankAccountApp.Views;
using BankAccount.Core.Domain.Enums;
using BankAccount.Core.Interfaces.Domain;
using BankAccountBase.Core.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccountApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            
            // Create distinct accounts, first
            var checkingAccount = AccountFactory.CreateAccount(
                AccountType.Checking, "Alice", 500m);
            var savingsAccount = AccountFactory.CreateAccount(
                AccountType.Savings, "Bob", 5m);

            // Register services with explicit types
            services.AddSingleton<IAccount>(serviceProvider  => checkingAccount);
            services.AddSingleton<IAccount>(serviceProvider  => savingsAccount);
            
            // Register ViewModel with named dependencies 
            services.AddSingleton(serviceProvider => new MainWindowViewModel(
                checkingAccount, savingsAccount));
                
            
            var serviceProvider = services.BuildServiceProvider();
            
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}