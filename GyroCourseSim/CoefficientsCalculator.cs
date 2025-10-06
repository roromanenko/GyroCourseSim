namespace GyroCourseSim
{
	public sealed class CoefficientsResult
	{
		public double[] C { get; }
		public double[] A { get; }
		public double[] B { get; }
		public double AlphaBalance { get; }

		public CoefficientsResult(double[] c, double[] a, double[] b, double alphaBalance)
		{
			C = c;
			A = a;
			B = b;
			AlphaBalance = alphaBalance;
		}
	}

	public class CoefficientsCalculator
	{
		public CoefficientsResult ComputeCoefficients(Params flightParams)
		{
			var c = new double[7];
			var a = new double[8];
			var b = new double[8];

			double m = flightParams.G / flightParams.g;

			double q = (flightParams.Rho * flightParams.V * flightParams.V) / 2;
			double CyBal = (2 * flightParams.G) / (flightParams.S * flightParams.Rho * Math.Pow(flightParams.V, 2));
			double alphaBalance = 57.3 * ((CyBal - flightParams.C_y0) / flightParams.C_y_alpha);

			// C coeff
			c[6] = flightParams.V / 57.3;

			//a coeff
			a[1] = -((flightParams.m_y_omega_y * flightParams.Rho * flightParams.V) / (4 * flightParams.Iy)) * (flightParams.S * Math.Pow(flightParams.l, 2));
			a[2] = -((flightParams.m_y_beta * flightParams.Rho * Math.Pow(flightParams.V, 2)) / (2 * flightParams.Iy)) * (flightParams.S * flightParams.l);
			a[3] = -((flightParams.m_y_delta_n * flightParams.Rho * Math.Pow(flightParams.V, 2)) / (2 * flightParams.Iy)) * (flightParams.S * flightParams.l);
			a[4] = -((flightParams.C_z_beta * flightParams.Rho * flightParams.V) / (2 * m)) * flightParams.S;
			a[5] = -((flightParams.m_x_delta_n * flightParams.Rho * Math.Pow(flightParams.V, 2)) / (2 * flightParams.Ix)) * (flightParams.S * flightParams.l);
			a[6] = -((flightParams.m_x_omega_y * flightParams.Rho * flightParams.V) / (4 * flightParams.Ix)) * (flightParams.S * Math.Pow(flightParams.l, 2));
			a[7] = -((flightParams.C_z_delta_n * flightParams.Rho * flightParams.V) / (2 * m)) * flightParams.S;

			//b coeff
			b[1] = -((flightParams.m_x_omega_x * flightParams.Rho * flightParams.V) / (4 * flightParams.Ix)) * (flightParams.S * Math.Pow(flightParams.l, 2));
			b[2] = -((flightParams.m_x_beta * flightParams.Rho * Math.Pow(flightParams.V, 2)) / (2 * flightParams.Ix)) * (flightParams.S * flightParams.l);
			b[3] = -((flightParams.m_x_delta_e * flightParams.Rho * Math.Pow(flightParams.V, 2)) / (2 * flightParams.Ix)) * (flightParams.S * flightParams.l);
			b[4] = (flightParams.g / flightParams.V) * Math.Cos(alphaBalance / 57.3);
			b[5] = -((flightParams.m_y_delta_e * flightParams.Rho * Math.Pow(flightParams.V, 2)) / (2 * flightParams.Iy)) * (flightParams.S * flightParams.l);
			b[6] = -((flightParams.m_y_omega_x * flightParams.Rho * flightParams.V) / (4 * flightParams.Iy)) * (flightParams.S * Math.Pow(flightParams.l, 2));
			b[7] = Math.Sin(alphaBalance / 57.3);

			return new CoefficientsResult(c, a, b, alphaBalance);
		}
	}
}
