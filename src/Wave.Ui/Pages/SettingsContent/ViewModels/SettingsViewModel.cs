using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Wave.Ui.Pages.SettingsContent.ViewModels;

public partial class SettingsViewModel : ObservableObject, IQueryAttributable
{
    /***************************
    * VARIABLES AND PROPERTIES *
    ****************************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "Java";

    /***************
    * CONSTRUCTORS *
    ***************/


    /**********
    * METHODS *
    **********/
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        return;
    }
}
