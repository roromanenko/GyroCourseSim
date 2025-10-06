using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroCourseSim.UI
{
	public class SimulationParameters : INotifyPropertyChanged
	{
		private double _tEnd = 40;
		private double _dt = 0.01;
		private int _mode = 1; // 1 - автоматическое, 0 - свободный
		private double _deZad = 0.0;
		private double _dnZad = 0.0;
		private double _deviationTime = 1.0;
		private int _dempf = 1; // 1 - демпфер включен, 0 - отказ демпфера
		private int _integrationMode = 1; // 1 - Эйлер, 2 - Рунге-Кутта
		private double _kGammaPsi = 2.0; // Коэффициент k_gamma_psi

		public double TEnd
		{
			get => _tEnd;
			set { _tEnd = value; OnPropertyChanged(); }
		}

		public double Dt
		{
			get => _dt;
			set { _dt = value; OnPropertyChanged(); }
		}

		public int Mode
		{
			get => _mode;
			set { _mode = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAutomaticMode)); OnPropertyChanged(nameof(IsFreeMode)); }
		}

		public bool IsAutomaticMode
		{
			get => _mode == 1;
			set { Mode = value ? 1 : 0; }
		}

		public bool IsFreeMode
		{
			get => _mode == 0;
			set { Mode = value ? 0 : 1; }
		}

		public double DeZad
		{
			get => _deZad;
			set { _deZad = value; OnPropertyChanged(); }
		}

		public double DnZad
		{
			get => _dnZad;
			set { _dnZad = value; OnPropertyChanged(); }
		}

		public double DeviationTime
		{
			get => _deviationTime;
			set { _deviationTime = value; OnPropertyChanged(); }
		}

		public int Dempf
		{
			get => _dempf;
			set { _dempf = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsDempferEnabled)); OnPropertyChanged(nameof(IsDempferFailed)); }
		}

		public bool IsDempferEnabled
		{
			get => _dempf == 1;
			set { Dempf = value ? 1 : 0; }
		}

		public bool IsDempferFailed
		{
			get => _dempf == 0;
			set { Dempf = value ? 0 : 1; }
		}

		public int IntegrationMode
		{
			get => _integrationMode;
			set { _integrationMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEulerMethod)); OnPropertyChanged(nameof(IsRungeKuttaMethod)); }
		}

		public bool IsEulerMethod
		{
			get => _integrationMode == 1;
			set { IntegrationMode = value ? 1 : 2; }
		}

		public bool IsRungeKuttaMethod
		{
			get => _integrationMode == 2;
			set { IntegrationMode = value ? 2 : 1; }
		}

		public double KGammaPsi
		{
			get => _kGammaPsi;
			set { _kGammaPsi = value; OnPropertyChanged(); }
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}