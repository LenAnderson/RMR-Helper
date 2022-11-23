using IniParser;
using IniParser.Model;
using RmrHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
			if (File.Exists(defaultsPath))
			{
				var ini = parser.ReadFile(defaultsPath);
				if (File.Exists(userPath)) { 
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
						SliderSetViewModel sliderSet;
						if (settings.SliderSetList.Count > i)
						{
							sliderSet = settings.SliderSetList[i];
						}
						else
						{
							sliderSet = new SliderSetViewModel();
							settings.AddSliderSet(sliderSet);
						}
						sliderSet.Title = $"Slider Set {i + 1}";
						sliderSet.SliderNames = section["sSliderName"];
						sliderSet.TriggerName = section["sTriggerName"];
						sliderSet.InvertTriggerValue = int.Parse(section["bInvertTriggerValue"] ?? "0") == 1;
						sliderSet.TargetSizeIncrease = (int)float.Parse(section["fTargetMorph"], CultureInfo.InvariantCulture);
						sliderSet.LowerThreshold = (int)float.Parse(section["fThresholdMin"], CultureInfo.InvariantCulture);
						sliderSet.UpperThreshold = (int)float.Parse(section["fThresholdMax"], CultureInfo.InvariantCulture);
						sliderSet.ArmorSlotsToUnequip = section["sUnequipSlot"];
						sliderSet.UnequipThreshold = (int)float.Parse(section["fThresholdUnequip"] ?? "0", CultureInfo.InvariantCulture);
						sliderSet.OnlyDoctorCanReset = int.Parse(section["bOnlyDoctorCanReset"] ?? "0") == 1;
						sliderSet.IsAdditive = int.Parse(section["bIsAdditive"] ?? "0") == 1;
						sliderSet.HasAdditiveLimit = int.Parse(section["bHasAdditiveLimit"] ?? "0") == 1;
						sliderSet.AdditiveLimit = (int)float.Parse(section["fAdditiveLimit"] ?? "0", CultureInfo.InvariantCulture);
						if (string.IsNullOrWhiteSpace(sliderSet.TriggerName))
                        {
							sliderSet.TriggerName = null;
                        }
						sliderSet.UpdateType = sliderSet.UpdateTypeList.FirstOrDefault(it => it.Item1 == int.Parse(section["iUpdateType"]));
						sliderSet.ApplyTo = sliderSet.ApplyToList.FirstOrDefault(it => it.Item1 == int.Parse(section["iApplyTo"] ?? "-1"));
						sliderSet.Sex = sliderSet.SexList.FirstOrDefault(it => it.Item1 == int.Parse(section["iSex"] ?? "-1"));
					}
                    for (int i = settings.NumberOfSliderSets; i < settings.SliderSetList.Count; i++)
                    {
						var sliderSet = settings.SliderSetList[i];
						sliderSet.SliderNames = "";
						sliderSet.TriggerName = "";
						sliderSet.InvertTriggerValue = false;
						sliderSet.TargetSizeIncrease = 100;
						sliderSet.LowerThreshold = 0;
						sliderSet.UpperThreshold = 100;
						sliderSet.ArmorSlotsToUnequip = "";
						sliderSet.UnequipThreshold = 0;
						sliderSet.OnlyDoctorCanReset = false;
						sliderSet.IsAdditive = false;
						sliderSet.HasAdditiveLimit = true;
						sliderSet.AdditiveLimit = 0;
					}
				}
			}
		}

		public void SaveRmrSettings(string bufferPath, string userPath, RmrSettingsViewModel settings)
        {
			var parser = new FileIniDataParser();
			IniData ini;
			if (File.Exists(userPath))
			{
				ini = parser.ReadFile(userPath);
			}
            else
            {
				ini = new IniData();
            }
			ini["Override"]["iOnlyDoctorCanReset"] = settings.OverrideOnlyDoctorCanReset.ToString(CultureInfo.InvariantCulture);
			ini["Override"]["iIsAdditive"] = settings.OverrideIsAdditive.ToString(CultureInfo.InvariantCulture);
			ini["Override"]["iHasAdditiveLimit"] = settings.OverrideHasAdditiveLimit.ToString(CultureInfo.InvariantCulture);
			ini["Override"]["fAdditiveLimit"] = settings.OverrideAdditiveLimit.ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < settings.SliderSetList.Count; i++)
            {
				var set = settings.SliderSetList[i];
				ini[$"Slider{i}"]["sSliderName"] = set.SliderNames;
				ini[$"Slider{i}"]["iApplyTo"] = set.ApplyTo.Item1.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["iSex"] = set.Sex.Item1.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["sTriggerName"] = set.TriggerName;
				ini[$"Slider{i}"]["bInvertTriggerValue"] = set.InvertTriggerValue ? "1" : "0";
				ini[$"Slider{i}"]["iUpdateType"] = set.UpdateType.Item1.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["fTargetMorph"] = set.TargetSizeIncrease.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["fThresholdMin"] = set.LowerThreshold.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["fThresholdMax"] = set.UpperThreshold.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["sUnequipSlot"] = set.ArmorSlotsToUnequip;
				ini[$"Slider{i}"]["fThresholdUnequip"] = set.UnequipThreshold.ToString(CultureInfo.InvariantCulture);
				ini[$"Slider{i}"]["bOnlyDoctorCanReset"] = set.OnlyDoctorCanReset ? "1" : "0";
				ini[$"Slider{i}"]["bIsAdditive"] = set.IsAdditive ? "1" : "0";
				ini[$"Slider{i}"]["bHasAdditiveLimit"] = set.HasAdditiveLimit ? "1" : "0";
				ini[$"Slider{i}"]["fAdditiveLimit"] = set.AdditiveLimit.ToString(CultureInfo.InvariantCulture);
			}

			Directory.CreateDirectory(Path.GetDirectoryName(bufferPath));
			parser.WriteFile(bufferPath, ini);
			File.WriteAllText(userPath, $"\n{File.ReadAllText(bufferPath)}");
		}
	}
}
