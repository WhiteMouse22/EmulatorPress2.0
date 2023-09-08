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
        private double lastPressure; 
        private double nextPressure;
        private int interval = 100; // интервал времени для каждой точки в миллисекундах
        public int IntervalCounter = 0;
        public DispatcherTimer Timer = new(DispatcherPriority.Render);//Render - Операции обрабатываются с таким же приоритетом, как и отрисовка.
        public double Value;
        public double MaxValue;
        public SignalType signalType;

        public void SubscribeUpdates(Action<XyValues> onDataUpdated)
        {
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (designMode) return;

            Timer.Interval = TimeSpan.FromMilliseconds(interval);
            Timer.Tick += (s, e) =>
            {
                var xyValues = GenerateRandomWalk();
                onDataUpdated(xyValues);
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
                var MinValue = Value;
                var rangeValue = MaxValue - MinValue;
                lastPressure = MinValue;
                if (MaxValue - MinValue < 5)
                    step = random.NextDouble() * random.Next(-10, 10) / 10;
                else
                    step = random.NextDouble() * random.Next(-(int)Math.Abs(rangeValue/2), (int)Math.Abs(rangeValue/2));
                lastPressure = nextPressure + step;
                if (lastPressure > MaxValue) 
                    lastPressure = MaxValue;
                else if (lastPressure < MinValue)  
                    lastPressure = MinValue; 
                nextPressure = lastPressure;
            }
            else // режимы роста и падения
            {
                step = Value / 10;
                nextPressure = lastPressure + step;
                lastPressure = nextPressure;
            }

            IntervalCounter += interval;
            values.XValues.Add(TimeSpan.FromMilliseconds(IntervalCounter));  // устанавливаем новую координату Х
            values.YValues.Add(Math.Round(nextPressure, 2)); // устанавливаем новую координату Y

            return values;
        }
    }
}
