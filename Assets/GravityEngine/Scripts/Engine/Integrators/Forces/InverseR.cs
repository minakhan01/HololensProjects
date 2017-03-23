using UnityEngine;
using System.Collections;

public class InverseR : IForceDelegate {

	public double CalcF(double r_sep) {

		return 1.0/r_sep;
	}

	public double CalcFdot(double r_sep) {
		return -1.0/(r_sep*r_sep);
	}
}
