using System.Collections.ObjectModel;
using Windows.Security.Credentials;

namespace CrowdSpark.App.Helpers
{
    class HamburgerMenuOptionsFactory
    {
        public ObservableCollection<MenuOption> MenuOptions { get; set; }

        public HamburgerMenuOptionsFactory(WebAccount account)
        {
            //should check if the user is logged in
            MenuOptions = new ObservableCollection<MenuOption>();

            var accountText = account == null ? "Sign In" : account.UserName.Split("@")[0];

            MenuOptions.Add(new MenuOption("Account", accountText));
            MenuOptions.Add(new MenuOption("Page", "Projects"));
            MenuOptions.Add(new MenuOption("Setting", "Settings"));
            MenuOptions.Add(new MenuOption("Message", "Feedback"));
        }
        
    }
}
