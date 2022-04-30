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
		Dictionary<string, IntPtr> Sliders = new Dictionary<string, IntPtr>();

		Dictionary<string, List<Slider>> Categories;




		public BodySlideService(Dictionary<string, List<Slider>> categories)
		{
			Categories = categories;
			FindBodySlide();
		}




		void FindBodySlide()
		{
			BodySlide = FindWindow(null, "BodySlide");
			if (BodySlide != null)
			{
				SliderScroller = FindWindowEx(BodySlide, IntPtr.Zero, null, "SliderScrollWindow");
				IntPtr x;

				List<IntPtr> childHandles = new List<IntPtr>();
				GCHandle gcHandleChildHandles = GCHandle.Alloc(childHandles);
				IntPtr pointerChildHandles = GCHandle.ToIntPtr(gcHandleChildHandles);
				EnumChildWindows(SliderScroller, new EnumWindowsProc(EnumWindowsCallback), pointerChildHandles);

				categoryOrder = categoryOrder.OrderBy(it => it.Item2).ToList();
				for (int i = 0; i < categoryOrder.Count; i++)
				{
					if (Categories.ContainsKey(categoryOrder[i].Item1))
					{
						var top = categoryOrder[i].Item2;
						List<Tuple<string, IntPtr, int>> items;
						if (i + 1 < categoryOrder.Count)
						{
							var next = categoryOrder[i + 1].Item2;
							items = sliders.Where(it => it.Item3 > top && it.Item3 < next).ToList();

						}
						else
						{
							items = sliders.Where(it => it.Item3 > top).ToList();
						}
						foreach (var slider in Categories[categoryOrder[i].Item1])
						{
							var item = items.FirstOrDefault(it => it.Item1 == slider.DisplayName);
							if (item != null)
							{
								Sliders[slider.Name] = item.Item2;
							}
						}
					}
				}

				//x = FindWindowEx(SliderScroller, IntPtr.Zero, null, "Breasts");
				//x = FindWindowEx(SliderScroller, x, null, "Size");
				//x = FindWindowEx(SliderScroller, x, null, "Size");
				//Sliders["Breasts"] = x;

				//x = FindWindowEx(SliderScroller, IntPtr.Zero, null, "Breasts");
				//x = FindWindowEx(SliderScroller, x, null, "Fantasy");
				//x = FindWindowEx(SliderScroller, x, null, "Fantasy");
				//Sliders["BreastsFantasy"] = x;

				//x = FindWindowEx(SliderScroller, IntPtr.Zero, null, "Butt");
				//x = FindWindowEx(SliderScroller, x, null, "Size");
				//x = FindWindowEx(SliderScroller, x, null, "Size");
				//Sliders["Butt"] = x;
			}
		}

		string prevClass;
		bool inCategoryList = false;
		bool second = false;
		string displayName;
		List<Tuple<string, IntPtr, int>> sliders = new List<Tuple<string, IntPtr, int>>();
		List<Tuple<string, int>> categoryOrder = new List<Tuple<string, int>>();

		bool EnumWindowsCallback(int handle, int lParam)
		{
			StringBuilder className = new StringBuilder(256);
			GetClassName(handle, className, className.Capacity);
			StringBuilder text = new StringBuilder(256);
			SendMessage(handle, WM_GETTEXT, text.Capacity, text);

			string x = className.ToString();
			string y = text.ToString();
			if (x == "Edit")
			{
				if (!second)
				{
					second = true;
				}
				else
				{
					second = false;
					RECT r;
					GetWindowRect(new IntPtr(handle), out r);
					sliders.Add(new Tuple<string, IntPtr, int>(displayName, new IntPtr(handle), r.Top));
				}
			}
			else if (x == "Static" && second)
			{
				displayName = y;
			}
			if (prevClass == "Edit" && x == "Button")
			{
				inCategoryList = true;
			}
			else if (inCategoryList && x == "Static")
			{
				var r = new RECT();
				GetWindowRect(new IntPtr(handle), out r);
				categoryOrder.Add(new Tuple<string, int>(y, r.Top));
			}
			prevClass = x;
			return true;
		}




		public void SetSlider(string sliderName, int value)
		{
			if (SliderScroller == null)
			{
				FindBodySlide();
			}
			if (SliderScroller == null)
			{
				return;
			}
			var slider = Sliders[sliderName];
			//var edit = FindWindowEx(SliderScroller, slider, "Edit", null);
			SendMessage(slider, WM_SETTEXT, 0, value.ToString());
			PostMessage(slider, WM_KEYDOWN, VK_RETURN, 1);
		}





		#region pinvoke
		const uint WM_SETTEXT = 0xC;
		const int WM_GETTEXT = 0xD;
		const int WM_GETTEXTLENGTH = 0xE;
		const int WM_KEYDOWN = 0x0100;

		const int VK_RETURN = 0x0D;


		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

		private delegate bool EnumWindowsProc(int hWnd, int lParam);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr lParam);

		[DllImport("User32.Dll")]
		public static extern void GetClassName(int hWnd, StringBuilder s, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

		[DllImport("User32.dll")]
		public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);

		[DllImport("User32.Dll")]
		public static extern Int32 PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref System.Windows.Rect lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		#endregion
	}




	public class Slider
	{
		public string Name;
		public string DisplayName;
	}
}
