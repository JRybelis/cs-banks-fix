using Avalonia.ReactiveUI;
using ReactiveUI;

namespace BankAccountApp.ViewModels;

public class ViewModelBase : ReactiveObject
{
    protected ViewModelBase()
    {
        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
    }
}