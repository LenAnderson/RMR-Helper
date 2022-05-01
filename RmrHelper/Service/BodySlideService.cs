using RmrHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.Service
{
	public class BodySlideService
	{
		IntPtr BodySlide;
		IntPtr SliderScroller;
		public Dictionary<string, IntPtr> Sliders = new Dictionary<string, IntPtr>();
		Dictionary<string, int> CurrentValues = new Dictionary<string, int>();

		Dictionary<string, List<SliderModel>> _categories;
		public Dictionary<string, List<SliderModel>> Categories
		{
			get { return _categories; }
			set
			{
				if (_categories != value)
				{
					_categories = value;
					FindBodySlide();
				}
			}
		}




		public BodySlideService()
		{
		}




		void FindBodySlide()
		{
			BodySlide = FindWindow(null, "BodySlide");
			if (BodySlide != IntPtr.Zero)
			{
				SliderScroller = FindWindowEx(BodySlide, IntPtr.Zero, null, "SliderScrollWindow");
				if (SliderScroller != IntPtr.Zero)
				{
					var helper = new BodySlideEnumeratorService();
					Sliders = helper.FindSliderInputs(SliderScroller, Categories);
				}
			}
		}




		public void SetSlider(string sliderName, int value)
		{
			if (SliderScroller == IntPtr.Zero)
			{
				FindBodySlide();
			}
			if (SliderScroller == IntPtr.Zero)
			{
				return;
			}
			if (!CurrentValues.ContainsKey(sliderName) || CurrentValues[sliderName] != value)
			{
				var slider = Sliders[sliderName];
				SendMessage(slider, WM_SETTEXT, 0, value.ToString());
				PostMessage(slider, WM_KEYDOWN, VK_RETURN, 1);
				CurrentValues[sliderName] = value;
			}
		}





		#region pinvoke
		const uint WM_SETTEXT = 0xC;
		const int WM_KEYDOWN = 0x0100;

		const int VK_RETURN = 0x0D;


		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

		[DllImport("User32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);

		[DllImport("User32.Dll")]
		public static extern Int32 PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
		#endregion
	}
}
