using RmrHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.ViewModel
{
	public class RmrSettingsViewModel : ObservableObject
	{
		#region custom events
		public event EventHandler SettingsChanged;
		public virtual void OnSettingsChanged()
		{
			SettingsChanged?.Invoke(this, EventArgs.Empty);
		}
		#endregion




		private int _overrideOnlyDoctorCanReset;
		public int OverrideOnlyDoctorCanReset
		{
			get { return _overrideOnlyDoctorCanReset; }
			set
			{
				if (_overrideOnlyDoctorCanReset != value)
				{
					_overrideOnlyDoctorCanReset = value;
					OnPropertyChanged(nameof(OverrideOnlyDoctorCanReset));
					OnSettingsChanged();
				}
			}
		}

		private int _overrideIsAdditive { get; set; }
		public int OverrideIsAdditive
		{
			get { return _overrideIsAdditive; }
			set
			{
				if (_overrideIsAdditive != value)
				{
					_overrideIsAdditive = value;
					OnPropertyChanged(nameof(OverrideIsAdditive));
					OnSettingsChanged();
				}
			}
		}

		private int _overrideHasAdditiveLimit { get; set; }
		public int OverrideHasAdditiveLimit
		{
			get { return _overrideHasAdditiveLimit; }
			set
			{
				if (_overrideHasAdditiveLimit != value)
				{
					_overrideHasAdditiveLimit = value;
					OnPropertyChanged(nameof(OverrideHasAdditiveLimit));
					OnSettingsChanged();
				}
			}
		}

		private int _overrideAdditiveLimit { get; set; }
		public int OverrideAdditiveLimit
		{
			get { return _overrideAdditiveLimit; }
			set
			{
				if (_overrideAdditiveLimit != value)
				{
					_overrideAdditiveLimit = value;
					OnPropertyChanged(nameof(OverrideAdditiveLimit));
					OnSettingsChanged();
				}
			}
		}

		int _numberOfSliderSets { get; set; }
		public int NumberOfSliderSets
		{
			get { return _numberOfSliderSets; }
			set
			{
				if (_numberOfSliderSets != value)
				{
					_numberOfSliderSets = value;
					OnPropertyChanged(nameof(NumberOfSliderSets));
					OnSettingsChanged();
				}
			}
		}

		public ObservableCollection<SliderSetViewModel> SliderSetList { get; set; } = new ObservableCollection<SliderSetViewModel>();




		public Dictionary<int, string> OverrideBool { get; set; } = new Dictionary<int, string>
		{
			{ 0, "No Override" },
			{ 1, "Enabled" },
			{ 2, "Disabled" }
		};




		public void AddSliderSet(SliderSetViewModel sliderSet)
		{
			this.SliderSetList.Add(sliderSet);
			sliderSet.PropertyChanged += SliderSet_PropertyChanged;
		}

		private void SliderSet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnSettingsChanged();
		}
	}
}
