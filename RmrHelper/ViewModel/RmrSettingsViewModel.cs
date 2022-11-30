using RmrHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


		public event PropertyChangedEventHandler SliderAdded;
		public event PropertyChangedEventHandler SliderRemoved;
		public virtual void OnSliderAdded(string sliderName)
		{
			SliderAdded?.Invoke(this, new PropertyChangedEventArgs(sliderName));
		}
		public virtual void OnSliderRemoved(string sliderName)
		{
			SliderRemoved?.Invoke(this, new PropertyChangedEventArgs(sliderName));
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

		public List<Tuple<int, string>> OverrideUnequipActionList { get; set; } = new List<Tuple<int, string>>
		{
			new Tuple<int, string>(0, "No Override"),
			new Tuple<int, string>(1, "Unequip"),
			new Tuple<int, string>(2, "Drop"),
			new Tuple<int, string>(3, "Destroy")
		};
		private Tuple<int, string> _overrideUnequipAction;
		public Tuple<int,string> OverrideUnequipAction
        {
			get { return _overrideUnequipAction; }
            set
            {
				if (_overrideUnequipAction != value)
                {
					_overrideUnequipAction = value;
					OnPropertyChanged(nameof(OverrideUnequipAction));
                }
            }
        }

		private bool _overrideUnequipDropChance;
		public bool OverrideUnequipDropChance
        {
            get { return _overrideUnequipDropChance;}
            set
            {
				if (_overrideUnequipDropChance != value)
                {
					_overrideUnequipDropChance = value;
					OnPropertyChanged(nameof(OverrideUnequipDropChance));
                }
            }
        }

		private int _overrideUnequipDropChanceValue;
		public int OverrideUnequipDropChanceValue
        {
            get { return _overrideUnequipDropChanceValue;}
            set
            {
				if (_overrideUnequipDropChanceValue != value)
                {
					_overrideUnequipDropChanceValue = value;
					OnPropertyChanged(nameof(OverrideUnequipDropChanceValue));
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
			sliderSet.SliderAdded += SliderSet_SliderAdded;
			sliderSet.SliderRemoved += SliderSet_SliderRemoved;
		}

		private void SliderSet_SliderRemoved(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnSliderRemoved(e.PropertyName);
		}

		private void SliderSet_SliderAdded(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnSliderAdded(e.PropertyName);
		}

		private void SliderSet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnSettingsChanged();
		}
	}
}
