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
        // App verzió lekérése és beállítása
        string version = AppInfo.Current.VersionString;
        AppVersionLabel.Text = $"Version: {version}";
    }
}
