using UnityEngine;
using System.Collections;

/// <summary>
/// Custom force.
/// Sample code to show how to make a custom force. To use this
/// set the GE force delegate to custom and attach this script
/// to the object holding the GravityEngine
/// </summary>
public class CustomForce : MonoBehaviour, IForceDelegate  {

	public float a = 2.0f;
	public float b = 1.0f;

	/// <summary>
	/// acceleration = a * ln(b * r)
	/// </summary>
	/// <returns>The accel.</returns>
	/// <param name="m2">M2.</param>
	/// <param name="r_sep">R sep. The distance between the bodies</param>
	public double CalcF(double r_sep) {

		return a*System.Math.Log(b*r_sep);
	}

	public double CalcFdot(double r_sep) {
		return a * b/r_sep;
	}

}
