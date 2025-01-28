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
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        if (version != null)
        {
            var buildNumber = version.Build;
            AppVersionLabel.Text = $"Version: {version}, Build: {buildNumber}";
        }
        else
        {
            AppVersionLabel.Text = "Version: 1.0.0.0, Build: 0";
        }
    }
}

