using UnityEngine;
using System.Collections;

public class ForceR : IForceDelegate {

	public double CalcF(double r_sep) {

		return r_sep;
	}

	public double CalcFdot(double r_sep) {
		return 1.0;
	}
}
