using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.ModelGeometry.Scene;
using Xbim.Presentation.LayerStyling;

namespace VisualDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			IfcStore.ModelProviderFactory.UseHeuristicModelProvider();
			myControl.ExcludedTypes.Remove(typeof(Xbim.Ifc2x3.ProductExtension.IfcSpace));
			myControl.ExcludedTypes.Remove(typeof(Xbim.Ifc4.ProductExtension.IfcSpace));
		}

		private void open(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = "IFC Files|*.ifc;*.ifczip;*.ifcxml|Xbim Files|*.xbim";
			dlg.FileOk += (s, args) =>
			{
				LoadXbimFile(dlg.FileName);
			};
			dlg.ShowDialog();
		}

		IndividualElementStyler ils
		{
			get
			{
				return myControl.DefaultLayerStyler as IndividualElementStyler;
			}
		}

		private void LoadXbimFile(string dlgFileName)
		{
			myControl.DefaultLayerStyler = new IndividualElementStyler();
			ils.BlinkHandled += Ils_BlinkHandled;
			var model = IfcStore.Open(dlgFileName);
			if (model.GeometryStore.IsEmpty)
			{
				// Create the geometry using the XBIM GeometryEngine
				try
				{
					var context = new Xbim3DModelContext(model);
					context.CreateContext();
					// TODO: SaveAs(xbimFile); // so we don't re-process every time
				}
				catch (Exception geomEx)
				{
					Console.WriteLine($"ERROR:{geomEx.Message}");
				}
			}
			clear();
			myControl.Model = model;
		}

		IPersistEntity w1;
		public IPersistEntity W1
		{
			get { if (w1 == null) w1 = myControl.Model.Instances.OfType<IIfcWall>().FirstOrDefault(); return w1; }
			set { w1 = value; }
		}

		IPersistEntity w2;
		public IPersistEntity W2
		{
			get { if (w2 == null) w2 = myControl.Model.Instances.OfType<IIfcWall>().Skip(1).FirstOrDefault(); return w2; }
			set { w2 = value; }
		}

		IPersistEntity s1;
		public IPersistEntity S1
		{
			get { if (s1 == null) s1 = myControl.Model.Instances.OfType<IIfcSpace>().Skip(1).FirstOrDefault(); return s1; }
			set { s1 = value; }
		}

		IPersistEntity s2;
		public IPersistEntity S2
		{
			get { if (s2 == null) s2 = myControl.Model.Instances.OfType<IIfcSpace>().FirstOrDefault(); return s2; }
			set { s2 = value; }
		}

		private void clear()
		{
			w1 = null;
			w2 = null;
			s1 = null;
			s2 = null;
		}

		private void Ils_BlinkHandled(object sender, System.ComponentModel.HandledEventArgs e)
		{
			// log blinking here
		}


		//Blinking 
		#region
		private void blink(object sender, RoutedEventArgs e)
		{
			ils.SetAnimationTime(100); // base time multiplier 
			ils.SetAnimation(W1, Colors.Red, Colors.Green, 5);
			ils.SetAnimation(W2, Colors.Blue, Colors.Yellow, 3);
		}

		private void stopBlink(object sender, RoutedEventArgs e)
		{
			ils.RemoveAnimation(W1);
			ils.RemoveAnimation(W2);
			ils.ResetColor(W1);
			ils.ResetColor(W2);
		}

		#endregion

		//Blinking Spaces
		#region
		private void blinkSpace(object sender, RoutedEventArgs e)
		{
			ils.SetAnimationTime(100); // base time multiplier 
			ils.SetAnimation(S1, Colors.Red, Colors.Green, 5);
			ils.SetAnimation(S2, Colors.Blue, Colors.Yellow, 3);
		}

		private void blinkSpace2(object sender, RoutedEventArgs e)
		{
			ils.SetAnimationTime(100); // base time multiplier 
			ils.SetAnimation(S1, Colors.Yellow, Colors.Blue, 5);
			ils.SetAnimation(S2, Colors.Black, Colors.Orange, 3);
		}

		private void stopBlinkingSpace(object sender, RoutedEventArgs e)
		{
			ils.RemoveAnimation(S1);
			ils.RemoveAnimation(S2);
			ils.ResetColor(S1);
			ils.ResetColor(S2);
		}
		#endregion

		//Color Space
		#region
		private void colorSpace(object sender, RoutedEventArgs e)
		{
			ils.RemoveAnimation(S1);
			ils.RemoveAnimation(S2);
			ils.SetColor(S1, Colors.Blue);
			ils.SetColor(S2, Colors.Green);
		}

		private void colorSpace2(object sender, RoutedEventArgs e)
		{
			ils.RemoveAnimation(S1);
			ils.RemoveAnimation(S2);
			ils.SetColor(S1, Colors.Red);
			ils.SetColor(S2, Colors.Black);
		}

		private void resetColorSpace(object sender, RoutedEventArgs e)
		{
			ils.ResetColor(S1);
			ils.ResetColor(S2);
		}
		#endregion

		//Color
		#region
		private void color(object sender, RoutedEventArgs e)
		{
			ils.RemoveAnimation(W1);
			ils.RemoveAnimation(W2);
			ils.SetColor(W1, Colors.Red);
			ils.SetColor(W2, Colors.Green);
		}

		private void color2(object sender, RoutedEventArgs e)
		{
			ils.RemoveAnimation(W1);
			ils.RemoveAnimation(W2);
			ils.SetColor(W1, Colors.Blue);
			ils.SetColor(W2, Colors.Brown);
		}

		private void resetColor(object sender, RoutedEventArgs e)
		{
			ils.ResetColor(W1);
			ils.ResetColor(W2);
		}
		#endregion

		//Color furnitures
		#region
		private void colorFurnituresWithSetColors(object sender, RoutedEventArgs e)
		{
			byte a, r, g, b;
			a = r = g = b = 150;

			var color = System.Windows.Media.Color.FromArgb(a, r, g, b);
			var colorState = new IndividualElementStyler.ColorState(color);

			foreach (IPersistEntity entity in myControl.Model.Instances.OfType<IIfcFurniture>())
			{
				ils.SetColor(entity, colorState);
			}
		}

		private void colorFurnituresWithColor(object sender, RoutedEventArgs e)
		{
			byte a, r, g, b;
			a = r = g = b = 150;

			var color = System.Windows.Media.Color.FromArgb(a, r, g, b);
			var colorState = new IndividualElementStyler.ColorState(color);

			foreach (IPersistEntity entity in myControl.Model.Instances.OfType<IIfcFurniture>())
			{
				ils.SetColor(entity, System.Windows.Media.Color.FromArgb(a, r, g, b));
			}
		}

		private void resetColorFurnitures(object sender, RoutedEventArgs e)
		{
			foreach (IPersistEntity entity in myControl.Model.Instances.OfType<IIfcFurniture>())
			{
				ils.ResetColor(entity);
			}
		}

		#endregion
	}
}