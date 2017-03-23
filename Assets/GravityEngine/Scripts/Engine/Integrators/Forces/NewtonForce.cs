using UnityEngine;
using System.Collections;

/// <summary>
/// Newton force.
/// This is not generally used - since Netwonian gravity is the more efficient default
/// force built in to the integrators. 
///
/// This code is used to double check the force delegate code
/// </summary>
public class NewtonForce : MonoBehaviour, IForceDelegate {

	public double CalcF(double r_sep) {

		return 1.0/(r_sep*r_sep);
	}

	public double CalcFdot(double r_sep) {
		return -2.0/(r_sep*r_sep*r_sep);
	}

}
