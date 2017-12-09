using CrowdSpark.App.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        //what is this??? --Ken
        public event PropertyChangedEventHandler PropertyChanged;

        //Options for hamburger menu, every page has a hamburger menu
        public ObservableCollection<MenuOption> MenuOptions { get; set; }

        //TODO: Hamburger menu relay command
        public ICommand HamburgerMenuCommand { get; }

        //Every page should have a sign in out functionality
        public ICommand SignInOutCommand { get; set; }

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
