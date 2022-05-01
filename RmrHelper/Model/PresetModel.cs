using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.Model
{
	public class PresetModel
	{
		public string Name { get; set; }
		public Dictionary<string, int> Sliders { get; set; }
	}
}
