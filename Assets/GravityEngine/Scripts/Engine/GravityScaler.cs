using UnityEngine;
using System.Collections;

public class GravityScaler : MonoBehaviour  {

	/// <summary>
	/// Units.
	/// Gravity N-Body simulations use units in which G=1 (G, the gravitational constant). 
	/// 
	/// From unit analysis of F = m_1 a = (G m_1 m_2)/r^2 we get: T = (L^3/G M)^(1/2) with
	///  T = time, L = length, M = Mass, G=Newton's constant = 6.67E-11.
	///
	///  SI (m,kg) => T=1.224E5 sec
	///  Solar (AU, 1E24 kg) => T = 7.083E9 sec.
	///
	/// To control game time, mass is rescaled in the physics engine to acheive the desired result. 
	/// Initial velocities are also adjusted to appropriate scale. 
	/// </summary>
	public const float G = 6.67408E-11f;  // m^3 kg^(-1) sec^(-2)

	public enum Units { DIMENSIONLESS, SI, ORBITAL, SOLAR }; //, STELLAR};
	public Units units;
	private static string[] lengthUnits =   {"DL", "m", "km", "AU", "light-year"};
	private static string[] massUnits =     {"DL", "kg", "1E24 kg", "1E24 kg", "Msolar"};
	private static string[] velocityUnits = {"DL", "m/s", "km/hr", "km/s", "km/s"};

	private static float velocityScale;

	/// <summary>
	/// Updates the time scale.
	/// Prior to scene starting GE adjusts the time scale by setting DT for the numerical integrators.
	///
	/// During evolution DT cannot be changed on the fly for the Leapfrog integrators without violating
	/// energy conservation - so changes are made in the number of integration performed. This imposes a
	/// practical limit on how much "speed up" can occur - since too much time evolution will lower the
	/// frame rate.
	/// </summary>
	/// <param name="value">Value.</param>

	private const float M_PER_KM = 1000f; 
	private const float M_PER_AU = 1.49598E+11f;
	private const float SEC_PER_YEAR = 3600f*24f*365.25f;
	private const float KM_HOUR_TO_M_SEC = M_PER_KM/SEC_PER_HOUR;

	private const float KM_SEC_TO_AU_YR = 0.210949527f;

	// Use T = Sqrt(L^3/(G M)) with all units and G in m/s/kg.
	// This gives the time unit that results from the choice of length/mass and the imposition of G=1
	// in the integrators. 
	// m/kg/sec
	private const float G1_TIME_SI = 122406.4481f;
	// km/ 1E24kg/ hr
	private const float SEC_PER_HOUR = 3600f;
	private const float G1_TIME_ORBIT = 0.003870832f;

	// SOLAR Units
	private const float G1_TIME_SOLAR = 7082595090f;

	// i.e. time is in units of approx. 3 ms
	// If we want Unity time in game sec. per physics hour then we need to 
	// timescale = game sec. per physics hour


	public static void UpdateTimeScale(Units units, float timeScale, float lengthScale) {
		// time unit size (sec.) for G=1 and units given
		float time_g1 = 1f; 
		// Number of physics seconds per Unity second, given the units and timeScale
		float game_sec_per_phys_sec = 1f; 
		// Convert all cases to SI units - this is the units for G
		// Adjust the SI units by the Unity scale factors. 
		switch(units) {
		case Units.DIMENSIONLESS:
			// mass scale is controlled via inspector for dimensionless units
			time_g1 = G1_TIME_SI;
			game_sec_per_phys_sec = 1f/timeScale;
			velocityScale = 1f;
			return;
		case Units.SI:
			time_g1 = G1_TIME_SI;
			game_sec_per_phys_sec = 1f/timeScale;
			velocityScale = lengthScale/timeScale;
			break;
		case Units.ORBITAL:
			time_g1 = G1_TIME_ORBIT;
			game_sec_per_phys_sec = SEC_PER_HOUR/timeScale;
			velocityScale = lengthScale/timeScale;
			break;
		case Units.SOLAR:
			time_g1 = G1_TIME_SOLAR;
			game_sec_per_phys_sec = SEC_PER_YEAR/timeScale;
			velocityScale = KM_SEC_TO_AU_YR * lengthScale/timeScale;
			break;
		}

		// The length scale chosen for the scene is now applied.
		// Convert to the designated length scale (convert from m to Unity units)
		float time_unity = time_g1 / Mathf.Sqrt(lengthScale*lengthScale*lengthScale);

		// timeScale indicates game seconds per physics sec. 

		// The masses do not affect position in scene, so instead of adjusting raw masses of all NBody objects this
		// adjusment is done to the physics copy in GE as they are added. See GravityEngine:SetupOneGameObject() [private]
		float mu = game_sec_per_phys_sec*game_sec_per_phys_sec/(time_unity*time_unity);
		GravityEngine.Instance().massScale = mu;

		// change in mass scale has done most of the work. Need to shift velocity by time/length scales

		#pragma warning disable 162		// disable unreachable code warning
		if (GravityEngine.DEBUG) {
			Debug.Log("SetMassScale: Time G1 = " + time_g1 + 
					" time_unity=" + time_unity +
					" game_sec_per_phys_sec=" + game_sec_per_phys_sec + 
					" massScale=" + mu + " velScale=" + velocityScale );
		}
		#pragma warning restore 162

	}

	public static float GetVelocityScale() {
		return velocityScale;
	}

	/// <summary>
	/// Changes the length scale of all NBody objects in the scene due to a change in the inspector.
	/// Find all NBody containing objects.
	/// - independent objects are rescaled
	/// - orbit based objects have their primary dimension adjusted (e.g. for ellipse, a)
	///   (these objects are scalable and are asked to rescale themselves)
	///
	/// Not intended for run-time use.
	/// </summary>
	public static void ScaleScene(Units units, float timeScale, float lengthScale) {

		// ensure velocity scale is determined
		// find everything with an NBody. Rescale will ensure only independent NBodies are rescaled. 
		NBody[] nbodies = FindObjectsOfType<NBody>();
		foreach (NBody nbody in nbodies) {
			ScaleNBody(nbody, units, timeScale, lengthScale);
		}
	}

	/// <summary>
	/// Scales the N body using the provided units and length scale.
	/// </summary>
	/// <param name="nbody">Nbody.</param>
	/// <param name="units">Units.</param>
	/// <param name="timeScale">Time scale.</param>
	/// <param name="lengthScale">Length scale.</param>
	public static void ScaleNBody(NBody nbody, Units units, float timeScale, float lengthScale) {
		// If there is an IOrbitScaler - use it instead
		IOrbitScalable iorbit = nbody.GetComponent<IOrbitScalable>();
		if (iorbit != null) {
			iorbit.ApplyScale(lengthScale);
		} else {
			if (units == GravityScaler.Units.DIMENSIONLESS && lengthScale == 1f) {
				// Backwards compatibility with pre 1.3 GE
				nbody.initialPos = nbody.transform.position;
			}
			nbody.ApplyScale(lengthScale, velocityScale);	
		}
	}

	/// <summary>
	/// Return the string indicating the length units in use by the gravity engine. 
	/// </summary>
	/// <returns>The units.</returns>
	public static string LengthUnits(Units units) {
		return lengthUnits[(int) units];
	}

	/// <summary>
	/// Return the string indicating the velocity units in use by the gravity engine. 
	/// </summary>
	/// <returns>The units.</returns>
	public static string VelocityUnits(Units units) {
		return velocityUnits[(int) units];
	}

	/// <summary>
	/// Return the string indicating the mass units in use by the gravity engine.
	/// </summary>
	/// <returns>The units.</returns>
	public static string MassUnits(Units units) {
		return massUnits[(int) units];
	}

}
