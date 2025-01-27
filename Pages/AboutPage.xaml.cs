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
        // App verzi� lek�r�se �s be�ll�t�sa
        string version = AppInfo.Current.VersionString;
        AppVersionLabel.Text = $"Version: {version}";
    }
}
