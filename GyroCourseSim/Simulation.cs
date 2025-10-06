namespace GyroCourseSim
{
	public sealed class SimulationResult
	{
		public List<double> Time { get; }
		public List<double> Gamma { get; }
		public List<double> GammaDot { get; }
		public List<double> Psi { get; }
		public List<double> PsiDot { get; }
		public List<double> Beta { get; }
		public List<double> De { get; }
		public List<double> Dn { get; }

		public double AlphaBalance { get; }

		public SimulationResult(List<double> time, List<double> gamma, List<double> gammaDot, List<double> psi, List<double> psiDot, List<double> beta, List<double> de, List<double> dn, double alphaBalance)
		{
			Time = time;
			Gamma = gamma;
			GammaDot = gammaDot;
			Psi = psi;
			PsiDot = psiDot;
			Beta = beta;
			De = de;
			Dn = dn;
			AlphaBalance = alphaBalance;
		}
	}

	public class Simulation
	{
		private readonly CoefficientsCalculator _coeff;
		private readonly Params _parameters;

		public Simulation(CoefficientsCalculator coeff, Params parameters)
		{
			_coeff = coeff;
			_parameters = parameters;
		}

		public SimulationResult Run(double tEnd, double dt, int mode, double de_zad, double dn_zad, double deviation_time, int dempf, int integrationMode)
		{
			// Result lists
			var time = new List<double>();
			var gamma = new List<double>();
			var gammaDot = new List<double>();
			var psi = new List<double>();
			var psiDot = new List<double>();
			var beta = new List<double>();
			var de = new List<double>();
			var dn = new List<double>();

			// Var arrays
			double[] x = new double[10];
			double[] y = new double[10];

			// Coeffs
			var coeffs = _coeff.ComputeCoefficients(_parameters);
			var c = coeffs.C;
			var a = coeffs.A;
			var b = coeffs.B;
			var alphaBalance = coeffs.AlphaBalance;

			// δ
			double DE = 0.0;
			double DN = 0.0;

			double psiG0 = 0;
			double psiGZad = psiG0 + 10;

			double h = 0.05;

			for (double t = 0; t < tEnd;)
			{
				// F_Δ(Ψ_zad)
				double FDeltaPsi = y[6] - psiGZad;
				FDeltaPsi = Math.Clamp(FDeltaPsi, -14, 14);

				if (mode == 1)
				{
					// δ_e
					DE = x[8] + _parameters.k_gamma * (y[2] - y[7]);
					DE = Math.Clamp(DE, -12, 12);

					// δ_e
					if (dempf == 0)
					{
						DN = 0;
					}
					else
					{
						DN = x[9];
						DN = Math.Clamp(DN, -10, 10);
					}
				}
				else
				{
					// δ_e
					if (t < deviation_time)
					{
						DE = de_zad;
					}
					else
					{
						DE = 0;
					}

					// δ_n
					if (t < deviation_time)
					{
						DN = dn_zad;
					}
					else
					{
						DN = 0;
					}
				}

				x[0] = y[1];                                                                                                        // Ψ` Psi`
				x[1] = -a[1] * x[0] - b[6] * x[2] - a[2] * y[4] - a[3] * DN - b[5] * DE;                                            // Ψ`` Psi``
				x[2] = y[3];                                                                                                        // Gamma`
				x[3] = -b[1] * x[2] - a[6] * x[0] - b[2] * y[4] - a[5] * DN - b[3] * DE;                                            // Gamma``
				x[4] = x[0] + b[4] * y[2] + b[7] * x[2] - a[4] * y[4] - a[7] * DN;                                                  // Beta`
				x[5] = -c[6] * (y[0] - y[4]);                                                                                       // Z`
				x[6] = (-y[0] / _parameters.T_psi) - (y[6] / _parameters.T_psi);                                                    // Psi_g
				x[7] = ((-FDeltaPsi * _parameters.k_gamma_psi) / _parameters.T_gamma_psi) - (y[7] / _parameters.T_gamma_psi);       // Gamma_zad
				x[8] = _parameters.k_omega_x * y[3] - y[8] / _parameters.T_omega_x;                                                 // De
				x[9] = _parameters.k_omega_y * y[1] - y[9] / _parameters.T_omega_y;                                                 // Dn

				//Integration
				if (integrationMode == 1)
				{
					y = EulerIntegration(x, y, dt);
				}
				else
				{
					y = RungeKutta(x, y, h, a, b, c, DN, DE, _parameters, FDeltaPsi);
				}

				//Results
				gamma.Add(y[2]);
				gammaDot.Add(x[2]);
				psi.Add(y[0]);
				psiDot.Add(x[0]);
				beta.Add(y[4]);
				de.Add(DE);
				dn.Add(DN);
				time.Add(t);

				t += dt;
			}

			return new SimulationResult(time, gamma, gammaDot, psi, psiDot, beta, de, dn, alphaBalance);
		}

		public static double[] EulerIntegration(double[] x, double[] y, double dt)
		{
			for (int k = 0; k < x.Length; k++)
			{
				y[k] += x[k] * dt;
			}

			return y;
		}

		public static double[] RungeKutta(double[] x, double[] y, double h, double[] a, double[] b, double[] c, double DN, double DE, Params parameters, double FDeltaPsi)
		{

			y[0] += (y[1] + h / 2 * x[0]) * h;
			y[1] += (-a[1] * (x[0] + h / 2) - b[6] * (x[2] + h / 2) - a[2] * (y[4] + h / 2 * x[4]) - a[3] * DN - b[5] * DE) * h;
			y[2] += (y[3] + h / 2 * x[2]) * h;
			y[3] += (-b[1] * (x[2] + h / 2) - a[6] * (x[0] + h / 2) - b[2] * (y[4] + h / 2 * x[4]) - a[5] * DN - b[3] * DE) * h;
			y[4] += ((x[0] + h / 2) + b[4] * (y[2] + h / 2 * x[2]) + b[7] * (x[2] + h / 2) - a[4] * (y[4] + h / 2 * x[4]) - a[7] * DN) * h;
			y[5] += (-c[6] * ((y[0] + h / 2 * x[0]) - (y[4] + h / 2 * x[4]))) * h;
			y[6] += ((-(y[0] + h / 2 * x[0]) / parameters.T_psi) - ((y[6] + h / 2 * x[6]) / parameters.T_psi)) * h;
			y[7] += (((-FDeltaPsi * parameters.k_gamma_psi) / parameters.T_gamma_psi) - ((y[7] + h / 2 * x[7]) / parameters.T_gamma_psi)) * h;
			y[8] += (parameters.k_omega_x * (y[3] + h / 2 * x[3]) - (y[8] + h / 2 * x[8]) / parameters.T_omega_x) * h;
			y[9] += (parameters.k_omega_y * (y[1] + h / 2 * x[1]) - (y[9] + h / 2 * x[9]) / parameters.T_omega_y) * h;

			return y;
		}
	}
}
