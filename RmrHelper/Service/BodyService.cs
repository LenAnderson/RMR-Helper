using RmrHelper.Helpers;
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
	public class BodyService
	{
		public List<string> Errors = new List<string>();

		public List<BodyModel> LoadBodyList(string dirPath)
		{
			Logger.Log($"BodyService.LoadBodyList: {dirPath}");
			var bodyList = new List<BodyModel>();
			if (Directory.Exists(dirPath))
			{
				var files = Directory.GetFiles(dirPath).Where(it => it.ToLowerInvariant().EndsWith(".xml")).ToList();
				foreach (var filePath in files)
				{
					Logger.Log($"loading: {filePath}");
					bodyList.Add(new BodyModel { FileName = Path.GetFileName(filePath), SliderCategories = LoadSliderCategories(filePath) });
				}

				bodyList = bodyList.OrderBy(it => it.FileName.ToLowerInvariant()).ToList();
			}
            else
            {
				Logger.Log($"directory does not exist: {dirPath}", "WARN");
				Errors.Add($"Directory does not exist: {dirPath}");
            }

			return bodyList;
		}


		public Dictionary<string, List<SliderModel>> LoadSliderCategories(string filePath)
		{
			Logger.Log($"BodyService.LoadSliderCategories: {filePath}");
			var categories = new Dictionary<string, List<SliderModel>>();
			var xmlText = "";
			try
			{
				xmlText = File.ReadAllText(filePath);
				var doc = XDocument.Parse(xmlText);
				foreach (var group in doc.Descendants("Category"))
				{
					var groupName = group.Attribute("name").Value;
					Logger.Log($"category: {groupName}");
					categories[groupName] = new List<SliderModel>();
					foreach (var slider in group.Descendants("Slider"))
					{
						var sliderName = slider.Attribute("name").Value;
						Logger.Log($"slider: {sliderName}");
						var sliderDisplayName = slider.Attribute("displayname").Value;
						categories[groupName].Add(new SliderModel { Name = sliderName, DisplayName = sliderDisplayName });
					}
				}
			}
			catch (Exception ex)
            {
				Errors.Add($"Failed to load {filePath}\n{ex.Message}");
				Logger.Log(ex);
				Logger.Log(xmlText, "ERROR");
            }

			return categories;
		}
	}
}
