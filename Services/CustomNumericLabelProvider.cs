using SciChart.Charting.Visuals.Axes.LabelProviders;
using System;


namespace EmulatorPress.ViewModels
{
    public class CustomNumericLabelProvider : LabelProviderBase
    {
        //класс позволяет переопределить формат отображения значений на оси
        public override string FormatLabel(IComparable dataValue)
        {
            var yValue = (double)dataValue; 
            if (yValue > 10000) // при значении более 10000 единиц, оно будет выводиться в формате 1,00е+004
                return yValue.ToString("e2");
            if (yValue < 1)
                return Math.Round(yValue, 2).ToString();
            return yValue.ToString();
        }

        public override string FormatCursorLabel(IComparable dataValue)
        {
            var yValue = (double)dataValue;
            if (yValue > 1000000) // показания курсора будут меняться при более высоких значениях
                return yValue.ToString("e6");

            return yValue.ToString();
        }
    }
}
