using UnityEngine;
using System.Collections;

public class NUtils  {

	private const int NDIM = 3; 

	/// <summary>
	/// Calculate the energy of the system
	/// Used internally by GravityEngine. Use GravityEngine.GetEnergy() from developer scripts. 
	/// </summary>
	/// <returns>The energy.</returns>
	/// <param name="numBodies">Number bodies.</param>
	/// <param name="m">M.</param>
	/// <param name="r">The red component.</param>
	/// <param name="v">V.</param>
	public static double GetEnergy(int numBodies, ref double[] m, ref double[,] r, ref double[,] v) {
		// If any bodies have gone from active -> inactive this won't be meaningful
		double epot = 0.0; 
		double ekin = 0.0; 
		double[] rji = new double[NDIM]; 
		double r2; 
		for (int i=0; i < numBodies; i++) {
			for (int j=i+1; j < numBodies; j++) {
				for (int k=0; k < NDIM; k++) {
					rji[k] = r[j,k] - r[i,k];
				}
				r2 = 0; 
				for (int k=0; k < NDIM; k++) {
					r2 += rji[k] * rji[k]; 
				}
				epot -= m[i] * m[j]/System.Math.Sqrt(r2);
			}
			for (int k=0; k < NDIM; k++) {
				ekin += 0.5 * m[i] * v[i,k]*v[i,k];
			}
		}	
		return ekin + epot; 	
	}

	public static float GaussianValue(float mean, float stdDev) {
		// Box-Mueller from StackOverflow
		//Random rand = new Random(); //reuse this if you are generating many
		float u1 = Random.value; //these are uniform(0,1) random doubles
		float u2 = Random.value;
		float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
             Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
		float randNormal =
             mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
	}

	/// <summary>
	/// Given the Sin and Cos of an angle return the correct value in 0..2 PI radians.
	/// </summary>
	/// <returns>The from sin cos.</returns>
	/// <param name="sinValue">Sin value.</param>
	/// <param name="cosValue">Cos value.</param>
	public static float AngleFromSinCos(float sinValue, float cosValue) {
		// may get numerical issues and exceed -1..1
	
		float value = 0f; 
		if (sinValue > 1f)
			sinValue = 1f;
		if (sinValue < -1f)
			sinValue = -1f;
		if (cosValue > 1f)
			cosValue = 1f;
		if (cosValue < -1f)
			cosValue = -1f;

		// determine the correct quadrant for Omega
		if (sinValue >= 0 && cosValue >= 0) {
			value = Mathf.Asin(sinValue);
		} else if (sinValue < 0 && cosValue >= 0) {
			value = 2f * Mathf.PI - Mathf.Acos(cosValue);
		} else if (sinValue >= 0 && cosValue < 0) {
			value = Mathf.PI - Mathf.Asin(sinValue);
		} else if (sinValue < 0 && cosValue < 0) {
			value = Mathf.PI + Mathf.Asin(-sinValue);
		} else {
			// sinOmega == cosOmega == 0 (impossible)
		}
		return value;

	}

	/// <summary>
	/// Degress modules 360f (angle in 0..360)
	/// </summary>
	/// <returns>The mod360.</returns>
	/// <param name="angle">Angle.</param>
	public static float DegressMod360(float angle) {
		int numCycles = (int)angle/360;
		float result = ( angle/360f - numCycles) * 360f;
		if (result < 0)
			result += 360f;
		return result;
	}

	public static bool VectorNaN(Vector3 v) {
		return System.Single.IsNaN(v.x) ||
			System.Single.IsNaN(v.y) ||
			System.Single.IsNaN(v.z);
	}

}
