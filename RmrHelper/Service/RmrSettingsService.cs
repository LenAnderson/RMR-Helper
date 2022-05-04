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
		public void PopulateRmrSettings(string defaultsPath, string userPath, RmrSettingsViewModel settings)
		{
			var parser = new FileIniDataParser();
			var ini = parser.ReadFile(defaultsPath);
			var userIni = parser.ReadFile(userPath);
			ini.Merge(userIni);

			settings.NumberOfSliderSets = int.Parse(ini["Static"]["iNumberOfSliderSets"], CultureInfo.InvariantCulture);
			settings.OverrideOnlyDoctorCanReset = int.Parse(ini["Override"]["iOnlyDoctorCanReset"], CultureInfo.InvariantCulture);
			settings.OverrideIsAdditive = int.Parse(ini["Override"]["iIsAdditive"], CultureInfo.InvariantCulture);
			settings.OverrideHasAdditiveLimit = int.Parse(ini["Override"]["iHasAdditiveLimit"], CultureInfo.InvariantCulture);
			settings.OverrideAdditiveLimit = (int)float.Parse(ini["Override"]["fAdditiveLimit"], CultureInfo.InvariantCulture);

			for (int i = 0; i < settings.NumberOfSliderSets; i++)
			{
				var section = ini[$"Slider{i}"];
				var sliderSet = new SliderSetViewModel
				{
					Title = $"Slider Set {i + 1}",
					SliderNames = section["sSliderName"],
					TriggerName = section["sTriggerName"],
					TargetSizeIncrease = (int)float.Parse(section["fTargetMorph"], CultureInfo.InvariantCulture),
					LowerThreshold = (int)float.Parse(section["fThresholdMin"], CultureInfo.InvariantCulture),
					UpperThreshold = (int)float.Parse(section["fThresholdMax"], CultureInfo.InvariantCulture),
					ArmorSlotsToUnequip = section["sUnequipSlot"],
					UnequipThreshold = (int)float.Parse(section["fThresholdUnequip"] ??"0", CultureInfo.InvariantCulture),
					OnlyDoctorCanReset = int.Parse(section["bOnlyDoctorCanReset"] ?? "0") == 1,
					IsAdditive = int.Parse(section["bIsAdditive"] ?? "0") == 1,
					HasAdditiveLimit = int.Parse(section["bHasAdditiveLimit"] ?? "0") == 1,
					AdditiveLimit = (int)float.Parse(section["fAdditiveLimit"] ?? "0", CultureInfo.InvariantCulture)
				};
				sliderSet.UpdateType = sliderSet.UpdateTypeList.FirstOrDefault(it => it.Item1 == int.Parse(section["iUpdateType"]));
				
				settings.AddSliderSet(sliderSet);
			}
		}
	}
}
