using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BybitTrader
{
    public partial class MainPage : ContentPage
    {
        private bool isPulsating = false;

        public MainPage()
        {
            InitializeComponent();
            LoadSavedCredentials();
            LoadTradingViewChart();
        }

        private void LoadSavedCredentials()
        {
            bool hasSavedKeys = Preferences.ContainsKey("apiKey") && Preferences.ContainsKey("apiSecret");

            // Mindig a login oldalt mutassuk
            LoginSection.IsVisible = true;
            DashboardSection.IsVisible = false;

            if (hasSavedKeys)
            {
                Debug.WriteLine("✅ API kulcsok betöltve. Felhasználónak be kell jelentkeznie.");
                ApiKeyEntry.Text = Preferences.Get("apiKey", "");
                ApiSecretEntry.Text = Preferences.Get("apiSecret", "");
                SaveCredentialsCheckBox.IsChecked = Preferences.Get("saveCredentials", false);
            }
            else
            {
                Debug.WriteLine("❌ Nincsenek mentett API kulcsok. Felhasználónak be kell írnia őket.");
                ApiKeyEntry.Text = "";
                ApiSecretEntry.Text = "";
                SaveCredentialsCheckBox.IsChecked = false;
            }
        }

        private void OnLoginClicked(Object sender, EventArgs e)
        {
            string apiKey = ApiKeyEntry.Text;
            string apiSecret = ApiSecretEntry.Text;
            bool saveKeys = SaveCredentialsCheckBox.IsChecked;

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                DisplayAlert("Error", "Please enter both API key and API Secret", "OK");
                return;
            }

            if (saveKeys)
            {
                Preferences.Set("apiKey", apiKey);
                Preferences.Set("apiSecret", apiSecret);
                Preferences.Set("saveCredentials", true);
            }
            else
            {
                Preferences.Remove("apiKey");
                Preferences.Remove("apiSecret");
                Preferences.Set("saveCredentials", false);
            }

            Debug.WriteLine("🔑 Bejelentkezés sikeres.");

            LoginSection.IsVisible = false;
            DashboardSection.IsVisible = true;

        }

        private void OnLogoutClicked(Object sender, EventArgs e)
        {
            Debug.WriteLine("🚪 Kijelentkezés. Visszatérés a bejelentkezési oldalra.");
            LoginSection.IsVisible = true;
            DashboardSection.IsVisible = false;
        }

        private void UpdateConnectionStatus(bool isConnected)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (isConnected)
                {
                    ConnectionStatusLabel.Text = "Connected";
                    ConnectionStatusLabel.TextColor = Colors.Green;
                    ConnectionStatusIndicator.BackgroundColor = Colors.Green;
                    StartPulsatingEffect();
                }
                else
                {
                    ConnectionStatusLabel.Text = "Disconnected";
                    ConnectionStatusLabel.TextColor = Colors.Red;
                    ConnectionStatusIndicator.BackgroundColor = Colors.Red;
                    StopPulsatingEffect();
                }
            });
        }

        private void OnWebSocketConnected()
        {
            Debug.WriteLine("✅ WebSocket kapcsolat létrejött.");
            UpdateConnectionStatus(true);
        }

        private void OnWebSocketDisonnected()
        {
            Debug.WriteLine("❌ WebSocket kapcsolat megszakadt.");
            UpdateConnectionStatus(false);
        }

        private async void StartPulsatingEffect()
        {
            if (isPulsating) return;
            isPulsating = true;

            while (isPulsating)
            {
                await ConnectionStatusIndicator.ScaleTo(1.2, 500, Easing.Linear);
                await ConnectionStatusIndicator.ScaleTo(1.0, 500, Easing.Linear);
            }
        }

        private void StopPulsatingEffect()
        {
            isPulsating = false;
            ConnectionStatusIndicator.ScaleTo(1.0, 500, Easing.Linear); // Reset scale to default
        }

        private void LoadTradingViewChart()
        {
            string htmlSource = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <script type='text/javascript' src='https://s3.tradingview.com/tv.js'></script>
        </head>
        <body>
            <div id='tradingview_12345'></div>
            <script type='text/javascript'> 
                new TradingView.widget({
                    'container_id': 'tradingview_12345',
                    'autosize': true,
                    'symbol': 'BYBIT:BTCUSDT',
                    'interval': '60',
                    'timezone': 'Etc/UTC',
                    'theme': 'dark',
                    'style': '1',
                    'locale': 'en',
                    'enable_publishing': true,
                    'allow_symbol_change': true,
                    'hideideas': true,
                    'hide_top_toolbar': true
                });
            </script>
        </body>
        </html>";

            TradingViewChart.Source = new HtmlWebViewSource
            {
                Html = htmlSource
            };
        }
    }
}
