using RmrHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.ViewModel
{
	public class TriggerViewModel : ObservableObject
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}


		private float _value;
		public int Value
		{
			get { return (int)(_value * 100); }
			set
			{
				if (_value != (float)value / 100.0f)
				{
					_value = (float)value / 100.0f;
					OnPropertyChanged(nameof(Value));
				}
			}
		}


		private float _additiveValue;
		public int AdditiveValue
		{
			get { return (int)(_additiveValue * 100); }
			set
			{
				if (_additiveValue != (float)value / 100.0f)
				{
					_additiveValue = (float)value / 100.0f;
					OnPropertyChanged(nameof(AdditiveValue));
				}
			}
		}
	}
}
