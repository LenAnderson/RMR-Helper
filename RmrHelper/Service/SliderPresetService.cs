﻿using RmrHelper.Helpers;
using RmrHelper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RmrHelper.Service
{
	public class SliderPresetService
	{
		public List<PresetModel> LoadPresetList(string dirPath)
		{
			Logger.Log($"SliderPresetService.LoadPresetList: {dirPath}");
			var presetList = new List<PresetModel>();
			if (Directory.Exists(dirPath))
			{
				var files = Directory.GetFiles(dirPath).Where(it => it.ToLowerInvariant().EndsWith(".xml")).ToList();
				foreach (var filePath in files)
				{
					presetList.AddRange(LoadPresetFile(filePath));
				}

				presetList = presetList.OrderBy(it => it.Name.ToLowerInvariant()).ToList();
			}
            else
            {
				Logger.Log($"directory does not exist: {dirPath}", "WARN");
			}

			return presetList;
		}

		
		List<PresetModel> LoadPresetFile(string filePath)
		{
			Logger.Log($"SliderPresetService.LoadPresetFile: {filePath}");
			var presetList = new List<PresetModel>();
			var doc = XDocument.Load(filePath).Root;
			foreach (var preset in doc.Descendants("Preset"))
			{
				var presetName = preset.Attribute("name").Value;
				Logger.Log($"preset: {presetName}");
				var sliders = new Dictionary<string, int>();
				foreach (var slider in preset.Descendants("SetSlider"))
				{
					var value = slider.Attribute("value").Value;
					int intValue;
					int.TryParse(value, out intValue);
					sliders[slider.Attribute("name").Value] = intValue;
				}
				presetList.Add(new PresetModel { Name = $"[{Path.GetFileName(filePath)}] {presetName}", Sliders = sliders });
			}

			return presetList;
		}
	}
}
