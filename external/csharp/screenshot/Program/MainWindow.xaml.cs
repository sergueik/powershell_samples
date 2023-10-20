using System.Windows;
using Utils;
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

namespace ScreenshotDemo
{
	
	public partial class MainWindow
	{
		Point point1 = new Point();
		Point point2 = new Point();
		public MainWindow()
		{
			InitializeComponent();
		}

		private void CaptureRegion_OnClick(object sender, RoutedEventArgs e){
			System.Windows.Media.ImageBrush ib = new ImageBrush(Screenshot.CaptureRegion());
			canvas.Background = ib;
		}
		    
		private void canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e){
		
			point1 = e.GetPosition(canvas);
		}

		private void canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			point2 = e.GetPosition(canvas);
			if (canvas.Children.Count > 0) {
				canvas.Children.RemoveAt(0);
			}
			var rectangle = new Rectangle();
			rectangle.Stroke = System.Windows.Media.Brushes.GreenYellow;
			rectangle.Width = point2.X - point1.X;
			rectangle.Height = point2.Y - point1.Y;
			rectangle.StrokeThickness = 2;

			canvas.Children.Add(rectangle);
			Canvas.SetLeft(rectangle, point1.X);
			Canvas.SetTop(rectangle, point1.Y);

		}
	}
	
}