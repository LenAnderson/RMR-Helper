using ModernWpf.Controls;
using RmrHelper.Helpers;
using RmrHelper.Model;
using RmrHelper.Service;
using RmrHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RmrHelper.ViewModel
{
	public class SliderSetViewModel : ObservableObject
	{
		#region events
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




		public string Title { get; set; } = "Slider Set #";

		private ContentDialog _addSliderDialog;
		private AddSliderViewModel _addSliderDialogContext;


		//ObservableCollection<string> _sliderNames = new ObservableCollection<string>();
		public string SliderNames
		{
			get { return string.Join("|", SliderNameList); }
			set
			{
				if (SliderNames != value)
				{
					SliderNameList.Clear();
					foreach (var sliderName in value.Split('|').ToList().Where(it=>!string.IsNullOrWhiteSpace(it)))
					{
						SliderNameList.Add(sliderName);
					}
					OnPropertyChanged(nameof(SliderNames));
				}
			}
		}
		public ObservableCollection<string> SliderNameList { get; set; } = new ObservableCollection<string>();// { get { return _sliderNames; } }

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

		private int _targetSizeIncrease;
		public int TargetSizeIncrease
		{
			get { return _targetSizeIncrease; }
			set
			{
				if (_targetSizeIncrease != value)
				{
					_targetSizeIncrease = value;
					OnPropertyChanged(nameof(TargetSizeIncrease));
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

		private string _armorSlotsToUnequip;
		public string ArmorSlotsToUnequip
		{
			get { return _armorSlotsToUnequip; }
			set
			{
				if (_armorSlotsToUnequip != value)
				{
					_armorSlotsToUnequip = value;
					OnPropertyChanged(nameof(ArmorSlotsToUnequip));
				}
			}
		}

		private int _unequipThreshold;
		public int UnequipThreshold
		{
			get { return _unequipThreshold; }
			set
			{
				if (_unequipThreshold != value)
				{
					_unequipThreshold = value;
					OnPropertyChanged(nameof(UnequipThreshold));
				}
			}
		}

		public List<Tuple<int, string>> UnequipActionList { get; set; } = new List<Tuple<int, string>>
		{
			new Tuple<int, string>(0, "Unequip"),
			new Tuple<int, string>(1, "Drop"),
			new Tuple<int, string>(2, "Destroy")
		};
		private Tuple<int, string> _unequipAction;
		public Tuple<int,string> UnequipAction
        {
			get { return _unequipAction; }
			set
            {
				if (_unequipAction != value)
                {
					_unequipAction = value;
					OnPropertyChanged(nameof(UnequipAction));
                }
            }
        }

		private int _unequipDropChance;
		public int UnequipDropChance
        {
            get { return _unequipDropChance; }
            set
            {
				if (_unequipDropChance != value)
                {
					_unequipDropChance = value;
					OnPropertyChanged(nameof(UnequipDropChance));
                }
            }
        }

		public List<Tuple<int, string>> ApplyToList { get; set; } = new List<Tuple<int, string>>
		{
			new Tuple<int, string>(0, "Player only"),
			new Tuple<int, string>(1, "Companion only"),
			new Tuple<int, string>(2, "Player & Companion")
		};
		private Tuple<int, string> _applyTo;
		public Tuple<int, string> ApplyTo
		{
			get { return _applyTo; }
			set
			{
				if (_applyTo != value)
				{
					_applyTo = value;
					OnPropertyChanged(nameof(ApplyTo));
				}
			}
		}

		public List<Tuple<int, string>> SexList { get; set; } = new List<Tuple<int, string>>
		{
			new Tuple<int, string>(0, "All"),
			new Tuple<int, string>(1, "Female"),
			new Tuple<int, string>(2, "Male")
		};
		private Tuple<int, string> _sex;
		public Tuple<int, string> Sex
        {
			get { return _sex; }
            set
            {
				if (_sex != value)
                {
					_sex = value;
					OnPropertyChanged(nameof(Sex));
                }
            }
        }

		private bool _onlyDoctorCanReset;
		public bool OnlyDoctorCanReset
		{
			get { return _onlyDoctorCanReset; }
			set
			{
				if (_onlyDoctorCanReset != value)
				{
					_onlyDoctorCanReset = value;
					OnPropertyChanged(nameof(OnlyDoctorCanReset));
				}
			}
		}

		private bool _isAdditive;
		public bool IsAdditive
		{
			get { return _isAdditive;}
			set
			{
				if (_isAdditive != value)
				{
					_isAdditive = value;
					OnPropertyChanged(nameof(IsAdditive));
				}
			}
		}

		private bool _hasAdditiveLimit;
		public bool HasAdditiveLimit
		{
			get { return _hasAdditiveLimit;}
			set
			{
				if (_hasAdditiveLimit != value)
				{
					_hasAdditiveLimit = value;
					OnPropertyChanged(nameof(HasAdditiveLimit));
				}
			}
		}

		private int _additiveLimit;
		public int AdditiveLimit
		{
			get { return _additiveLimit;}
			set
			{
				if (_additiveLimit != value)
				{
					_additiveLimit = value;
					OnPropertyChanged(nameof(AdditiveLimit));
				}
			}
		}




		public SliderSetViewModel()
		{
			_addSliderDialog = new AddSliderView();
			_addSliderDialogContext = _addSliderDialog.DataContext as AddSliderViewModel;
		}




		public void UpdateCategoryList(Dictionary<string, List<SliderModel>> categoryList)
		{
			_addSliderDialogContext.UpdateCategoryList(categoryList);
		}




		public int GetMorph(int triggerValue, int additiveValue, bool? onlyDoctorCanReset=null, bool? isAdditive=null, bool? hasAdditiveLimit=null, int? additiveLimit=null)
		{
			float trigger = ((float)triggerValue) / 100.0f;
			if (InvertTriggerValue)
			{
				trigger = 1.0f - trigger;
			}
			float additive = ((float)additiveValue) / 100.0f;
			float limit = ((float)AdditiveLimit) / 100.0f;

			bool hasLimit = (hasAdditiveLimit.HasValue && hasAdditiveLimit.Value) || (!hasAdditiveLimit.HasValue && HasAdditiveLimit);
			bool doctor = (onlyDoctorCanReset.HasValue && onlyDoctorCanReset.Value) || (!onlyDoctorCanReset.HasValue && OnlyDoctorCanReset);
			bool isAdd = (isAdditive.HasValue && isAdditive.Value) || (!isAdditive.HasValue && IsAdditive);
			limit = additiveLimit.HasValue ? ((float)additiveLimit.Value)/100.0f : limit;

			if (hasLimit)
			{
				additive *= limit;
			}
			else
			{
				additive *= 6.0f;
			}

			float morph = 0.0f;
			if (trigger < _lowerThreshold / 100.0f)
			{
				morph = 0.0f;
			}
			else if (trigger > _upperThreshold / 100.0f)
			{
				morph = 1.0f;
			}
			else
			{
				morph = (trigger - _lowerThreshold / 100.0f) / (_upperThreshold / 100.0f - _lowerThreshold / 100.0f);
			}
			if (doctor && isAdd)
			{
				morph += additive;
			}

			// r = (V / (4/3π) )^1/3
			//var dblMorph = Math.Pow(morph / (4d / Math.PI), 1d / 3d);
			//morph = (float)dblMorph;

			return (int)(TargetSizeIncrease * morph);
		}




		#region Commands
		ICommand _removeSliderNameCommand;
		public ICommand RemoveSliderNameCommand
		{
			get
			{
				if (_removeSliderNameCommand == null)
				{
					_removeSliderNameCommand = new RelayCommand(p =>
					{
						var sliderName = p.ToString();
						SliderNameList.Remove(sliderName);
						OnSliderRemoved(sliderName);
					});
				}
				return _removeSliderNameCommand;
			}
		}

		ICommand _addSliderCommand;
		public ICommand AddSliderCommand
		{
			get
			{
				if (_addSliderCommand== null)
				{
					_addSliderCommand= new RelayCommand(async(p) =>
					{
						foreach (var category in _addSliderDialogContext.CategoryList)
						{
							foreach(var cb in category.Item2)
							{
								cb.IsChecked = SliderNameList.Contains(cb.Slider.Name);
							}
						}
						if (await _addSliderDialog.ShowAsync() == ContentDialogResult.Primary)
						{
							var newSliderNames = _addSliderDialogContext.CategoryList.SelectMany(category => category.Item2.Where(it => it.IsChecked).Select(it => it.Slider.Name)).ToList();
							foreach (var sliderName in newSliderNames)
							{
								if (!SliderNameList.Contains(sliderName))
								{
									SliderNameList.Add(sliderName);
									OnSliderAdded(sliderName);
								}
							}
							foreach (var sliderName in SliderNameList.ToList())
							{
								if (!newSliderNames.Contains(sliderName))
								{
									SliderNameList.Remove(sliderName);
									OnSliderRemoved(sliderName);
								}
							}
							OnPropertyChanged(nameof(SliderNames));
						}
					});
				}
				return _addSliderCommand;
			}
		}
		#endregion
	}
}
