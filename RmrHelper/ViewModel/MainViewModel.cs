using IniParser;
using Microsoft.Win32;
using ModernWpf.Controls;
using RmrHelper.Helpers;
using RmrHelper.Model;
using RmrHelper.Service;
using RmrHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;

namespace RmrHelper.ViewModel
{
	public class MainViewModel : ObservableObject
	{
		public string AppDir
		{
			get
			{
				#if DEBUG
				return @"E:\Games\Steam\steamapps\common\Fallout 4\Data\Tools\RmrHelper";
				#endif
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		private ContentDialog AddTriggerDialog;
		private AddTriggerViewModel AddTriggerDialogContext;


		public string RmrIniPath
		{
			get { return Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini")); }
		}

		public ObservableCollection<BodyModel> BodyList { get; set; } = new ObservableCollection<BodyModel>();
		private BodyModel _selectedBody;
		public BodyModel SelectedBody
		{
			get { return _selectedBody; }
			set
			{
				if (_selectedBody != value)
				{
					_selectedBody = value;
					BodySlide.Categories = SelectedBody.SliderCategories;
					foreach (var sliderSet in RmrSettings.SliderSetList)
					{
						sliderSet.UpdateCategoryList(SelectedBody.SliderCategories);
					}
					OnPropertyChanged(nameof(SelectedBody));
					OnPropertyChanged(nameof(SelectedBodySliderCount));
				}
			}
		}
		public int SelectedBodySliderCount
		{
			get { return BodySlide.Sliders.Count; }
		}

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

		public RmrSettingsViewModel RmrSettings { get; set; } = new RmrSettingsViewModel();


		public ObservableCollection<TriggerViewModel> Triggers { get; set; } = new ObservableCollection<TriggerViewModel>();
		public ObservableCollection<string> TriggerNames { get; set; } = new ObservableCollection<string>();
		//public ObservableCollection<SliderSetViewModel> SliderSets { get; set; } = new ObservableCollection<SliderSetViewModel>();


		BodySlideService BodySlide;
		RmrSettingsService RmrService;





		public MainViewModel()
		{
			// prepare dialogs
			AddTriggerDialog = new AddTriggerView();
			AddTriggerDialogContext = AddTriggerDialog.DataContext as AddTriggerViewModel;

			// load body types (e.g. CBBE, FusionGirl, BodyTalk...)
			var bodyService = new BodyService();
			var bodies = bodyService.LoadBodyList(Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderCategories")));
			BodyList.Clear();
			foreach (var body in bodies)
			{
				BodyList.Add(body);
			}

			// load presets
			var presetService = new SliderPresetService();
			var presets = presetService.LoadPresetList(Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderPresets")));
			PresetList.Clear();
			foreach (var preset in presets)
			{
				PresetList.Add(preset);
			}

			// prepare service to interact with BodySlide application
			BodySlide = new BodySlideService();

			// load RMR ini (slider sets and overrides)
			RmrService = new RmrSettingsService();
			RmrService.PopulateRmrSettings(
				Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Config", "LenA_RadMorphing", "settings.ini")),
				Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini")),
				RmrSettings
				);
			//SliderSets.Clear();
			//foreach (var sliderSet in sliders)
			//{
			//	SliderSets.Add(sliderSet);
			//}


			//TODO load triggers (saved in helper, and from ini)
			foreach (var triggerName in RmrSettings.SliderSetList.Select(it => it.TriggerName).Where(it=>!string.IsNullOrWhiteSpace(it)).Distinct())
			{
				AddTrigger(triggerName);
			}

			RmrSettings.SettingsChanged += RmrSettings_SettingsChanged;
			RmrSettings.SliderAdded += RmrSettings_SliderAdded;
			RmrSettings.SliderRemoved += RmrSettings_SliderRemoved;
		}

		private void RmrSettings_SliderRemoved(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var sliderName = e.PropertyName;
			var original = 0;
			if (SelectedPreset?.Sliders?.ContainsKey(sliderName) ?? false)
			{
				original = SelectedPreset.Sliders[sliderName];
			}
			BodySlide.SetSlider(sliderName, original);
		}

		private void RmrSettings_SliderAdded(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			ApplyMorphs();
		}

		private void RmrSettings_SettingsChanged(object sender, EventArgs e)
		{
			ApplyMorphs();
		}

		void AddTrigger(string name)
		{
			var trigger = new TriggerViewModel { Name = name, Value = 0 };
			Triggers.Add(trigger);
			TriggerNames.Add(name);
			trigger.PropertyChanged += Trigger_PropertyChanged;
		}

		private void Trigger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(TriggerViewModel.Value):
				case nameof(TriggerViewModel.AdditiveValue):
					ApplyMorphs();
					break;
			}
		}




		void ApplyMorphs()
		{
			bool? doctor = null;
			if (RmrSettings.OverrideOnlyDoctorCanReset != 0) doctor = RmrSettings.OverrideOnlyDoctorCanReset == 1;
			bool? isAdditive = null;
			if (RmrSettings.OverrideIsAdditive != 0) isAdditive = RmrSettings.OverrideIsAdditive == 1;
			bool? hasLimit = null;
			if (RmrSettings.OverrideHasAdditiveLimit != 0) hasLimit = RmrSettings.OverrideHasAdditiveLimit == 1;
			int? limit = null;
			if (RmrSettings.OverrideHasAdditiveLimit != 0) limit = RmrSettings.OverrideAdditiveLimit;
			foreach (var trigger in Triggers)
			{
				foreach (var sliderSet in RmrSettings.SliderSetList.Where(it=>it.TriggerName == trigger.Name))
				{
					var morph = sliderSet.GetMorph(
						trigger.Value,
						trigger.AdditiveValue,
						doctor, isAdditive, hasLimit, limit
						);
					foreach (var sliderName in sliderSet.SliderNameList)
					{
						var original = 0;
						if (SelectedPreset?.Sliders?.ContainsKey(sliderName) ?? false)
						{
							original = SelectedPreset.Sliders[sliderName];
						}
						BodySlide.SetSlider(sliderName, original + morph);
					}
				}
			}
		}




		#region Commands
		ICommand _addTriggerCommand;
		public ICommand AddTriggerCommand
		{
			get
			{
				if (_addTriggerCommand == null)
				{
					_addTriggerCommand = new RelayCommand(async(p) =>
					{
						if (await AddTriggerDialog.ShowAsync() == ContentDialogResult.Primary)
						{
							AddTrigger(AddTriggerDialogContext.Name);
						}
					});
				}
				return _addTriggerCommand;
			}
		}


		ICommand _saveRmrIniCommand;
		public ICommand SaveRmrIniCommand
        {
            get
            {
				if (_saveRmrIniCommand == null)
                {
					_saveRmrIniCommand = new RelayCommand(async (p) => {
						RmrService.SaveRmrSettings(
							Path.GetFullPath(Path.Combine(AppDir, "buffer.ini")),
							Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini")),
							RmrSettings
							);
					});
                }
				return _saveRmrIniCommand;
            }
        }

		ICommand _connectBodySlideCommand;
		public ICommand ConnectBodySlideCommand
        {
            get
            {
				return _connectBodySlideCommand ?? (_connectBodySlideCommand = new RelayCommand(async (p) =>
				{
					BodySlide.Categories = SelectedBody?.SliderCategories;
					OnPropertyChanged(nameof(SelectedBodySliderCount));
				}));
            }
        }
		#endregion
	}
}
