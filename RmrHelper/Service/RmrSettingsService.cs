using IniParser;
using IniParser.Model;
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
					InvertTriggerValue = int.Parse(section["bInvertTriggerValue"] ?? "0") == 1,
					TargetSizeIncrease = (int)float.Parse(section["fTargetMorph"], CultureInfo.InvariantCulture),
					LowerThreshold = (int)float.Parse(section["fThresholdMin"], CultureInfo.InvariantCulture),
					UpperThreshold = (int)float.Parse(section["fThresholdMax"], CultureInfo.InvariantCulture),
					ArmorSlotsToUnequip = section["sUnequipSlot"],
					UnequipThreshold = (int)float.Parse(section["fThresholdUnequip"] ?? "0", CultureInfo.InvariantCulture),
					OnlyDoctorCanReset = int.Parse(section["bOnlyDoctorCanReset"] ?? "0") == 1,
					IsAdditive = int.Parse(section["bIsAdditive"] ?? "0") == 1,
					HasAdditiveLimit = int.Parse(section["bHasAdditiveLimit"] ?? "0") == 1,
					AdditiveLimit = (int)float.Parse(section["fAdditiveLimit"] ?? "0", CultureInfo.InvariantCulture)
				};
				sliderSet.UpdateType = sliderSet.UpdateTypeList.FirstOrDefault(it => it.Item1 == int.Parse(section["iUpdateType"]));
				sliderSet.ApplyCompanion = sliderSet.ApplyCompanionList.FirstOrDefault(it => it.Item1 == int.Parse(section["iApplyCompanion"]));
				
				settings.AddSliderSet(sliderSet);
			}
		}

		public void SaveRmrSettings(string bufferPath, string userPath, RmrSettingsViewModel settings)
        {
			var parser = new FileIniDataParser();
			var ini = parser.ReadFile(userPath);
			ini["Override"]["iOnlyDoctorCanReset"] = settings.OverrideOnlyDoctorCanReset.ToString(CultureInfo.InvariantCulture);
			ini["Override"]["iIsAdditive"] = settings.OverrideIsAdditive.ToString(CultureInfo.InvariantCulture);
			ini["Override"]["iHasAdditiveLimit"] = settings.OverrideHasAdditiveLimit.ToString(CultureInfo.InvariantCulture);
			ini["Override"]["fAdditiveLimit"] = settings.OverrideAdditiveLimit.ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < settings.SliderSetList.Count; i++)
            {
				var set = settings.SliderSetList[i];
				ini[$"Slider{i}"]["sSliderName"] = set.SliderNames;
				ini[$"Slider{i}"]["sTriggerName"] = set.TriggerName;
				ini[$"Slider{i}"]["bInvertTriggerValue"] = set.InvertTriggerValue ? "1" : "0";
				ini[$"Slider{i}"]["iUpdateType"] = set.UpdateType.Item1.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["fTargetMorph"] = set.TargetSizeIncrease.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["fThresholdMin"] = set.LowerThreshold.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["fThresholdMax"] = set.UpperThreshold.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["sUnequipSlot"] = set.ArmorSlotsToUnequip;
				ini[$"Slider{i}"]["fThresholdUnequip"] = set.UnequipThreshold.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["iApplyCompanion"] = set.ApplyCompanion.Item1.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["bOnlyDoctorCanReset"] = set.OnlyDoctorCanReset ? "1" : "0";
				ini[$"Slider{i}"]["bIsAdditive"] = set.IsAdditive ? "1" : "0";
				ini[$"Slider{i}"]["bHasAdditiveLimit"] = set.HasAdditiveLimit ? "1" : "0";
				ini[$"Slider{i}"]["fAdditiveLimit"] = set.AdditiveLimit.ToString(CultureInfo.InvariantCulture);
			}

			parser.WriteFile(userPath, ini);
		}
	}
}
