using System.Diagnostics;

namespace BybitTrader.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load API keys if saved
            ApiKeyEntry.Text = Preferences.Get("apiKey", "");
            ApiSecretEntry.Text = Preferences.Get("apiSecret", "");

            // Load theme preference
            ThemePicker.SelectedIndex = Preferences.Get("theme", "Dark") == "Dark" ? 0 : 1;

            // Load refresh interval
            string refreshInterval = Preferences.Get("refreshInterval", "5s");
            RefreshIntervalPicker.SelectedIndex = refreshInterval == "5s" ? 0 : (refreshInterval == "10s" ? 1 : 2);

            // Load notification settings
            TradeAlertsSwitch.IsToggled = Preferences.Get("tradeAlerts", false);
            WebSocketAlertsSwitch.IsToggled = Preferences.Get("webSocketAlerts", false);
        }

        private void OnSaveApiKeysClicked(object sender, EventArgs e)
        {
            Preferences.Set("apiKey", ApiKeyEntry.Text);
            Preferences.Set("apiSecret", ApiSecretEntry.Text);
            Debug.WriteLine("✅ API keys saved.");
            DisplayAlert("Success", "API keys saved successfully.", "OK");
        }

        private void OnClearApiKeysClicked(object sender, EventArgs e)
        {
            Preferences.Remove("apiKey");
            Preferences.Remove("apiSecret");
            ApiKeyEntry.Text = "";
            ApiSecretEntry.Text = "";
            Debug.WriteLine("❌ API keys cleared.");
            DisplayAlert("Success", "API keys cleared.", "OK");
        }

        private void OnTestApiConnectionClicked(object sender, EventArgs e)
        {
            string apiKey = ApiKeyEntry.Text;
            string apiSecret = ApiSecretEntry.Text;

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                DisplayAlert("Error", "API keys are required to test connection.", "OK");
                return;
            }

            Debug.WriteLine("🔍 Testing API connection...");
            // Call a function to test API connection (To be implemented)
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            string selectedTheme = ThemePicker.SelectedIndex == 0 ? "Dark" : "Light";
            Preferences.Set("theme", selectedTheme);
            Debug.WriteLine($"🎨 Theme changed to: {selectedTheme}");
            // Apply theme change (To be implemented)
        }

        private void OnRefreshIntervalChanged(object sender, EventArgs e)
        {
            string selectedInterval = RefreshIntervalPicker.SelectedIndex == 0 ? "5s" :
                                      RefreshIntervalPicker.SelectedIndex == 1 ? "10s" : "30s";

            Preferences.Set("refreshInterval", selectedInterval);
            Debug.WriteLine($"🔄 Refresh interval set to: {selectedInterval}");
            // Apply interval change (To be implemented)
        }

        private void OnTradeAlertsToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("tradeAlerts", e.Value);
            Debug.WriteLine($"🔔 Trade alerts: {(e.Value ? "Enabled" : "Disabled")}");
        }

        private void OnWebSocketAlertsToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("webSocketAlerts", e.Value);
            Debug.WriteLine($"📡 WebSocket notifications: {(e.Value ? "Enabled" : "Disabled")}");
        }
    }
}
