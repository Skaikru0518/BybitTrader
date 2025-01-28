using System.Reflection;

namespace BybitTrader.Pages;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
        LoadAppVersion();
    }

    private void LoadAppVersion()
    {
        //var version = Assembly.GetExecutingAssembly().GetName().Version;
        var version = AppInfo.Current.VersionString;
        if (version != null)
        {
            
            AppVersionLabel.Text = $"Version: {version}";
        }
        else
        {
            AppVersionLabel.Text = "Version: 1.0.0.0, Build: 0";
        }
    }
}

