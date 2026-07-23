using System;
using Microsoft.UI.Xaml.Controls;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class NavigationService : INavigationService
    {
        public Frame? Frame { get; set; }

        public bool Navigate(Type pageType, object? parameter = null)
        {
            if (Frame == null)
                return false;

            return Frame.Navigate(pageType, parameter);
        }

        public bool GoBack()
        {
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
                return true;
            }
            return false;
        }

        public bool CanGoBack => Frame?.CanGoBack ?? false;
    }
}
