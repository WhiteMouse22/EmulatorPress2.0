

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using System;
using EmulatorPress.Models;


namespace EmulatorPress.Services
{
    public interface IDataProvider
    {
        void SubscribeUpdates(Action<XyValues> onDataUpdated);
    }
    public struct XyValues
    {
        public IList<TimeSpan> XValues;
        public IList<double> YValues;
    }

    public class DummyDataProvider : IDataProvider
    {
        private readonly Random random = new();
        private double lastPressure; // предыдущее значение давления
        private double nextPressure; // следующее значение давления
        private TimeSpan t = TimeSpan.FromSeconds(0); //координата Х
        private int interval = 100; // интервал времени миллисекунды

        public int X = 0;
        public DispatcherTimer Timer = new(DispatcherPriority.Render);//Render - Операции обрабатываются с таким же приоритетом, как и отрисовка.
        public double Value;
        public double MaxValue;
        public SignalType signalType;

        public void SubscribeUpdates(Action<XyValues> onDataUpdated)
        {
            // LicenseManager.UsageMode Возвращает объект LicenseUsageMode,
            // определяющий, когда можно использовать лицензированный объект для контекста CurrentContext.
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (designMode) return;

            Timer.Interval = TimeSpan.FromMilliseconds(interval); //шаг
            Timer.Tick += (s, e) =>
            {
                var xyValues = GenerateRandomWalk(); // генерируем новые координаты
                onDataUpdated(xyValues);            // отправляем их на визуализацию
            };
        }
        private XyValues GenerateRandomWalk()
        {
            XyValues values = new()
            {
                XValues = new List<TimeSpan>(),
                YValues = new List<double>(),
            };


            double step; // шаг изменения давления

            if (signalType == SignalType.Constant) // режим постоянного давления
            {
                nextPressure = Value;
            }
            else if (signalType == SignalType.Randoms) // режим случайного давления
            {
                lastPressure = Value;
                if (MaxValue - Value < 5)
                    step = random.NextDouble() * random.Next(-10, 10) / 10;
                else
                    step = random.NextDouble() * random.Next(-(int)Math.Abs(Value), (int)Math.Abs(Value));
                lastPressure = nextPressure + step;
                if (lastPressure > MaxValue) { lastPressure = MaxValue; }
                else if (lastPressure < Value) { lastPressure = Value; }
                nextPressure = lastPressure;
            }
            else // режимы роста и падения
            {
                step = Value / 10;
                nextPressure = (step > 0) ? lastPressure + step : lastPressure - Math.Abs(step);
                lastPressure = nextPressure;
            }

            X += interval;
            values.XValues.Add(TimeSpan.FromSeconds(0).Add(TimeSpan.FromMilliseconds(X)));  // устанавливаем новую координату Х
            values.YValues.Add(Math.Round(nextPressure, 2)); // устанавливаем новую координату Y

            return values;
        }
    }
}
