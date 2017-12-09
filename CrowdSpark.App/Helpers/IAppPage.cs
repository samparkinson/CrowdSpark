using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace CrowdSpark.App.Helpers
{
    //This class is to store common functions used by all pages. Ex: search, menu options
    interface IAppPage
    {
        //Menu option navigation
        void OptionsList_ItemClick(object sender, ItemClickEventArgs e);

        //Search 
        void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args);
    }
}
