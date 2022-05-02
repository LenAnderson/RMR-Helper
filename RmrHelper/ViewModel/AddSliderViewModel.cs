using RmrHelper.Helpers;
using RmrHelper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.ViewModel
{
	public class AddSliderViewModel : ObservableObject
	{
		public ObservableCollection<Tuple<string, List<SliderCheckbox>>> CategoryList { get; set; } = new ObservableCollection<Tuple<string, List<SliderCheckbox>>>();

		public AddSliderViewModel()
		{
			var l1 = new List<SliderCheckbox>();
			l1.Add(new SliderCheckbox { IsChecked = false, Slider = new SliderModel { DisplayName = "Size", Name = "Breasts" } });
			l1.Add(new SliderCheckbox { IsChecked = false, Slider = new SliderModel { DisplayName = "Fantasy", Name = "SizeFantasy" } });
			var c1 = new Tuple<string, List<SliderCheckbox>>("Boobs", l1);
			CategoryList.Add(c1);
			var l2 = new List<SliderCheckbox>();
			l2.Add(new SliderCheckbox { IsChecked = false, Slider = new SliderModel { DisplayName = "Size", Name = "Butt" } });
			l2.Add(new SliderCheckbox { IsChecked = false, Slider = new SliderModel { DisplayName = "Apple Cheeks", Name = "AppleCheeks" } });
			var c2 = new Tuple<string, List<SliderCheckbox>>("Butt", l2);
			CategoryList.Add(c2);
		}

		public class SliderCheckbox : ObservableObject
		{
			bool _isChecked = false;
			public bool IsChecked
			{
				get { return _isChecked; }
				set
				{
					if (_isChecked != value)
					{
						_isChecked = value;
						OnPropertyChanged(nameof(IsChecked));
					}
				}
			}

			public SliderModel Slider { get; set; }
		}
	}
}
