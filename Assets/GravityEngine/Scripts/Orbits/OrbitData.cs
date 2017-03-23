using UnityEngine;
using System.Collections;

/// <summary>
/// Orbit data.
/// Hold the traditional orbit parameters for an elliptic/hyperbolic orbit. 
///
/// Provide utility methods to derive the orbit parameters from the position, velocity and centerBody
/// of an NBody object pair. This orbit prediction is based only on the two bodies (and assumes the central
/// body mass dominates) - the influence of other bodies in the scene may result in an actual path that is
/// different from the predicted path. 
/// </summary>
[System.Serializable]
public class OrbitData  {

	// Orbit parameters (user control via FixedEllipseEditor)
	// These parameters are in world space. 
	//! eccentricity (0..1, 0=circle, 1=linear)
	public float ecc; 			

	// Allow EITHER a or p to specify size (JPL data uses a for planets and p for comets, so both are useful)
	//! semi-major axis - based on paramBy user can specify a OR p. a = p/(1-ecc)
	public float a = 10f; 			
	//! perihelion 
	public float perihelion; 			
	//! "longitude of ascending node" - angle from x-axis to line from focus to pericenter
	public float omega_uc; 		
	//! "argument of perienter" - angle from ascending node to pericenter
	public float omega_lc; 		
	//! inclination (degrees!)
	public float inclination; 	
	//! initial TRUE anomoly (angle wrt line from focus to closest approach)
	public float phase; 	

	//! period of orbit (currently only in dimensionless units)
	public float period; 

	//! time to periapsis (point of closest approach)
	public float tau; 	

	//! Hyperbola - initial distance from focus
	public float r_initial = 10f; 	

	/// <summary>
	/// Computes the orbit parameters for a specified velocity with respect to a central body.
	/// </summary>
	/// <param name="vel">Vel.</param>
	public void SetOrbitForVelocity(NBody forNbody, NBody aroundNBody) {
		float velSq = forNbody.vel_scaled.sqrMagnitude;
		Vector3 r_vec = forNbody.transform.position - aroundNBody.transform.position;
		float r = Vector3.Magnitude(r_vec);
		float mu = aroundNBody.mass;
		if ( 2/r > velSq/mu) {
			EllipseOrbitForVelocity(forNbody, aroundNBody);
		} else {
			HyperOrbitForVelocity(forNbody, aroundNBody);
		}
//		Debug.Log(string.Format("SetOrbit: a={0} perih={1} e={2} i={3} Omega={4} omega={5} r_initial={6}", 
//						a, perihelion, ecc, inclination, omega_uc, omega_lc, r_initial));
	}

	private void EllipseOrbitForVelocity(NBody forNbody, NBody aroundNBody) {
		// Murray and Dermott Ch2.8 (2.134) - (2.140)
		Vector3 vel = forNbody.vel_scaled - aroundNBody.vel_scaled;
		float velSq = vel.sqrMagnitude;

		Vector3 r_vec = forNbody.transform.position - aroundNBody.transform.position;
		float r = Vector3.Magnitude(r_vec);
		float mu = aroundNBody.mass;
		// semi-major axis (2.134)
		a = 1f/(2/r - velSq/mu);

		// Determine angular momentum, h
		Vector3 h_vec = Vector3.Cross(r_vec, forNbody.vel_scaled);
		float h = Vector3.Magnitude(h_vec);
		//Debug.Log("h_vec = " + h_vec + " r_vec=" + r_vec + " v=" + forNbody.vel + " planet pos=" + forNbody.transform.position);

		// eccentricity (2.135)
		float eqn1 = h*h/(mu*a);
		if (eqn1 > 1f) {
			// Can be slightly bigger than 1 due to numerical error
			ecc = 0f; 
		} else {
			ecc = Mathf.Sqrt( 1f - eqn1);
		}

		// inclination (0..180 so Acos is unambiguous) (2.136)
		float inclRad = Mathf.Acos(h_vec.z/h);

		// Omega_uc - only relevant if there is non-zero inclination
		float omega_ucRad = 0f;
		float sinOmega = 0f;
		float cosOmega = 1f;
		bool inclNoneZero = Mathf.Abs(Mathf.Sin(inclRad)) > 1E-5;
		if (inclNoneZero) {
			sinOmega = h_vec.x/(h*Mathf.Sin(inclRad));
			cosOmega = -h_vec.y/(h*Mathf.Sin(inclRad));
		} else if (h_vec.z < 0) {
			// if incl = 180
			sinOmega *= -1f;
			cosOmega *= -1f;
		}
		omega_ucRad = NUtils.AngleFromSinCos( sinOmega, cosOmega);
//		Debug.Log("sinOmega=" + sinOmega + " cosOmega=" + cosOmega + " omega_ucRad=" + omega_ucRad + " inclRad=" + inclRad
//			+ " inclNonZero=" + inclNoneZero + " h_vec=" + h_vec);

		float f = 0; 
		if (ecc > 1E-3) {
			// Ellipse (from Sidi)
			float cosPhi = (a - r)/(a * ecc); 
			float sinPhi = Vector3.Dot(r_vec, forNbody.vel_scaled)/(ecc*Mathf.Sqrt(mu*a));
			float cosf = (cosPhi - ecc)/(1-ecc*cosPhi); 
			float sinf = sinPhi*Mathf.Sqrt(1-ecc*ecc)/(1-ecc*cosPhi);
			f = NUtils.AngleFromSinCos(sinf, cosf);
		} 

		// (2.6.17)
		// sin(w+f) = Z/(r sin(i))
		// cos(w+f) = [X cos(Omega) + Y sin(Omega)]/r
		// when sin(i) = 0, will have Z=0 and w + f = 0
		float sin_of = 0f;
		float cos_of = 1f;
		float omega_lcRad = 0f;
		if (inclNoneZero) {
			sin_of = r_vec.z/(r*Mathf.Sin(inclRad));
			cos_of = (r_vec.x * cosOmega + r_vec.y * sinOmega)/r;
			omega_lcRad = (NUtils.AngleFromSinCos(sin_of, cos_of) - f);
		} else {
			float sin_theta = r_vec.y/r;
			float cos_theta = r_vec.x/r;
			float theta = NUtils.AngleFromSinCos(sin_theta, cos_theta);
			if (inclRad < Mathf.PI/2f) {
				omega_lcRad = theta - f; 
			} else {
				if (h_vec.z > 0) {
					omega_ucRad = f - theta; 
				} else {
					// Rotated by 180 around x, so flip sign of y
					sin_theta = -r_vec.y/r;
					cos_theta = r_vec.x/r;
					theta = NUtils.AngleFromSinCos(sin_theta, cos_theta);
					omega_ucRad = f - theta; 
				}
			}
		}
		// Ellipse wants degrees
		omega_lc = Mathf.Rad2Deg * omega_lcRad;
		if (omega_lc < 0)
			omega_lc += 360f;
		omega_uc = Mathf.Rad2Deg * omega_ucRad;
		if (omega_uc < 0)
			omega_uc += 360f;
		phase = Mathf.Rad2Deg * f; 
		inclination = Mathf.Rad2Deg * inclRad;
//		Debug.Log("cosof=" + cos_of + " sinof=" + sin_of + " of=" + NUtils.AngleFromSinCos(sin_of, cos_of) + " f=" + f);

		// Determine time to periapsis (2.140)
		// Eqn assumes E=0 when t=tau
		float E = Mathf.Acos((1f-r/a)/ecc);
		// this equation has a G but we set G=1
		period = 2*Mathf.PI*Mathf.Sqrt(a*a*a)/Mathf.Sqrt(aroundNBody.mass);
		float M = (E-ecc*Mathf.Sin(E));
		float tau = M*Mathf.Sqrt(a*a*a)/Mathf.Sqrt(aroundNBody.mass);
		// tau is giving time to/from apoapsis, need to find out which
		float vdotr = Vector3.Dot(vel,r_vec);
		if (vdotr > 0) {
			tau = period - tau;
		}

	}

	private void HyperOrbitForVelocity(NBody forNbody, NBody aroundNBody) {
		// Murray and Dermott Ch2.8 (2.134) - (2.140)
		float velSq = forNbody.vel_scaled.sqrMagnitude;

		Vector3 r_vec = forNbody.transform.position - aroundNBody.transform.position;
		float r = Vector3.Magnitude(r_vec);
		r_initial = r;
		float mu = aroundNBody.mass;
		// semi-major axis (2.134)
		// (reversed for HB)
		a = 1f/(velSq/mu - 2/r);

		// Determine angular momentum, h
		Vector3 h_vec = Vector3.Cross(r_vec, forNbody.vel_scaled);
		float h = Vector3.Magnitude(h_vec);
		//Debug.Log("h_vec = " + h_vec + " r_vec=" + r_vec + " v=" + forNbody.vel + " planet pos=" + forNbody.transform.position);

		// eccentricity (2.135)
		ecc = Mathf.Sqrt( h*h/(mu*a) + 1f);
		perihelion = a*(ecc-1);

		// inclination (0..180 so Acos is unambiguous) (2.136)
		float inclRad = Mathf.Acos(h_vec.z/h);

		// Omega_uc - only relevant if there is non-zero inclination
		float omega_ucRad = 0f;
		float sinOmega = 0f;
		float cosOmega = 1f;
		bool inclNoneZero = Mathf.Abs(Mathf.Sin(inclRad)) > 1E-5;
		if (inclNoneZero) {
			sinOmega = h_vec.x/(h*Mathf.Sin(inclRad));
			cosOmega = -h_vec.y/(h*Mathf.Sin(inclRad));
		} else if (h_vec.z < 0) {
			// if incl = 180
			sinOmega *= -1f;
			cosOmega *= -1f;
		}
		omega_ucRad = NUtils.AngleFromSinCos( sinOmega, cosOmega);
//		Debug.Log("sinOmega=" + sinOmega + " cosOmega=" + cosOmega + " omega_ucRad=" + omega_ucRad + " inclRad=" + inclRad
//			+ " inclNonZero=" + inclNoneZero + " h_vec=" + h_vec);

		float f = 0; 
		if (ecc > 1E-3) {
			// Like M&D 2.31 but for e^2-1 not 1-e^2
			float rdot = Mathf.Sqrt(velSq - h*h/(r*r));
			if (Vector3.Dot(r_vec, forNbody.vel_scaled) < 0) {
				rdot *= -1f;
			} 
			float cos_f = (1/ecc)*(a*(ecc*ecc-1)/r - 1f);
			float sin_f = rdot*a*(ecc*ecc-1f)/(h*ecc);
			f = NUtils.AngleFromSinCos(sin_f, cos_f);
		} 

		// (2.6.17)
		// sin(w+f) = Z/(r sin(i))
		// cos(w+f) = [X cos(Omega) + Y sin(Omega)]/r
		// when sin(i) = 0, will have Z=0 and w + f = 0
		float sin_of = 0f;
		float cos_of = 1f;
		float omega_lcRad = 0f;
		if (inclNoneZero) {
			sin_of = r_vec.z/(r*Mathf.Sin(inclRad));
			cos_of = (r_vec.x * cosOmega + r_vec.y * sinOmega)/r;
			omega_lcRad = (NUtils.AngleFromSinCos(sin_of, cos_of) - f);
		} else {
			float sin_theta = r_vec.y/r;
			float cos_theta = r_vec.x/r;
			float theta = NUtils.AngleFromSinCos(sin_theta, cos_theta);
//			Debug.Log("r_vec=" + r_vec + " theta=" + theta + " f=" + f + " (theta - f)=" + (theta-f));
			if (inclRad < Mathf.PI/2f) {
				omega_lcRad = theta - f; 
			} else {
				if (h_vec.z > 0) {
					omega_ucRad = f - theta; 
				} else {
					// Rotated by 180 around x, so flip sign of y
					sin_theta = -r_vec.y/r;
					cos_theta = r_vec.x/r;
					theta = NUtils.AngleFromSinCos(sin_theta, cos_theta);
					omega_ucRad = f - theta; 
				}
			}
		}
		// Ellipse wants degrees
		omega_lc = Mathf.Rad2Deg * omega_lcRad;
		if (omega_lc < 0)
			omega_lc += 360f;
		omega_uc = Mathf.Rad2Deg * omega_ucRad;
		if (omega_uc < 0)
			omega_uc += 360f;
		phase = Mathf.Rad2Deg * f; 
		inclination = Mathf.Rad2Deg * inclRad;
//		Debug.Log("cosof=" + cos_of + " sinof=" + sin_of + " of=" + NUtils.AngleFromSinCos(sin_of, cos_of) + " f=" + f);

	}

}
