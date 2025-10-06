using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GyroCourseSim;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GyroCourseSim.UI
{
	public partial class MainViewModel : ObservableObject
	{
		[ObservableProperty]
		private SimulationParameters _parameters;

		[ObservableProperty]
		private string _statusMessage = "Готов к запуску";

		[ObservableProperty]
		private double _alphaBalance;

		[ObservableProperty]
		private bool _isRunning;

		[ObservableProperty]
		private bool _showGamma = true;

		[ObservableProperty]
		private bool _showGammaDot = false;

		[ObservableProperty]
		private bool _showPsi = false;

		[ObservableProperty]
		private bool _showPsiDot = false;

		[ObservableProperty]
		private bool _showBeta = false;

		[ObservableProperty]
		private bool _showDe = false;

		[ObservableProperty]
		private bool _showDn = false;

		public ObservableCollection<ISeries> CombinedSeries { get; set; }

		public Axis[] XAxes { get; set; }
		public Axis[] YAxesCombined { get; set; }

		// Храним данные результата для обновления графиков
		private SimulationResult? _lastResult;

		public MainViewModel()
		{
			_parameters = new SimulationParameters();

			// Инициализация серий данных
			CombinedSeries = new ObservableCollection<ISeries>();

			// Подписка на изменения чекбоксов
			PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(ShowGamma) ||
					e.PropertyName == nameof(ShowGammaDot) ||
					e.PropertyName == nameof(ShowPsi) ||
					e.PropertyName == nameof(ShowPsiDot) ||
					e.PropertyName == nameof(ShowBeta) ||
					e.PropertyName == nameof(ShowDe) ||
					e.PropertyName == nameof(ShowDn))
				{
					if (_lastResult != null)
					{
						UpdateCharts(_lastResult);
					}
				}
			};

			// Настройка осей
			XAxes = new Axis[]
			{
				new Axis
				{
					Name = "Время (с)",
					NamePaint = new SolidColorPaint(SKColors.Black),
					LabelsPaint = new SolidColorPaint(SKColors.Black),
					TextSize = 12,
					Labeler = value => $"{value:F2}"
				}
			};

			YAxesCombined = new Axis[]
			{
				new Axis
				{
					Name = "Значение",
					NamePaint = new SolidColorPaint(SKColors.Black),
					LabelsPaint = new SolidColorPaint(SKColors.Black),
					TextSize = 12,
					Labeler = value => $"{value:F4}"
				}
			};
		}

		[RelayCommand]
		private async Task RunSimulation()
		{
			IsRunning = true;
			StatusMessage = "Выполняется симуляция...";

			try
			{
				await Task.Run(() =>
				{
					// Создаём параметры с пользовательским k_gamma_psi
					var simParams = new Params
					{
						k_gamma_psi = Parameters.KGammaPsi
					};

					// Создание калькулятора коэффициентов
					var coeffCalculator = new CoefficientsCalculator();

					// Создание и запуск симуляции
					var simulation = new Simulation(coeffCalculator, simParams);
					var result = simulation.Run(
						tEnd: Parameters.TEnd,
						dt: Parameters.Dt,
						mode: Parameters.Mode,
						de_zad: Parameters.DeZad,
						dn_zad: Parameters.DnZad,
						deviation_time: Parameters.DeviationTime,
						dempf: Parameters.Dempf,
						integrationMode: Parameters.IntegrationMode
					);

					// Обновление UI в главном потоке
					Application.Current.Dispatcher.Invoke(() =>
					{
						_lastResult = result;
						UpdateCharts(result);
						AlphaBalance = result.AlphaBalance;

						string modeText = Parameters.Mode == 1 ? "Автоматическое" : "Свободный самолёт";
						string dempfText = Parameters.Dempf == 1 ? "включён" : "отказ";
						string integrationText = Parameters.IntegrationMode == 1 ? "Эйлер" : "Рунге-Кутта 2";
						StatusMessage = $"Симуляция завершена. Режим: {modeText}. Демпфер: {dempfText}. Метод: {integrationText}. α_balance = {result.AlphaBalance:F4}";
					});
				});
			}
			catch (Exception ex)
			{
				StatusMessage = $"Ошибка: {ex.Message}";
			}
			finally
			{
				IsRunning = false;
			}
		}

		private void UpdateCharts(SimulationResult result)
		{
			// Очистка всех серий
			CombinedSeries.Clear();

			// Добавляем только выбранные графики
			if (ShowGamma)
			{
				var gammaPoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.Gamma[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = gammaPoints,
					Name = "γ (крен)",
					Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}

			if (ShowGammaDot)
			{
				var gammaDotPoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.GammaDot[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = gammaDotPoints,
					Name = "γ' (ω_x)",
					Stroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}

			if (ShowPsi)
			{
				var psiPoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.Psi[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = psiPoints,
					Name = "ψ (курс)",
					Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}

			if (ShowPsiDot)
			{
				var psiDotPoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.PsiDot[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = psiDotPoints,
					Name = "ψ' (ω_y)",
					Stroke = new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}

			if (ShowBeta)
			{
				var betaPoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.Beta[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = betaPoints,
					Name = "β (скольжение)",
					Stroke = new SolidColorPaint(SKColors.Purple) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}

			if (ShowDe)
			{
				var dePoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.De[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = dePoints,
					Name = "δₑ (элероны)",
					Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}

			if (ShowDn)
			{
				var dnPoints = result.Time.Select((t, i) => new LiveChartsCore.Defaults.ObservablePoint(t, result.Dn[i])).ToList();
				CombinedSeries.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
				{
					Values = dnPoints,
					Name = "δₙ (руль направления)",
					Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 },
					Fill = null,
					GeometrySize = 0,
					GeometryStroke = null,
					LineSmoothness = 0
				});
			}
		}

		[RelayCommand]
		private void ClearCharts()
		{
			CombinedSeries.Clear();
			_lastResult = null;
			AlphaBalance = 0;
			StatusMessage = "Графики очищены";
		}
	}
}