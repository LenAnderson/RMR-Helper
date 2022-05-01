using RmrHelper.Helpers;
using RmrHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.Model
{
	public class BodyModel
	{
		public string FileName { get; set; }
		public Dictionary<string, List<SliderModel>> SliderCategories { get; set; }
	}
}
