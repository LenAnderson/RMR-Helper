using ModernWpf.Controls;
using RmrHelper.Helpers;
using RmrHelper.Model;
using RmrHelper.Service;
using RmrHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RmrHelper.ViewModel
{
	public class SliderSetViewModel : ObservableObject
	{
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

		public List<Tuple<int, string>> UpdateTypeList { get; set; } = new List<Tuple<int, string>>
		{
			new Tuple<int, string>(0, "Periodic"),
			new Tuple<int, string>(1, "On Sleep"),
			new Tuple<int, string>(2, "Immediate")
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




		public int GetMorph(int triggerValue)
		{
			float trigger = ((float)triggerValue) / 100.0f;
			float morph = 0.0f;
			if (trigger < _lowerThreshold)
			{
				morph = 0.0f;
			}
			else if (trigger > _upperThreshold)
			{
				morph = 1.0f;
			}
			else
			{
				morph = (trigger - _lowerThreshold) / (_upperThreshold - _lowerThreshold);
			}
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
						OnPropertyChanged(nameof(SliderNames));
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
							foreach (var category in _addSliderDialogContext.CategoryList)
							{
								foreach (var sliderName in category.Item2.Where(it => it.IsChecked).Select(it => it.Slider.Name))
								{
									if (!SliderNameList.Contains(sliderName))
									{
										SliderNameList.Add(sliderName);
									}
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
