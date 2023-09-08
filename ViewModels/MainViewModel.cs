using SciChart.Charting.Common.Helpers;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using EmulatorPress.Services;
using EmulatorPress.Models;


namespace EmulatorPress.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly DummyDataProvider dummyDataProvider = new();
        private XyDataSeries<TimeSpan, double> lineData = new() { SeriesName = "Давление испытания" };
        private IDialogService dialogService = new DialogService();
        private SettingPress setting = new();

        #region renderableSeries
        private ObservableCollection<IRenderableSeriesViewModel> renderableSeries;
        public ObservableCollection<IRenderableSeriesViewModel> RenderableSeries
        {
            get { return renderableSeries; }
            set
            {
                renderableSeries = value;
                OnPropertyChanged(nameof(RenderableSeries));
            }
        }
        #endregion
        #region  Масштабирование, панорамирование
        private bool enableZoom = true;
        public bool EnableZoom
        {
            get { return enableZoom; }
            set
            {
                if (enableZoom != value)
                {
                    enableZoom = value;
                    OnPropertyChanged(nameof(EnableZoom));
                    if (enableZoom) EnablePan = false;
                }
            }
        }
        private bool enablePan;
        public bool EnablePan
        {
            get { return enablePan; }
            set
            {
                if (enablePan != value)
                {
                    enablePan = value;
                    OnPropertyChanged(nameof(EnablePan));
                    if (enablePan) EnableZoom = false;
                }
            }
        }
        #endregion
        #region Блокировка кнопки Start
        private bool isStartEnabled = true;
        public bool IsStartEnabled
        {
            get { return isStartEnabled; }
            set
            {
                isStartEnabled = value;
                OnPropertyChanged(nameof(IsStartEnabled));
            }
        }
        #endregion
        #region Блокировка кнопки Stop
        private bool isStopEnabled = false;
        public bool IsStopEnabled
        {
            get { return isStopEnabled; }
            set
            {
                isStopEnabled = value;
                OnPropertyChanged(nameof(IsStopEnabled));
            }
        }
        #endregion
        #region Блокировка кнопки Выдержка
        private bool isExcerptEnable;

        public bool IsExcerptEnable
        {
            get { return isExcerptEnable; }
            set
            {
                isExcerptEnable = value;
                OnPropertyChanged(nameof(IsExcerptEnable));
            }
        }
        #endregion
        #region Текущие настройки
        public string SettingsStatus
        {
            get
            {
                return setting.Type switch
                {
                    SignalType.Constant => "Статическое давление " + setting.MinValue + " усл.ед.",
                    SignalType.Randoms => "Случайное давление от " + setting.MinValue + " до " + setting.MaxValue + " усл.ед.",
                    SignalType.Step => "Изменение давления на " + setting.MinValue + " усл.ед.",
                    _ => "",
                };
            }
        }
        #endregion        
        #region Выдержка
        private ObservableCollection<IAnnotationViewModel> annotations = new ObservableCollection<IAnnotationViewModel>();
        public ObservableCollection<IAnnotationViewModel> Annotations { get { return annotations; } }
        #endregion

        // Команды
        #region Старт
        private ActionCommand onEmulation;
        public ICommand OnEmulation
        {
            get
            {
                onEmulation ??= new ActionCommand(PerformOnEmulation);
                return onEmulation;
            }
        }
        private void PerformOnEmulation()
        {
            UpdateStatus();
            dummyDataProvider.signalType = setting.Type;
            dummyDataProvider.Value = setting.MinValue;
            dummyDataProvider.MaxValue = setting.MaxValue;
            dummyDataProvider.Timer.Start();
            isStopEnabled = true;
            OnPropertyChanged(nameof(IsStopEnabled));
            isStartEnabled = false;
            OnPropertyChanged(nameof(IsStartEnabled));
            isExcerptEnable = true;
            OnPropertyChanged(nameof(IsExcerptEnable));
            
        }
        #endregion
        #region Стоп
        private ActionCommand offEmulation;
        public ICommand OffEmulation
        {
            get
            {
                offEmulation ??= new ActionCommand(PerformOffEmulation);
                return offEmulation;
            }
        }
        private void PerformOffEmulation()
        {
            dummyDataProvider.Timer.Stop();
            isStopEnabled = false;
            OnPropertyChanged(nameof(IsStopEnabled));
            isStartEnabled = true;
            OnPropertyChanged(nameof(IsStartEnabled));
            isExcerptEnable = false;
            OnPropertyChanged(nameof(IsExcerptEnable));
        }
        #endregion
        #region Очистка графика
        private ActionCommand clearChart;
        public ICommand ClearChart
        {
            get
            {
                clearChart ??= new ActionCommand(PerformClearChart);
                return clearChart;
            }
        }
        private void PerformClearChart()
        {
            lineData.Clear();
            dummyDataProvider.IntervalCounter = 1;
            annotations.Clear();
        }
        #endregion
        #region Открыть настройки
        private ActionCommand openSettings;

        public ICommand OpenSettings
        {
            get
            {
                openSettings ??= new ActionCommand(PerformOpenSettings);
                return openSettings;
            }
        }

        private void PerformOpenSettings()
        {
            PerformOffEmulation();
            dialogService.ShowDialog<SettingPressViewModel>(result =>
            {
                UpdateStatus();
            });
        }

        private void UpdateStatus()
        {
            setting.LoadSettings();
            OnPropertyChanged(nameof(SettingsStatus));
        }
        #endregion
        #region Установить выдержку
        private ActionCommand startExcerpt;

        public ICommand StartExcerpt
        {
            get
            {
                startExcerpt ??= new ActionCommand(PerformStartExcerpt);
                return startExcerpt;
            }
        }

        private void PerformStartExcerpt()
        {
            Annotations.Add(new VerticalLineAnnotationViewModel()
            {
                X1 = lineData.XValues.Last(),
                StrokeThickness = 2,
                ShowLabel = true,
            }) ;
            Annotations.Add(new AxisMarkerAnnotationViewModel()
            {
                Y1 = lineData.YValues.Last(),
            });
        }
        #endregion

        public MainViewModel()
        {
            setting.LoadSettings(); // выгружаем настройки из БД
            renderableSeries = new ObservableCollection<IRenderableSeriesViewModel>() { };
            RenderableSeries.Add(new LineRenderableSeriesViewModel()
            {
                StrokeThickness = 2, // толщина линии
                Stroke = Colors.LightBlue, // цвет линии
                DataSeries = lineData,
                StyleKey = "LineSeriesStyle"
            });
            lineData.Append(TimeSpan.FromSeconds(0), setting.MinValue); //начальная точка
            dummyDataProvider.SubscribeUpdates((newValues) =>
            {
                lineData.Append(newValues.XValues, newValues.YValues);
                lineData.InvalidateParentSurface(RangeMode.ZoomToFit);// Масштабирование диаграммы по размеру
                
            });

        }

    }
}
