using IniParser;
using Microsoft.Win32;
using RmrHelper.Helpers;
using RmrHelper.Model;
using RmrHelper.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
				return @"C:\Games\Steam\steamapps\common\Fallout 4\Data\Tools\RmrHelper";
				#endif
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}


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


		public ObservableCollection<TriggerViewModel> Triggers { get; set; } = new ObservableCollection<TriggerViewModel>();
		public ObservableCollection<SliderSetViewModel> SliderSets { get; set; } = new ObservableCollection<SliderSetViewModel>();


		BodySlideService BodySlide;





		public MainViewModel()
		{
			//RmrIniPath = Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini"));
			//SliderCategoriesXmlPath = Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderCategories", "CBBE.xml"));
			//PresetXmlPath = Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderPresets", "CBBE.xml"));

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

			var rmrService = new RmrSettingsService();
			var sliders = rmrService.LoadRmrSettings(
				Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Config", "LenA_RadMorphing", "settings.ini")),
				Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini"))
				);
			SliderSets.Clear();
			foreach (var sliderSet in sliders)
			{
				SliderSets.Add(sliderSet);
			}


			//TODO load triggers (saved in helper, and from ini)
			Triggers.Add(new TriggerViewModel { Name = "Rads", Value=0 });
			Triggers.Add(new TriggerViewModel { Name = "HP", Value = 0 });
			foreach (var trigger in Triggers)
			{
				trigger.PropertyChanged += Trigger_PropertyChanged;
			}
		}

		private void Trigger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(TriggerViewModel.Value):
					ApplyMorphs();
					break;
			}
		}




		void ApplyMorphs()
		{
			foreach (var trigger in Triggers)
			{
				foreach (var sliderSet in SliderSets.Where(it=>it.TriggerName == trigger.Name))
				{
					var morph = sliderSet.GetMorph(trigger.Value);
					foreach (var sliderName in sliderSet.SliderNameList)
					{
						var original = 0;
						if (SelectedPreset.Sliders.ContainsKey(sliderName))
						{
							original = SelectedPreset.Sliders[sliderName];
						}
						BodySlide.SetSlider(sliderName, original + morph);
					}
				}
			}
		}




		#region Commands
		#endregion
	}
}
