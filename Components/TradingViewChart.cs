using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace BybitTrader.Components
{
    public class TradingViewChart
    {
        private const string TradingViewBaseUrl = "https://www.tradingview.com/chart/";
        private WebView _webView;

        public TradingViewChart(WebView webView)
        {
            _webView = webView;
            _webView.Navigated += OnWebViewNavigated;
        }

        public void LoadChart()
        {
            Debug.WriteLine("📊 TradingView chart betöltése...");
            _webView.Source = TradingViewBaseUrl;
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Url.Contains("signin") || e.Url.Contains("login"))
            {
                Debug.WriteLine("🔑 A felhasználónak be kell jelentkeznie a TradingView fiókjába.");
            }
            else
            {
                Debug.WriteLine($"✅ TradingView Chart sikeresen betöltve: {e.Url}");
            }
        }
    }
}
