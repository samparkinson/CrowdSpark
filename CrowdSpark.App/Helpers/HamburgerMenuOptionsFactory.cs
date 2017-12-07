using System;
using System.Collections.ObjectModel;

namespace CrowdSpark.App.Helpers
{
    class HamburgerMenuOptionsFactory
    {
        public ObservableCollection<MenuOption> MenuOptions { get; set; }

        public HamburgerMenuOptionsFactory(string UserName)
        {
            MenuOptions = new ObservableCollection<MenuOption>();
            
            MenuOptions.Add(new MenuOption("Account", UserName));
            MenuOptions.Add(new MenuOption("Page", "Projects"));
            MenuOptions.Add(new MenuOption("Setting", "Settings"));
            MenuOptions.Add(new MenuOption("Message", "Feedback"));
        }
        
    }
}
