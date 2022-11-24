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

		private ContentDialog PresetToPresetDialog;
		private PresetToPresetViewModel PresetToPresetDialogContext;

		private ContentDialog ErrorDialog;
		private ErrorViewModel ErrorDialogContext;


		public string RmrIniPath
		{
			get { return Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini")); }
		}
		public string WindowTitle
		{
			get
			{
				var ver = Assembly.GetExecutingAssembly().GetName().Version;
				return $"RMR Helper v{ver.Major}.{ver.Minor}.{ver.Build}";
			}
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
			get { return BodySlide?.Sliders?.Count ?? 0; }
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


		BodySlideService BodySlide;
		RmrSettingsService RmrService;





		public MainViewModel()
		{
			Logger.Clear();
			Logger.Log("starting RmrHelper");

			// prepare dialogs
			Logger.Log("preparing dialogs");
			AddTriggerDialog = new AddTriggerView();
			AddTriggerDialogContext = AddTriggerDialog.DataContext as AddTriggerViewModel;
			PresetToPresetDialog = new PresetToPresetView();
			PresetToPresetDialogContext = PresetToPresetDialog.DataContext as PresetToPresetViewModel;
			ErrorDialog = new ErrorView();
			ErrorDialogContext = ErrorDialog.DataContext as ErrorViewModel;

			Init();
		}

		private async void Init()
        {
			// load body types (e.g. CBBE, FusionGirl, BodyTalk...)
			Logger.Log("loading body types");
			var bodyService = new BodyService();
			var bodies = bodyService.LoadBodyList(Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderCategories")));
			if (bodyService.Errors.Count > 0)
			{
				ErrorDialogContext.Errors.Clear();
				foreach (var err in bodyService.Errors)
				{
					ErrorDialogContext.Errors.Add(err);
				}
				bodyService.Errors.Clear();
				await ErrorDialog.ShowAsync();
			}
			BodyList.Clear();
			foreach (var body in bodies)
			{
				BodyList.Add(body);
			}

			// load presets
			Logger.Log("loading presets");
			var presetService = new SliderPresetService();
			var presets = presetService.LoadPresetList(Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderPresets")));
			if (presetService.Errors.Count > 0)
			{
				ErrorDialogContext.Errors.Clear();
				foreach(var err in presetService.Errors)
                {
					ErrorDialogContext.Errors.Add(err);
                }
				presetService.Errors.Clear();
				await ErrorDialog.ShowAsync();
			}
			PresetList.Clear();
			PresetToPresetDialogContext.PresetList.Clear();
			foreach (var preset in presets)
			{
				PresetList.Add(preset);
				PresetToPresetDialogContext.PresetList.Add(preset);
			}

			// prepare service to interact with BodySlide application
			Logger.Log("preparing BodySlide service");
			BodySlide = new BodySlideService();

			// load RMR ini (slider sets and overrides)
			Logger.Log("loading RMR ini");
			RmrService = new RmrSettingsService();
			RmrService.PopulateRmrSettings(
				Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Config", "RadMorphingRedux", "settings.ini")),
				Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "RadMorphingRedux.ini")),
				RmrSettings
				);
			if (RmrService.Errors.Count > 0)
            {
				ErrorDialogContext.Errors.Clear();
				foreach(string err in RmrService.Errors)
                {
					ErrorDialogContext.Errors.Add(err);
                }
				RmrService.Errors.Clear();
				await ErrorDialog.ShowAsync();
            }


			//TODO load triggers (saved in helper, and from ini)
			Logger.Log("loading triggers");
			foreach (var triggerName in RmrSettings.SliderSetList.Select(it => it.TriggerName).Where(it => !string.IsNullOrWhiteSpace(it)).Distinct())
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
			Logger.Log($"AddTrigger: {name}");
			var trigger = new TriggerViewModel { Name = name, Value = 0 };
			Triggers.Add(trigger);
			TriggerNames.Add(name);
			PresetToPresetDialogContext.TriggerNameList.Add(name);
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
			Logger.Log($"ApplyMorphs");
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
							Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "RadMorphingRedux.ini")),
							RmrSettings
							);
						if (RmrService.Errors.Count > 0)
						{
							ErrorDialogContext.Errors.Clear();
							foreach (string err in RmrService.Errors)
							{
								ErrorDialogContext.Errors.Add(err);
							}
							RmrService.Errors.Clear();
							await ErrorDialog.ShowAsync();
						}
					});
                }
				return _saveRmrIniCommand;
            }
        }

		ICommand _saveAsCommand;
		public ICommand SaveAsCommand
        {
            get
            {
				if (_saveAsCommand == null)
                {
					_saveAsCommand = new RelayCommand(async (p) =>
					{
						var dir = Path.GetFullPath(Path.Combine(AppDir, "SavedSettings"));
						if (!Directory.Exists(dir))
                        {
							Directory.CreateDirectory(dir);
                        }
						var sfd = new SaveFileDialog();
						sfd.Filter = "INI file (*.ini)|*.ini";
						sfd.InitialDirectory = dir;
						if (sfd.ShowDialog() == true)
						{
							RmrService.SaveRmrSettings(
								Path.GetFullPath(Path.Combine(AppDir, "buffer.ini")),
								sfd.FileName,
								RmrSettings
								);
							if (RmrService.Errors.Count > 0)
							{
								ErrorDialogContext.Errors.Clear();
								foreach (string err in RmrService.Errors)
								{
									ErrorDialogContext.Errors.Add(err);
								}
								RmrService.Errors.Clear();
								await ErrorDialog.ShowAsync();
							}
						}
					});
                }
				return _saveAsCommand;
            }
        }

		ICommand _loadCommand;
		public ICommand LoadCommand
        {
            get
            {
				if (_loadCommand == null)
                {
					_loadCommand = new RelayCommand(async (p) =>
					{
						var ofd = new OpenFileDialog();
						ofd.Filter = "INI file (*.ini)|*.ini";
						ofd.InitialDirectory = Path.GetFullPath(Path.Combine(AppDir, "SavedSettings"));
						if (ofd.ShowDialog() == true)
                        {
							RmrService.PopulateRmrSettings(
								Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Config", "RadMorphingRedux", "settings.ini")),
								ofd.FileName,
								RmrSettings
								);
							if (RmrService.Errors.Count > 0)
							{
								ErrorDialogContext.Errors.Clear();
								foreach (string err in RmrService.Errors)
								{
									ErrorDialogContext.Errors.Add(err);
								}
								RmrService.Errors.Clear();
								await ErrorDialog.ShowAsync();
							}
						}
					});
                }
				return _loadCommand;
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

		ICommand _presetToPresetCommand;
		public ICommand PresetToPresetCommand
		{
			get
			{
				return _presetToPresetCommand ?? (_presetToPresetCommand = new RelayCommand(async (p) =>
				{
					if (await PresetToPresetDialog.ShowAsync() == ContentDialogResult.Primary && PresetToPresetDialogContext.SelectedPreset != null)
					{
						var src = SelectedPreset;
						var dst = PresetToPresetDialogContext.SelectedPreset;

						var sliders = new Dictionary<string, Tuple<int, int, int>>();
						foreach (var kv in src.Sliders)
						{
							sliders[kv.Key] = new Tuple<int, int, int>(kv.Value, 0, -kv.Value);
						}
						foreach (var kv in dst.Sliders)
						{
							if (sliders.ContainsKey(kv.Key))
							{
								sliders[kv.Key] = new Tuple<int, int, int>(sliders[kv.Key].Item1, kv.Value, kv.Value - sliders[kv.Key].Item1);
							}
							else
							{
								sliders[kv.Key] = new Tuple<int, int, int>(0, kv.Value, kv.Value);
							}
						}
						
						var sliderSets = (
							from kv in sliders
							where kv.Value.Item3 != 0
							group kv by kv.Value.Item3 into g
							select new KeyValuePair<int, List<string>>(g.Key, g.Select(kv=>kv.Key).ToList())
						).ToList();

						for (int i = 0; i < RmrSettings.SliderSetList.Count; i++)
						{
							var sliderSet = RmrSettings.SliderSetList[i];
							if (i < sliderSets.Count)
							{
								sliderSet.TargetSizeIncrease = sliderSets[i].Key;
								sliderSet.SliderNames = string.Join("|", sliderSets[i].Value);

								sliderSet.ApplyTo = PresetToPresetDialogContext.ApplyTo;
								sliderSet.Sex = PresetToPresetDialogContext.Sex;
								sliderSet.TriggerName = PresetToPresetDialogContext.TriggerName;
								sliderSet.InvertTriggerValue = PresetToPresetDialogContext.InvertTriggerValue;
								sliderSet.UpdateType = PresetToPresetDialogContext.UpdateType;
								sliderSet.LowerThreshold = PresetToPresetDialogContext.LowerThreshold;
								sliderSet.UpperThreshold = PresetToPresetDialogContext.UpperThreshold;
							}
							else
							{
								sliderSet.SliderNames = "";
							}
						}
						if (sliderSets.Count > RmrSettings.SliderSetList.Count)
                        {
                            for (int i = RmrSettings.SliderSetList.Count; i < sliderSets.Count; i++)
                            {
								var sliderSet = new SliderSetViewModel();
								sliderSet.Title = $"Slider Set {i+1}";
								sliderSet.TargetSizeIncrease = sliderSets[i].Key;
								sliderSet.SliderNames = string.Join("|", sliderSets[i].Value);

								sliderSet.ApplyTo = PresetToPresetDialogContext.ApplyTo;
								sliderSet.Sex = PresetToPresetDialogContext.Sex;
								sliderSet.TriggerName = PresetToPresetDialogContext.TriggerName;
								sliderSet.InvertTriggerValue = PresetToPresetDialogContext.InvertTriggerValue;
								sliderSet.UpdateType = PresetToPresetDialogContext.UpdateType;
								sliderSet.LowerThreshold = PresetToPresetDialogContext.LowerThreshold;
								sliderSet.UpperThreshold = PresetToPresetDialogContext.UpperThreshold;
								RmrSettings.SliderSetList.Add(sliderSet);
							}
                        }
					}
				}));
			}
		}
		#endregion
	}
}
