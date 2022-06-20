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
		public List<BodyModel> LoadBodyList(string dirPath)
		{
			var bodyList = new List<BodyModel>();
			if (Directory.Exists(dirPath))
			{
				var files = Directory.GetFiles(dirPath).Where(it => it.ToLowerInvariant().EndsWith(".xml")).ToList();
				foreach (var filePath in files)
				{
					bodyList.Add(new BodyModel { FileName = Path.GetFileName(filePath), SliderCategories = LoadSliderCategories(filePath) });
				}

				bodyList = bodyList.OrderBy(it => it.FileName.ToLowerInvariant()).ToList();
			}

			return bodyList;
		}


		public Dictionary<string, List<SliderModel>> LoadSliderCategories(string filePath)
		{
			var categories = new Dictionary<string, List<SliderModel>>();
			var doc = XDocument.Load(filePath);
			foreach (var group in doc.Descendants("Category"))
			{
				var groupName = group.Attribute("name").Value;
				categories[groupName] = new List<SliderModel>();
				foreach (var slider in group.Descendants("Slider"))
				{
					var sliderName = slider.Attribute("name").Value;
					var sliderDisplayName = slider.Attribute("displayname").Value;
					categories[groupName].Add(new SliderModel { Name = sliderName, DisplayName = sliderDisplayName });
				}
			}

			return categories;
		}
	}
}
