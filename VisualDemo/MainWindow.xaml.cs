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
					// todo: log any error
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

		private void clear()
		{
			w1 = null;
			w2 = null;
		}

		private void Ils_BlinkHandled(object sender, System.ComponentModel.HandledEventArgs e)
		{
			// log blinking here
		}

		private void blink(object sender, RoutedEventArgs e)
		{
			ils.SetAnimationTime(100); // base time multiplier 
			ils.SetAnimation(W1, Colors.Red, Colors.Green, 5);
			ils.SetAnimation(W2, Colors.Blue, Colors.Yellow, 3);
		}
	}
}