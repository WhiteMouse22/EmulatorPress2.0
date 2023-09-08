using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorPress.Services
{
    internal interface IDialogService
    {
        void ShowDialog<ViewModel>(Action<string> callback);
    }
}
