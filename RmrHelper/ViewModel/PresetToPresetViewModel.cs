using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RmrHelper.Helpers;
using RmrHelper.Model;

namespace RmrHelper.ViewModel
{
	public class PresetToPresetViewModel : ObservableObject
	{
		public ObservableCollection<PresetModel> PresetList { get; set; } = new ObservableCollection<PresetModel>();
		private PresetModel _selectedPreset;
		public PresetModel SelectedPreset
		{
			get { return _selectedPreset; }
			set
			{
				if (_selectedPreset != value)
				{
					_selectedPreset = value;
					OnPropertyChanged(nameof(SelectedPreset));
				}
			}
		}

		public ObservableCollection<string> TriggerNameList { get; set; } = new ObservableCollection<string>();
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

		private bool _invertTriggerValue;
		public bool InvertTriggerValue
		{
			get { return _invertTriggerValue; }
			set
			{
				if (_invertTriggerValue != value)
				{
					_invertTriggerValue = value;
					OnPropertyChanged(nameof(InvertTriggerValue));
				}
			}
		}

		public List<Tuple<int, string>> UpdateTypeList { get; set; } = new List<Tuple<int, string>>
		{
			new Tuple<int, string>(0, "Immediate"),
			new Tuple<int, string>(1, "Periodic"),
			new Tuple<int, string>(2, "On Sleep")
		};
		private Tuple<int, string> _updateType;
		public Tuple<int, string> UpdateType
		{
			get { return _updateType; }
			set
			{
				if (_updateType != value)
				{
					_updateType = value;
					OnPropertyChanged(nameof(UpdateType));
				}
			}
		}

		private int _lowerThreshold;
		public int LowerThreshold
		{
			get { return _lowerThreshold; }
			set
			{
				if (_lowerThreshold != value)
				{
					_lowerThreshold = value;
					OnPropertyChanged(nameof(LowerThreshold));
				}
			}
		}

		private int _upperThreshold;
		public int UpperThreshold
		{
			get { return (int)(_upperThreshold); }
			set
			{
				if (_upperThreshold != value)
				{
					_upperThreshold = value;
					OnPropertyChanged(nameof(UpperThreshold));
				}
			}
		}
	}
}
