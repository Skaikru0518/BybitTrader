namespace BybitTrader.Pages;

public partial class CalculatorsPage : ContentPage
{
    public CalculatorsPage()
    {
        InitializeComponent();
    }

    private void ShowEntryCalculator(object sender, EventArgs e)
    {
        EntryCalculatorGrid.IsVisible = true;
        TPSLCalculatorGrid.IsVisible = false;
        OrderByValueCalculatorGrid.IsVisible = false;
    }

    private void ShowTPSLCalculator(object sender, EventArgs e)
    {
        EntryCalculatorGrid.IsVisible = false;
        TPSLCalculatorGrid.IsVisible = true;
        OrderByValueCalculatorGrid.IsVisible = false;
    }

    private void ShowOrderByValueCalculator(object sender, EventArgs e)
    {
        EntryCalculatorGrid.IsVisible = false;
        TPSLCalculatorGrid.IsVisible = false;
        OrderByValueCalculatorGrid.IsVisible = true;
    }

    private void OnCalculateEntry(object sender, EventArgs e)
    {
        // Parse input values
        bool isCurrentPositionValid = float.TryParse(CurrentPositionSizeEntry.Text, out float currentPositionSize);
        bool isFistEntryValid = float.TryParse(FirstEntryPriceEntry.Text, out float firstEntryPrice);
        bool isNewEntryValid = float.TryParse(NewEntryPriceEntry.Text, out float newEntryPrice);
        bool isNewPositionValid = float.TryParse(NewPositionSizeEntry.Text, out float newPositionSize);

        if (isCurrentPositionValid && isFistEntryValid && isNewPositionValid && isNewPositionValid)
        {
            // Calculate the new average entry price
            float a = firstEntryPrice * currentPositionSize;
            float b = newEntryPrice * newPositionSize;
            float c = newPositionSize + currentPositionSize;

            if (c <= 0)
            {
                DisplayAlert("Calculation Error", "Total position size cannot be <=0", "OK"); return;
            }

            double newAverageEntryPrice = Math.Round((a + b) / c, 3);

            // Update the label
            NewAverageEntryPriceLabel.Text = newAverageEntryPrice.ToString();
        }
        else
        {
            DisplayAlert("Input error", "Please enter valid numeric values", "OK");
        }
    }

    private void OnCalculateTPSL(object sender, EventArgs e)
    {
        try
        {
            // Parse input values
            bool isEntryPriceValid = decimal.TryParse(EntryPriceTPSL.Text, out decimal entryPrice);
            bool isLeverageValid = decimal.TryParse(Leverage.Text, out decimal leverage);
            bool isRiskRewardValid = decimal.TryParse(RiskRewardRatio.Text, out decimal rrRatio);
            bool isInvestedAmountValid = decimal.TryParse(InvestedAmount.Text, out decimal amountInvested);

            if (!isEntryPriceValid || !isLeverageValid || !isRiskRewardValid || !isInvestedAmountValid)
            {
                DisplayAlert("Input error", "Please enter valid numeric values", "OK"); return;
            }

            // Parse TP% and SL% handling optional input
            bool isTPValid = decimal.TryParse(TakeProfitPercent.Text, out decimal tpPercentage);
            bool isSLValid = decimal.TryParse(StopLossPercent.Text, out decimal slPercentage);

            // Calculate the number of coins purchased
            decimal coinQuantity = amountInvested / entryPrice;

            // If TP% is missing calculate from SL% and RR ratio
            if (!isTPValid && isSLValid)
            {
                tpPercentage = slPercentage * rrRatio;
                TakeProfitPercent.Text = tpPercentage.ToString("F2"); // auto fill
            }

            // If SL% is missing calculate from SL% and RR ratio
            if (!isSLValid && isTPValid)
            {
                slPercentage = tpPercentage / rrRatio;
                StopLossPercent.Text = slPercentage.ToString("F2"); // auto fill
            }

            if (!isSLValid || !isTPValid)
            {
                DisplayAlert("Input error", "Either TP% or SL% must be provided. The other can be auto calculated", "OK");
                return;
            }

            // SL Calculation
            decimal stopLossPrice = entryPrice - (entryPrice * (slPercentage / 100));
            decimal stopLossLoss = (entryPrice - stopLossPrice) * coinQuantity * leverage;

            // TP Calculation
            decimal takeProfitPrice = entryPrice + (entryPrice * (tpPercentage / 100));
            decimal takeProfitGain = (takeProfitPrice - entryPrice) * coinQuantity * leverage;

            // Calculate total pos size with leverage
            decimal effectivePositionSize = coinQuantity * entryPrice * leverage;

            // Update UI labels
            SLPriceEntry.Text = $"{stopLossPrice:F3}";
            TPPriceEntry.Text = $"{takeProfitPrice:F3}";
            CoinsPurchasedLabel.Text = $"Coins Purchased: {coinQuantity:F3}";
            PositionSizeLabel.Text = $"Position Size: {effectivePositionSize:F3}";
            TPMaxProfitLabel.Text = $"TP Profit: {takeProfitGain:F3} USDT";
            SLMaxLossLabel.Text = $"SL Loss: {stopLossLoss:F3} USDT";


        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "An unexpected error occured, please try again.", "OK");
        }
    }

    private void OnCalculateOrderByValue(object sender, EventArgs e)
    {
        try
        {
            // Parse user inputs
            bool isEntryPriceValid = decimal.TryParse(EntryPriceOrderValue.Text, out decimal entryPrice);
            bool isMaxLossValid = decimal.TryParse(MaxLossOrderValue.Text, out decimal maxLoss);
            bool isLeverageValid = decimal.TryParse(LeverageOrderValue.Text, out decimal leverage);
            bool isSLPriceValid = decimal.TryParse(SLPriceOrderValue.Text, out decimal stopLossPrice);

            if (!isEntryPriceValid || !isMaxLossValid || !isLeverageValid || !isSLPriceValid)
            {
                DisplayAlert("Input Error", "Please enter valid numeric values.", "OK");
                return;
            }

            // Ensure SL price is not equal to entry price to prevent division by zero
            decimal perUnitLoss = entryPrice - stopLossPrice;
            if (perUnitLoss == 0)
            {
                DisplayAlert("Calculation Error", "Stop-loss price cannot be equal to the entry price.", "OK");
                return;
            }

            // Calculate position size (units/coin)
            decimal positionSize = maxLoss / perUnitLoss;

            // Calculate order value (total cost of position)
            decimal orderValue = Math.Round(positionSize * entryPrice, 3);

            // Calculate required margin
            decimal marginRequired = Math.Round(orderValue / leverage, 3);

            // Update UI
            OrderByValueValue.Text = orderValue.ToString();
            MarginRequiredValue.Text = marginRequired.ToString();

        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "An unexpected error occured, please try again.", "OK");
        }
    } 
}
