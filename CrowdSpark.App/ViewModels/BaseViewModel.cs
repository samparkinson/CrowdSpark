using CrowdSpark.App.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Security.Credentials;

namespace CrowdSpark.App.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //Options for hamburger menu, every page has a hamburger menu
        public ObservableCollection<MenuOption> MenuOptions { get; set; }

        //TODO: Hamburger menu relay command
        public ICommand HamburgerMenuCommand { get; }

        //Every page should have a sign in out functionality
        public ICommand SignInOutCommand { get; set; }
        
        //store login information, 
        //should this be in base class or common attributes static class??
        public WebAccount account { get; set; }
        
        //set button text on top left, Sign In as default
        private string _signInOutButtonText;
        public string SignInOutButtonText {
            get { return _signInOutButtonText; }
            set {
                _signInOutButtonText = value;
                OnPropertyChanged();
            }
        }

        //store user name
        private string _userName;
        public string UserName {
            get { return _userName; }
            set {
                _userName = value;
                OnPropertyChanged();
            } }

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
