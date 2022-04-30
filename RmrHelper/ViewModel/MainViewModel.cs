using Microsoft.Win32;
using RmrHelper.Helpers;
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


		private string _rmrIniPath;
		public string RmrIniPath
		{
			get { return _rmrIniPath; }
			set
			{
				if (_rmrIniPath != value)
				{
					_rmrIniPath = value;
					OnPropertyChanged(nameof(RmrIniPath));
				}
			}
		}


		private string _sliderCategoriesXmlPath;
		public string SliderCategoriesXmlPath
		{
			get { return _sliderCategoriesXmlPath; }
			set
			{
				if (_sliderCategoriesXmlPath != value)
				{
					_sliderCategoriesXmlPath = value;
					OnPropertyChanged(nameof(SliderCategoriesXmlPath));
				}
			}
		}


		private string _presetXmlPath;
		public string PresetXmlPath
		{
			get { return _presetXmlPath; }
			set
			{
				if (_presetXmlPath != value)
				{
					_presetXmlPath = value;
					OnPropertyChanged(nameof(PresetXmlPath));
				}
			}
		}


		public ObservableCollection<TriggerViewModel> Triggers { get; set; } = new ObservableCollection<TriggerViewModel>();
		public ObservableCollection<SliderSetViewModel> SliderSets { get; set; } = new ObservableCollection<SliderSetViewModel>();


		BodySlideService BodySlide;





		public MainViewModel()
		{
			RmrIniPath = Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings", "LenA_RadMorphing.ini"));
			SliderCategoriesXmlPath = Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderCategories", "CBBE.xml"));
			PresetXmlPath = Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderPresets", "CBBE.xml"));

			var categories = new Dictionary<string, List<Slider>>();
			var sliderXmlText = File.ReadAllText(SliderCategoriesXmlPath);
			var doc = XDocument.Parse(sliderXmlText);
			foreach (var group in doc.Descendants("Category"))
			{
				var groupName = group.Attribute("name").Value;
				categories[groupName] = new List<Slider>();
				foreach(var slider in group.Descendants("Slider"))
				{
					var sliderName = slider.Attribute("name").Value;
					var sliderDisplayName = slider.Attribute("displayname").Value;
					categories[groupName].Add(new Slider { Name = sliderName, DisplayName = sliderDisplayName });
					var x = 1;
				}
			}

			BodySlide = new BodySlideService(categories);

			Triggers.Add(new TriggerViewModel { Name = "Rads", Value=0 });
			Triggers.Add(new TriggerViewModel { Name = "HP", Value = 0 });
			foreach (var trigger in Triggers)
			{
				trigger.PropertyChanged += Trigger_PropertyChanged;
			}

			SliderSets.Add(new SliderSetViewModel
			{
				Title = "Slider Set 1",
				SliderNames = "Breasts|BreastFantasy",
				TriggerName = "Rads",
				TargetSizeIncrease = 110,
				LowerThreshold = 10,
				UpperThreshold = 70
			});
			SliderSets.Add(new SliderSetViewModel
			{
				Title = "Slider Set 1",
				SliderNames = "Butt",
				TriggerName = "Rads",
				TargetSizeIncrease = 200,
				LowerThreshold = 40,
				UpperThreshold = 90
			});
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
					foreach (var sliderName in sliderSet.SliderNameList)
					{
						BodySlide.SetSlider(sliderName, trigger.Value);
					}
				}
			}
		}




		#region Commands
		private ICommand _browseForRmrIniCommand;
		public ICommand BrowseForRmrIniCommand
		{
			get
			{
				if (_browseForRmrIniCommand == null)
				{
					_browseForRmrIniCommand = new RelayCommand(async p =>
					{
						try
						{
							var ofd = new OpenFileDialog();
							ofd.Filter = "LenA_RadMorphing.ini (*.ini)|*.ini";
							ofd.InitialDirectory = Path.GetFullPath(Path.Combine(AppDir, "..", "..", "MCM", "Settings"));
							ofd.DefaultExt = ".ini";
							if (ofd.ShowDialog() == true)
							{
								if (File.Exists(ofd.FileName))
								{
									//TODO load ini file
									RmrIniPath = ofd.FileName;
								}
							}
						}
						catch (Exception ex)
						{
							//TODO notify about exception
							var x = 1;
						}
					});
				}
				return _browseForRmrIniCommand;
			}
		}


		private ICommand _browseForSliderCategoriesXmlCommand;
		public ICommand BrowseForSliderCategoriesXmlCommand
		{
			get
			{
				if (_browseForSliderCategoriesXmlCommand == null)
				{
					_browseForSliderCategoriesXmlCommand = new RelayCommand(async p =>
					{
						try
						{
							var ofd = new OpenFileDialog();
							ofd.Filter = "Slider Categories XML (*.xml)|*.xml";
							ofd.InitialDirectory = Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderCategories"));
							ofd.DefaultExt = ".xml";
							if (ofd.ShowDialog() == true)
							{
								if (File.Exists(ofd.FileName))
								{
									//TODO load ini file
									SliderCategoriesXmlPath = ofd.FileName;
								}
							}
						}
						catch (Exception ex)
						{
							//TODO notify about exception
							var x = 1;
						}
					});
				}
				return _browseForSliderCategoriesXmlCommand;
			}
		}


		private ICommand _browseForPresetXmlCommand;
		public ICommand BrowseForPresetXmlCommand
		{
			get
			{
				if (_browseForPresetXmlCommand == null)
				{
					_browseForPresetXmlCommand = new RelayCommand(async p =>
					{
						try
						{
							var ofd = new OpenFileDialog();
							ofd.Filter = "BodySlide Preset XML (*.xml)|*.xml";
							ofd.InitialDirectory = Path.GetFullPath(Path.Combine(AppDir, "..", "BodySlide", "SliderPresets"));
							ofd.DefaultExt = ".xml";
							if (ofd.ShowDialog() == true)
							{
								if (File.Exists(ofd.FileName))
								{
									//TODO load ini file
									PresetXmlPath = ofd.FileName;
								}
							}
						}
						catch (Exception ex)
						{
							//TODO notify about exception
							var x = 1;
						}
					});
				}
				return _browseForPresetXmlCommand;
			}
		}
		#endregion
	}
}
