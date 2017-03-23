using UnityEngine;
using System.Collections;

public class OrbitUtils  {

	/// <summary>
	/// Calculates the Hill Radius (radius at which the secondary's gravity becomes dominant, when the 
	/// secondary is in orbit around the primary). 
	/// </summary>
	/// <returns>The radius.</returns>
	/// <param name="primary">Primary.</param>
	/// <param name="secondary">Secondary. In orbit around primary</param>
	static public float HillRadius(GameObject primary, GameObject secondary) {

		NBody primaryBody = primary.GetComponent<NBody>(); 
		NBody secondaryBody = secondary.GetComponent<NBody>(); 
		EllipseBase orbit = secondary.GetComponent<EllipseBase>();
		if ((primaryBody == null) || (secondaryBody == null) || (orbit == null)) {
			return 0;
		}
		float denom = 3f*(secondaryBody.mass + primaryBody.mass);
		if (Mathf.Abs(denom) < 1E-6) {
			return 0;
		}
		return Mathf.Pow(secondaryBody.mass/denom, 1/3f) * orbit.p;

	}
}
