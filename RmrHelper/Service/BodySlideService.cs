using RmrHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
				_categories = value;
				FindBodySlide();
			}
		}

		Timer UpdateTimer;
		Dictionary<string, int> Updates = new Dictionary<string, int>();
		bool IsUpdating = false;




		public BodySlideService()
		{
			UpdateTimer = new Timer();
			UpdateTimer.Elapsed += UpdateTimer_Elapsed;
		}

		private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			UpdateSliders();
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
			Updates[sliderName] = value;
			if (UpdateTimer != null)
			{
				UpdateTimer.Stop();
				UpdateTimer.Interval = 100;
				UpdateTimer.Start();
			}
		}

		async Task UpdateSliders()
		{
			if (Updates.Count == 0 || IsUpdating) return;
			IsUpdating = true;
			var theUpdates = new Dictionary<string, int>();
			foreach (var kv in Updates)
			{
				theUpdates[kv.Key] = kv.Value;
			}
			Updates.Clear();
			foreach (var kv in theUpdates)
			{
				await UpdateSlider(kv.Key, kv.Value);
				await Task.Delay(50);
			}
			IsUpdating = false;
		}
		async Task UpdateSlider(string sliderName, int value)
		{
			if (SliderScroller == IntPtr.Zero)
			{
				FindBodySlide();
			}
			if (SliderScroller == IntPtr.Zero)
			{
				return;
			}
			if (Sliders.ContainsKey(sliderName) && (!CurrentValues.ContainsKey(sliderName) || CurrentValues[sliderName] != value))
			{
				var slider = Sliders[sliderName];
				PostMessage(slider, WM_LBUTTONDOWN, 1, MakeLParam(10, 10));
				await Task.Delay(2);
				PostMessage(slider, WM_LBUTTONUP, 0, MakeLParam(10, 10));
				SendMessage(slider, WM_SETTEXT, 0, value.ToString());
				await Task.Delay(2);
				PostMessage(slider, WM_KEYDOWN, VK_TAB, 0);
				CurrentValues[sliderName] = value;
			}
		}





		#region pinvoke
		const uint WM_SETTEXT = 0xC;
		const int WM_KEYDOWN = 0x0100;
		const int WM_IME_KEYDOWN = 0x290;
		const int WM_KEYUP = 0x0101;
		const int WM_LBUTTONDOWN = 0x201;
		const int WM_LBUTTONUP = 0x202;

		const int VK_RETURN = 0x0D;
		const int VK_TAB = 0x09;

		public int MakeLParam(int LoWord, int HiWord)
		{
			return (int)((HiWord << 16) | (LoWord & 0xFFFF));
		}


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
