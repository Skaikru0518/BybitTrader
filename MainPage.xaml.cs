using BybitTrader.Components;
using Microsoft.Maui.Controls.Compatibility.Platform;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BybitTrader
{
    public partial class MainPage : ContentPage
    {
        private bool isPulsating = false;
        private UpdateManager _updateManager;
        private TradingViewChart _tradingViewChart;


        public MainPage()
        {
            InitializeComponent();
            _updateManager = new UpdateManager();


            // tradingview integration
            LoadTradingViewChart();


            // Frissítés elérhetőségének ellenőrzése indításkor
            _updateManager.UpdateAvailable += OnUpdateAvailable;
            _updateManager.UpdateFailed += OnUpdateFailed;

            LoadSavedCredentials();
            
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

        private async void btnUpdateClick(Object sender, EventArgs e)
        {
            if (_updateManager.IsUpdateAvailable)
            {
                await _updateManager.DownloadAndUpdate();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            await _updateManager.CheckForUpdates();
        }

        private void OnUpdateAvailable(string latestVersion)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateButton.Text = $"Update to v{latestVersion}";
                UpdateButton.IsEnabled = true;
                UpdateButton.BackgroundColor = Color.FromArgb("#6b005de");
            });
        }

        private void OnUpdateFailed(string errorMessage)
        {
            Debug.WriteLine($"❌ Update check failed: {errorMessage}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateButton.Text = "No updates available";
                UpdateButton.IsEnabled = false;
                UpdateButton.BackgroundColor = Colors.Gray;
            });
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
            ConnectionStatusIndicator.ScaleTo(1.0, 500, Easing.Linear);
        }

        private void LoadTradingViewChart()
        {
            if (TradingViewWebView != null)
            {
                Debug.WriteLine("Tradingview inicializálása");
                _tradingViewChart = new TradingViewChart(TradingViewWebView);
                _tradingViewChart.LoadChart();
            }
        }

    }
                    
}

