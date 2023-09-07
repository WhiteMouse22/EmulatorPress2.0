using EmulatorPress.Views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EmulatorPress.Services
{
    internal class DialogService : IDialogService
    {
        static Dictionary<Type, Type> mappings = new();

        public static void RegisterDialog<TViewModel, TView>()
        {
            mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public void ShowDialog(string name, Action<string> callback)
        {
            var type = Type.GetType($"EmulatorPress.Views.{name}");
            ShowDialogInternal(type, callback, null);
        }

        public void ShowDialog<TViewModel>(Action<string> callback)
        {
            var type = mappings[typeof(TViewModel)];
            ShowDialogInternal(type, callback, typeof(TViewModel));
        }

        private static void ShowDialogInternal(Type type, Action<string> callback, Type vmType)
        {
            var dialog = new DialogWindow();

            EventHandler closeEventHandler = null;
            closeEventHandler = (s, e) =>
            {
                callback(dialog.DialogResult.ToString());
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;

            var content = Activator.CreateInstance(type);

            if (vmType != null)
            {
                var vm = Activator.CreateInstance(vmType);
                (content as FrameworkElement).DataContext = vm;
            }

            dialog.Content = content;
            dialog.ShowDialog();
        }
    }
}
