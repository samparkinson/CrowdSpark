using CrowdSpark.App.ViewModels;

namespace CrowdSpark.App.Helpers
{
    class MenuOption
    {
        //should store the logged in user

        public string Text { get; set; }

        public string Icon { get; set; }

        //to store the page which the option is associated with
        public BaseViewModel ViewModel { get; set; }

        public MenuOption(string Icon, string Text)
        {
            this.Text = Text;

            this.Icon = Icon;

            this.ViewModel = ViewModel;
        }
    }
}
