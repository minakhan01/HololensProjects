﻿using UnityEngine;
using System.Collections;

public class ForceR2 : IForceDelegate {

	public double CalcF(double r_sep) {

		return r_sep*r_sep;
	}

	public double CalcFdot(double r_sep) {
		return 2.0 * r_sep;
	}
}
