using EmulatorPress.Models;
using SciChart.Charting.Common.Helpers;
using SciChart.Data.Model;
using System.Windows.Input;

namespace EmulatorPress.ViewModels
{
    internal class SettingPressViewModel : BindableObject
    {
        private readonly SettingPress setting = new();

        #region Значение min давления
        private double minValue;
        public double MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value; //(value>=maxValue) ? maxValue : value;
                OnPropertyChanged(nameof(MinValue));
                //OnPropertyChanged(nameof(MaxValue));
            }
        }
        #endregion
        #region Значение max давления
        private double maxValue;
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = (value >= minValue) ? value : minValue;
                OnPropertyChanged(nameof(MaxValue));
            }
        }
        #endregion
        #region Тип сигнала
        SignalType signalType = SignalType.Constant;

        public SignalType SignalType
        {
            get { return signalType; }
            set
            {
                if (signalType == value)
                    return;

                signalType = value;
                OnPropertyChanged(nameof(SignalType));
                OnPropertyChanged(nameof(IsConstant));
                OnPropertyChanged(nameof(IsRandoms));
                OnPropertyChanged(nameof(IsStep));
                OnPropertyChanged(nameof(MinValue));
                OnPropertyChanged(nameof(MaxValue));
            }
        }
        public bool IsConstant
        {
            get { return SignalType == SignalType.Constant; }
            set 
            { 
                SignalType = value ? SignalType.Constant : SignalType;
            }
        }

        public bool IsRandoms
        {
            get { return SignalType == SignalType.Randoms; }
            set { SignalType = value ? SignalType.Randoms : SignalType; }
        }

        public bool IsStep
        {
            get { return SignalType == SignalType.Step; }
            set 
            { 
                SignalType = value ? SignalType.Step : SignalType;
            }
        }

        #endregion

        // Команды
        #region Сохранить настройки
        private ActionCommand saveSetting;
        public ICommand SaveSetting
        {
            get
            {
                saveSetting ??= new ActionCommand(PerformSaveSettings);
                return saveSetting;
            }
        }
        private void PerformSaveSettings()
        {
            setting.MinValue = minValue;
            setting.MaxValue = maxValue;
            setting.Type = signalType;
            setting.SaveSettings();
        }
        #endregion

        public SettingPressViewModel()
        {
            setting.LoadSettings();
            minValue = setting.MinValue;
            maxValue = setting.MaxValue;
            signalType = setting.Type;
        }
    }
}
