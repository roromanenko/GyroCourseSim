namespace GyroCourseSim
{
	public class Params
	{
		//Aircraft geometry and inertia
		public double S { get; init; } = 201.45;        //S, m^2
		public double l { get; init; } = 37.55;         //l, m
		public double ba { get; init; } = 5.285;        // MAC, m
		public double Xt { get; init; } = 0.24;         // x̄_t, %
		public double G { get; init; } = 73000;         //G, kg
		public double Ix { get; init; } = 170000;       //Ix, kg m s^2
		public double Iy { get; init; } = 800000;       //Iy, kg m s^2
		public double Iz { get; init; } = 660000;       //Iz, kg m s^2

		// Flight conditions
		public double V { get; init; } = 97.2;          // V0, m/s
		public double H { get; init; } = 500;           // H0, m
		public double Rho { get; init; } = 0.1190;      // ρ, кг·с^2/м^4
		public double a { get; init; } = 338.36;        // aH, m/s
		public double g { get; init; } = 9.81;          // g, m/s^2

		// Aerodynamic coefficients
		public double C_z_beta { get; init; } = -0.8136;
		public double C_z_delta_n { get; init; } = -0.16;
		public double C_y_alpha { get; init; } = 5.78;
		public double C_y0 { get; init; } = -0.255;

		// Pitching moment
		public double m_y_omega_y { get; init; } = -0.141;
		public double m_y_beta { get; init; } = -0.1518;
		public double m_y_delta_n { get; init; } = -0.0710;
		public double m_x_delta_n { get; init; } = -0.02;
		public double m_x_omega_y { get; init; } = -0.151;
		public double m_x_omega_x { get; init; } = -0.56;
		public double m_x_beta { get; init; } = -0.1146;
		public double m_x_delta_e { get; init; } = -0.07;
		public double m_y_delta_e { get; init; } = 0.0;
		public double m_y_omega_x { get; init; } = 0.026;

		public double P1_delta_g { get; init; } = 7003.0;
		public double P1_V { get; init; } = -13.8;

		// Control law params1
		public double k_omega_x { get; init; } = 1.5;
		public double k_omega_y { get; init; } = 2.5;
		public double T_omega_x { get; init; } = 1.6;
		public double T_omega_y { get; init; } = 2.5;
		public double k_gamma { get; init; } = 2;

		// Control law params2
		public double k_gamma_psi { get; init; } = 2;
		public double T_gamma_psi { get; init; } = 0.1;
		public double k_psi { get; init; } = 1.0;
		public double T_psi { get; init; } = 0.1;
	}
}
