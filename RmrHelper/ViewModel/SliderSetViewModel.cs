using RmrHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.ViewModel
{
	public class SliderSetViewModel : ObservableObject
	{
		public string Title { get; set; } = "Slider Set #";


		List<string> _sliderNames = new List<string>();
		public string SliderNames
		{
			get { return string.Join("|", _sliderNames); }
			set
			{
				if (SliderNames != value)
				{
					_sliderNames = value.Split('|').ToList();
					OnPropertyChanged(nameof(SliderNames));
				}
			}
		}
		public List<string> SliderNameList { get { return _sliderNames; } }

		private string _triggerName;
		public string TriggerName
		{
			get { return _triggerName; }
			set
			{
				if (_triggerName != value)
				{
					_triggerName = value;
					OnPropertyChanged(nameof(TriggerName));
				}
			}
		}

		private float _targetSizeIncrease;
		public int TargetSizeIncrease
		{
			get { return (int)(100 * _targetSizeIncrease); }
			set
			{
				if (_targetSizeIncrease != (float)value / 100.0f)
				{
					_targetSizeIncrease = (float)value / 100.0f;
					OnPropertyChanged(nameof(TargetSizeIncrease));
				}
			}
		}

		private float _lowerThreshold;
		public int LowerThreshold
		{
			get { return (int)(100 * _lowerThreshold); }
			set
			{
				if (_lowerThreshold != (float)value / 100.0f)
				{
					_lowerThreshold = (float)value / 100.0f;
					OnPropertyChanged(nameof(LowerThreshold));
				}
			}
		}

		private float _upperThreshold;
		public int UpperThreshold
		{
			get { return (int)(100 * _upperThreshold); }
			set
			{
				if (_upperThreshold != (float)value / 100.0f)
				{
					_upperThreshold = (float)value / 100.0f;
					OnPropertyChanged(nameof(UpperThreshold));
				}
			}
		}




		public int GetMorph(int triggerValue)
		{
			float trigger = ((float)triggerValue) / 100.0f;
			float morph = 0.0f;
			if (trigger < _lowerThreshold)
			{
				morph = 0.0f;
			}
			else if (trigger > _upperThreshold && false)
			{
				morph = 1.0f;
			}
			else
			{
				morph = (trigger - _lowerThreshold) / (_upperThreshold - _lowerThreshold);
			}
			return (int)(TargetSizeIncrease * morph);
		}
	}
}
