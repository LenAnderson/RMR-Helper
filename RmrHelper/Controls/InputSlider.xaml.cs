using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RmrHelper.Controls
{
    /// <summary>
    /// Interaction logic for InputSlider.xaml
    /// </summary>
    public partial class InputSlider : UserControl
    {
        public static readonly DependencyProperty SliderWidthProperty = DependencyProperty.Register(
            nameof(SliderWidth),
            typeof(int),
            typeof(InputSlider),
            new PropertyMetadata(400)
            );
        public int SliderWidth
        {
            get { return (int)GetValue(SliderWidthProperty); }
            set { SetValue(SliderWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum),
            typeof(int),
            typeof(InputSlider),
            new PropertyMetadata(0)
            );
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(InputSlider),
            new PropertyMetadata(100)
            );
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty TypedMinimumProperty = DependencyProperty.Register(
            nameof(TypedMinimum),
            typeof(int),
            typeof(InputSlider),
            new PropertyMetadata(-999)
            );
        public int TypedMinimum
        {
            get { return (int)GetValue(TypedMinimumProperty); }
            set { SetValue(TypedMinimumProperty, value); }
        }

        public static readonly DependencyProperty TypedMaximumProperty = DependencyProperty.Register(
            nameof(TypedMaximum),
            typeof(int),
            typeof(InputSlider),
            new PropertyMetadata(999)
            );
        public int TypedMaximum
        {
            get { return (int)GetValue(TypedMaximumProperty); }
            set { SetValue(TypedMaximumProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(InputSlider),
            new PropertyMetadata("")
            );
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty AlignmentProperty = DependencyProperty.Register(
            nameof(Alignment),
            typeof(VerticalAlignment),
            typeof(InputSlider),
            new PropertyMetadata(VerticalAlignment.Bottom)
            );
        public VerticalAlignment Alignment
        {
            get { return (VerticalAlignment)GetValue(AlignmentProperty); }
            set { SetValue(AlignmentProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(int),
            typeof(InputSlider),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }




        public InputSlider()
        {
            InitializeComponent();
        }
    }
}
