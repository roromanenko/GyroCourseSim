using System.Windows;

namespace GyroCourseSim.UI
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void SelectAllGraphs(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as MainViewModel;
			if (vm != null)
			{
				vm.ShowGamma = true;
				vm.ShowGammaDot = true;
				vm.ShowPsi = true;
				vm.ShowPsiDot = true;
				vm.ShowBeta = true;
				vm.ShowDe = true;
				vm.ShowDn = true;
			}
		}

		private void DeselectAllGraphs(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as MainViewModel;
			if (vm != null)
			{
				vm.ShowGamma = false;
				vm.ShowGammaDot = false;
				vm.ShowPsi = false;
				vm.ShowPsiDot = false;
				vm.ShowBeta = false;
				vm.ShowDe = false;
				vm.ShowDn = false;
			}
		}
	}
}