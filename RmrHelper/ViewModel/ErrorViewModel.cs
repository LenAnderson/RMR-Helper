using RmrHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.ViewModel
{
    public class ErrorViewModel : ObservableObject
    {
        public ObservableCollection<string> Errors { get; set; } = new ObservableCollection<string>();
    }
}
