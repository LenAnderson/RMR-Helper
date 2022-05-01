using IniParser;
using RmrHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.Service
{
	public class RmrSettingsService
	{
		public List<SliderSetViewModel> LoadRmrSettings(string defaultsPath, string userPath)
		{
			var sliderSets = new List<SliderSetViewModel>();

			var parser = new FileIniDataParser();
			var ini = parser.ReadFile(defaultsPath);
			var userIni = parser.ReadFile(userPath);
			ini.Merge(userIni);

			var x = ini["Static"]["iNumberOfSliderSets"];
			int numberOfSliderSets = int.Parse(ini["Static"]["iNumberOfSliderSets"], CultureInfo.InvariantCulture);

			for (int i = 0; i < numberOfSliderSets; i++)
			{
				var section = ini[$"Slider{i}"];
				var sliderSet = new SliderSetViewModel
				{
					Title = $"Slider Set {i + 1}",
					SliderNames = section["sSliderName"],
					TriggerName = section["sTriggerName"],
					TargetSizeIncrease = (int)float.Parse(section["fTargetMorph"], CultureInfo.InvariantCulture),
					LowerThreshold = (int)float.Parse(section["fThresholdMin"], CultureInfo.InvariantCulture),
					UpperThreshold = (int)float.Parse(section["fThresholdMax"], CultureInfo.InvariantCulture)
				};
				sliderSets.Add(sliderSet);
			}

			return sliderSets;
		}
	}
}
