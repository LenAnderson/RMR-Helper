using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RmrHelper.Helpers
{
	public class RelayCommand : ICommand
	{
		readonly Action<object> _execute;
		readonly Func<object, bool> _canExecute;




		public RelayCommand(Action<object> execute) : this(execute, null) { }
		public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;

			CommandManager.RequerySuggested += CommandManager_RequerySuggested;
		}




		private void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, e);
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}
