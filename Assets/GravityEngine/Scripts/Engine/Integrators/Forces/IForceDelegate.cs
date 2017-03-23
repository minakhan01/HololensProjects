using UnityEngine;
using System.Collections;

public interface IForceDelegate  {

	/// <summary>
	/// Calculate the distance dependent force of two bodies 
	/// seperated by distance r_sep.
	/// (Note: This has taken e.g.  F1 = m1 a1 = (G m1 m2)/r^2
	///  and removes m1 and m2. These are handled in the integrator. 
	///
	/// e.g. for Newtonian (1/R^2) gravity this would be:
	///			m2/(r_sep*r_sep);
	///
	/// </summary>
	/// <returns>The accel.</returns>
	/// <param name="m2">M2.</param>
	/// <param name="r_sep">R sep. The distance between the bodies</param>
	double CalcF(double r_sep); 

	/// <summary>
	/// Calculates the time derivitive of the force law
	/// This function is required only by the Hermite algorithm. If Leapfrog
	/// is used it can be stubbed out. 
	///
	/// e.g. for Newtonian (1/R^2) gravity
	/// </summary>
	/// <returns>The jerk.</returns>
	/// <param name="m2">M2.</param>
	/// <param name="r_sep">R sep.</param>
	double CalcFdot(double r_sep); 

}
