using RmrHelper.Helpers;
using RmrHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.Service
{
	public class BodySlideEnumeratorService
	{
		string PreviousClass;
		bool InCategoryList = false;
		bool FoundFirstEdit = false;
		string DisplayName;
		List<SliderInput> SliderInputs = new List<SliderInput>();
		List<SliderCategoryTitle> SliderCategoryTitles = new List<SliderCategoryTitle>();

		
		public struct SliderInput
		{
			public string Name;
			public IntPtr Handle;
			public int Top;
		}
		public struct SliderCategoryTitle
		{
			public string Name;
			public int Top;
		}




		public Dictionary<string, IntPtr> FindSliderInputs(IntPtr sliderScroller, Dictionary<string, List<SliderModel>> categories)
		{
			Logger.Log($"BodySlideEnumeratorService.FindSliderInput");
			Dictionary<string, IntPtr> results = new Dictionary<string, IntPtr>();

			EnumChildWindows(sliderScroller, new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);

			SliderCategoryTitles = SliderCategoryTitles.OrderBy(it => it.Top).ToList();
			for (int i = 0; i < SliderCategoryTitles.Count; i++)
			{
				var category = SliderCategoryTitles[i];
				if (categories?.ContainsKey(category.Name) ?? false)
				{
					var top = category.Top;
					List<SliderInput> inputs;
					if (i+1 < SliderCategoryTitles.Count)
					{
						var next = SliderCategoryTitles[i + 1].Top;
						inputs = SliderInputs.Where(it => it.Top > top && it.Top < next).ToList();
					}
					else
					{
						inputs = SliderInputs.Where(it => it.Top > top).ToList();
					}

					foreach (var slider in categories[category.Name])
					{
						if (inputs.Any(it => it.Name == slider.DisplayName))
						{
							results[slider.Name] = inputs.First(it => it.Name == slider.DisplayName).Handle;
						}
					}
				}
			}

			return results;
		}




		bool EnumWindowsCallback(int handle, int lParam)
		{
			StringBuilder classNameBuilder = new StringBuilder(256);
			GetClassName(handle, classNameBuilder, classNameBuilder.Capacity);
			StringBuilder textBuilder = new StringBuilder(256);
			SendMessage(handle, WM_GETTEXT, textBuilder.Capacity, textBuilder);

			var className = classNameBuilder.ToString();
			var text = textBuilder.ToString();

			var handlePointer = new IntPtr(handle);

			if (!InCategoryList)
			{
				if (className == "Edit")
				{
					if (!FoundFirstEdit)
					{
						FoundFirstEdit = true;
					}
					else
					{
						FoundFirstEdit = false;
						RECT windowRect;
						GetWindowRect(handlePointer, out windowRect);
						SliderInputs.Add(new SliderInput { Name = DisplayName, Handle = handlePointer, Top = windowRect.Top });
					}
				}
				else if (className == "Static" && FoundFirstEdit)
				{
					DisplayName = text;
				}
				else if (PreviousClass == "Edit" && className == "Button")
				{
					InCategoryList = true;
				}
			}
			else if (className == "Static")
			{
				RECT windowRect;
				GetWindowRect(handlePointer, out windowRect);
				SliderCategoryTitles.Add(new SliderCategoryTitle { Name = text, Top = windowRect.Top });
			}

			PreviousClass = className;

			return true;
		}




		#region pinvoke
		const int WM_GETTEXT = 0xD;


		private delegate bool EnumWindowsProc(int hWnd, int lParam);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr lParam);


		[DllImport("User32.Dll")]
		public static extern void GetClassName(int hWnd, StringBuilder s, int nMaxCount);
		
		[DllImport("User32.dll")]
		public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		#endregion
	}

}
