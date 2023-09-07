using SciChart.Charting.Visuals.Axes.LabelProviders;
using System;


namespace EmulatorPress.ViewModels
{
    // Чтобы создать LabelProvider для NumericAxis или Log Axis, наследовать NumericLabelProvider 
    // .. для DateTimeAxis наследовать DateTimeLabelProvider 
    // .. для TimeSpanAxis наследовать TimeSpanLabelProvider 
    // .. для CategoryDateTimeAxis наследовать TradeChartAxisLabelProvider 
    public class CustomNumericLabelProvider : LabelProviderBase
    {
        public override string FormatLabel(IComparable dataValue)
        {
            var yValue = (double)dataValue; 
            if (yValue > 1000)
                return yValue.ToString("e2");

            return yValue.ToString();
        }


        public override string FormatCursorLabel(IComparable dataValue)
        {
            var yValue = (double)dataValue;
            if (yValue > 1000000)
                return yValue.ToString("e5");

            return yValue.ToString();
        }
    }
}
