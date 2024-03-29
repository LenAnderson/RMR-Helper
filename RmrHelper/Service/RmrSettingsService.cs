﻿using IniParser;
using IniParser.Model;
using RmrHelper.Helpers;
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
		public List<string> Errors = new List<string>();

		public void PopulateRmrSettings(string defaultsPath, string userPath, RmrSettingsViewModel settings)
		{
			Logger.Log($"RmrSettingsService.PopulateRmrSettings: defaultPath={defaultsPath}; userPath={userPath}");
			try
			{
				var parser = new FileIniDataParser();
				if (File.Exists(defaultsPath))
				{
					Logger.Log("found defaults ini");
					var ini = parser.ReadFile(defaultsPath);
					Logger.Log("parsed defaults ini");
					if (File.Exists(userPath))
					{
						Logger.Log("found user ini");
						var userIni = parser.ReadFile(userPath);
						Logger.Log("parsed user ini");
						ini.Merge(userIni);
						Logger.Log("merged defaults and user ini");

						settings.NumberOfSliderSets = int.Parse(ini["Static"]["iNumberOfSliderSets"], CultureInfo.InvariantCulture);
						settings.OverrideOnlyDoctorCanReset = int.Parse(ini["Override"]["iOnlyDoctorCanReset"], CultureInfo.InvariantCulture);
						settings.OverrideIsAdditive = int.Parse(ini["Override"]["iIsAdditive"], CultureInfo.InvariantCulture);
						settings.OverrideHasAdditiveLimit = int.Parse(ini["Override"]["iHasAdditiveLimit"], CultureInfo.InvariantCulture);
						settings.OverrideAdditiveLimit = (int)float.Parse(ini["Override"]["fAdditiveLimit"], CultureInfo.InvariantCulture);
						settings.OverrideUnequipAction = settings.OverrideUnequipActionList.FirstOrDefault(it=>it.Item1==int.Parse(ini["Override"]["iUnequipAction"] ?? "-1", CultureInfo.InvariantCulture));
						settings.OverrideUnequipDropChance = int.Parse(ini["Override"]["bOverrideUnequipDropChance"] ?? "0") == 1;
						settings.OverrideUnequipDropChanceValue = (int)float.Parse(ini["Override"]["fUnequipDropChance"] ?? "0.0", CultureInfo.InvariantCulture);
						Logger.Log("parsed static and override settings");

						for (int i = 0; i < settings.NumberOfSliderSets; i++)
						{
							Logger.Log($"parsing slider set {i}");
							var section = ini[$"Slider{i}"];
							SliderSetViewModel sliderSet;
							if (settings.SliderSetList.Count > i)
							{
								Logger.Log("getting existing slider set");
								sliderSet = settings.SliderSetList[i];
							}
							else
							{
								Logger.Log("adding new slider set");
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
							sliderSet.UnequipDropChance = (int)float.Parse(section["fUnequipDropChance"] ?? "0", CultureInfo.InvariantCulture);
							sliderSet.OnlyDoctorCanReset = int.Parse(section["bOnlyDoctorCanReset"] ?? "0") == 1;
							sliderSet.IsAdditive = int.Parse(section["bIsAdditive"] ?? "0") == 1;
							sliderSet.HasAdditiveLimit = int.Parse(section["bHasAdditiveLimit"] ?? "0") == 1;
							sliderSet.AdditiveLimit = (int)float.Parse(section["fAdditiveLimit"] ?? "0", CultureInfo.InvariantCulture);
							if (string.IsNullOrWhiteSpace(sliderSet.TriggerName))
							{
								sliderSet.TriggerName = null;
							}
							Logger.Log("parsed basics");
							sliderSet.UpdateType = sliderSet.UpdateTypeList.FirstOrDefault(it => it.Item1 == int.Parse(section["iUpdateType"]));
							sliderSet.ApplyTo = sliderSet.ApplyToList.FirstOrDefault(it => it.Item1 == int.Parse(section["iApplyTo"] ?? "-1"));
							sliderSet.UnequipAction = sliderSet.UnequipActionList.FirstOrDefault(it => it.Item1 == int.Parse(section["iUnequipAction"] ?? "-1"));
							sliderSet.Sex = sliderSet.SexList.FirstOrDefault(it => it.Item1 == int.Parse(section["iSex"] ?? "-1"));
							Logger.Log("parsed dropdowns");
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
						Logger.Log($"ini loaded with {settings.SliderSetList.Count} slider sets");
					}
					else
					{
						Logger.Log($"user file does not exist: {userPath}", "WARN");
					}
				}
				else
				{
					Logger.Log($"defaults file does not exist: {defaultsPath}", "WARN");
					Errors.Add($"MCM default settings file does not exist at {defaultsPath}");
				}
			}
			catch (Exception ex)
            {
				Logger.Log(ex);
				Errors.Add($"Failed to load MCM settings.\n{ex.Message}");
            }
		}

		public void SaveRmrSettings(string bufferPath, string userPath, RmrSettingsViewModel settings)
		{
			Logger.Log($"RmrSettingsService.SaveRmrSettings: bufferPath={bufferPath}; userPath={userPath}");
			try
			{
				var parser = new FileIniDataParser();
				IniData ini;
				if (File.Exists(userPath))
				{
					Logger.Log("found user ini");
					ini = parser.ReadFile(userPath);
					Logger.Log("parsed user ini");
				}
				else
				{
					Logger.Log("user ini not found, starting with blank ini");
					ini = new IniData();
				}
				var now = DateTime.Now;
				ini["Internal"]["sModified"] = DateTime.Now.ToString("yyyyMMddHHmmss");
				ini["Override"]["iOnlyDoctorCanReset"] = settings.OverrideOnlyDoctorCanReset.ToString(CultureInfo.InvariantCulture);
				ini["Override"]["iIsAdditive"] = settings.OverrideIsAdditive.ToString(CultureInfo.InvariantCulture);
				ini["Override"]["iHasAdditiveLimit"] = settings.OverrideHasAdditiveLimit.ToString(CultureInfo.InvariantCulture);
				ini["Override"]["fAdditiveLimit"] = settings.OverrideAdditiveLimit.ToString(CultureInfo.InvariantCulture);
				ini["Override"]["iUnequipAction"] = settings.OverrideUnequipAction.Item1.ToString(CultureInfo.InvariantCulture);
				ini["Override"]["bOverrideUnequipDropChance"] = settings.OverrideUnequipDropChance ? "1" : "0";
				ini["Override"]["fUnequipDropChance"] = settings.OverrideUnequipDropChanceValue.ToString(CultureInfo.InvariantCulture);
				Logger.Log("finished setting static and override settings into ini dict");

				for (int i = 0; i < settings.SliderSetList.Count; i++)
				{
					Logger.Log($"slider set {i}");
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
					ini[$"Slider{i}"]["iUnequipAction"] = set.UnequipAction.Item1.ToString(CultureInfo.InvariantCulture);
					ini[$"Slider{i}"]["fUnequipDropChance"] = set.UnequipDropChance.ToString(CultureInfo.InvariantCulture);
					ini[$"Slider{i}"]["bOnlyDoctorCanReset"] = set.OnlyDoctorCanReset ? "1" : "0";
					ini[$"Slider{i}"]["bIsAdditive"] = set.IsAdditive ? "1" : "0";
					ini[$"Slider{i}"]["bHasAdditiveLimit"] = set.HasAdditiveLimit ? "1" : "0";
					ini[$"Slider{i}"]["fAdditiveLimit"] = set.AdditiveLimit.ToString(CultureInfo.InvariantCulture);
				}

				Logger.Log("create directory");
				Directory.CreateDirectory(Path.GetDirectoryName(bufferPath));
				Logger.Log("write buffer file");
				parser.WriteFile(bufferPath, ini);
				Logger.Log("write user file");
				File.WriteAllText(userPath, $"\n{File.ReadAllText(bufferPath)}");
				Logger.Log($"ini saved to {userPath}");
			}
			catch (Exception ex)
            {
				Logger.Log(ex);
				Errors.Add($"Failed to save MCM settings.\n{ex.Message}");
            }
		}
	}
}
