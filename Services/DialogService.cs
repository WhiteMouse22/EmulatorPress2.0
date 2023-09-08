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
        public void ShowDialog<TViewModel>(Action<string> callback)
        {
            var type = mappings[typeof(TViewModel)];
            var dialog = new DialogWindow();

            EventHandler closeEventHandler = null;
            closeEventHandler = (s, e) =>
            {
                callback(dialog.DialogResult.ToString());
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;

            var content = Activator.CreateInstance(type);
            var vm = Activator.CreateInstance(typeof(TViewModel));
            (content as FrameworkElement).DataContext = vm;
            dialog.Content = content;
            dialog.ShowDialog();
        }

    }
}
