namespace CrowdSpark.App.Helpers
{
    class MenuOption
    {
        public string Text { get; set; }

        public string Icon { get; set; }

        public MenuOption(string Icon, string Text)
        {
            this.Text = Text;

            this.Icon = Icon;
        }
    }
}
