using System;
using Microsoft.UI.Xaml.Controls;

namespace PrimeCheque.Services.Interfaces
{
    public interface INavigationService
    {
        Frame? Frame { get; set; }
        bool Navigate(Type pageType, object? parameter = null);
        bool GoBack();
        bool CanGoBack { get; }
    }
}
