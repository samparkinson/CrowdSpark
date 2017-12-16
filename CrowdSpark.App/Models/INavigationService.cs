using System;

namespace CrowdSpark.App.ViewModels
{
    public interface INavigationService
    {
        bool Navigate(Type sourcePageType, object parameter);
    }
}