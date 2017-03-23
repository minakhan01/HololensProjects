#define SOLAR_SYSTEM
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;


/// <summary>
/// Ellipse base.
///
/// Base class used to handle the orbital parameters and draw the orbit path in the editor using Gizmos. 
///
/// How to specify an ellipse in 3D:
///
/// size: one of a or p can be used to specify the ellipse size. 
///    a - length of semi-major axis i.e. the distance from the center of the ellipse to the farthest point
///    p - pericenter - distance from focus of ellipse to point of closest approach to that focus
///
/// shape: controlled by ecc (eccentricity) 0 for a circle, 0.99 for a very long thin ellipse
///
/// orientation:
///  The standard orbit parameters are used. Y	ou can develop some intuition for these by chaging them in the Editor and observing
///  the change in the orbit. 
///
///  Orientation is defined with respect to the positive X axis. 
///    omega (lower case) - is a rotation in the plane of the orbit 
///    Inclination - is the tilt of the closest approach vector to the XY plance
///    Omega (capital Omega) - is the rotation around Z after preceeding rotations
///
/// </summary>
public class EllipseBase : MonoBehaviour, IOrbitPositions {

	public enum ParamBy { AXIS_A, CLOSEST_P}; 

	//! Define ellipse by semi0major axis (A) or closest approach (P)
	public ParamBy paramBy = ParamBy.AXIS_A;

	//! object to orbit around (if null, will take parent game object)
	public GameObject centerObject; 
	
	// Orbit parameters (user control via FixedEllipseEditor)
	// These parameters are in world space. 
	//! eccentricity (0..1, 0=circle, 1=linear)
	public float ecc; 	

	public float a_scaled = -1f; 
	public float p_scaled;		

	// Allow EITHER a or p to specify size (JPL data uses a for planets and p for comets, so both are useful)
	//! semi-major axis - based on paramBy user can specify a OR p. a = p/(1-ecc)
	/// <summary>
	/// (a,p) hold the values for a and p in the unit system specified
	/// by the gravity engine. These are scaled and used to set a and p for game simulation
	/// based on the unit scaling system provided by gravity engine. 
	/// </summary>
	public float a = 10f; 			
	//! pericenter - based on paramBy user can specify a OR p. a = p/(1-ecc)
	public float p; 			
	//! "longitude of ascending node" - angle from x-axis to line from focus to pericenter
	public float omega_uc; 		
	//! "argument of perienter" - angle from ascending node to pericenter
	public float omega_lc; 		
	//! inclination (degrees!)
	public float inclination; 	
	//! initial TRUE anomoly (angle wrt line from focus to closest approach)
	public float phase; 		

	protected Quaternion ellipse_orientation;

	protected Vector3 xunit = Vector3.right;
	protected Vector3 yunit = Vector3.up;
	protected Vector3 zunit = Vector3.forward;

	protected NBody centerNbody; 

	protected OrbitData initData; 

	void Awake() {
		a_scaled = a;
	}
	
	/// <summary>
	/// Init the ellipse, verify a center body is present, determine orientation and update transform.
	/// </summary>
	public void Init () {
		// mode determines if user wants to define wrt to A or P. Determine other quantity 
		if (paramBy == ParamBy.AXIS_A ){
			p = a*(1-ecc);
		} else if (paramBy == ParamBy.CLOSEST_P) {
			a = p/(1-ecc);
		}
		// If parent has an Nbody assume it is the center
		if (centerObject == null) {
			if (transform.parent != null) {
				centerNbody = transform.parent.gameObject.GetComponent<NBody>();
				if (centerNbody != null) {
					centerObject = transform.parent.gameObject;
				} else {
					Debug.LogError("Parent object must have NBody attached");
				}
			} else {
				Debug.Log("Warning - Require a parent object (with NBody)");
				// This path when init-ed via Instantiate() script will need to 
				// call Init() explicily once orbit params and center are set
				return; 
			}
		} else {
			centerNbody = centerObject.GetComponent<NBody>();
			if (centerNbody == null) {
				Debug.LogError("CenterObject must have an NBody attached");
			}
		}
		CalculateRotation();
		SetTransform();
	}

	/// <summary>
	/// Sets the center body and initializes the ellipse configuration.
	/// </summary>
	/// <param name="centerBody">Center body.</param>
	public void SetCenterBody(GameObject centerBody) {
		centerObject = centerBody; 
		Init(); 
	}

	/// <summary>
	/// Inits an EllipseBase orbit from orbital parameters contained in an OrbitData. 
	///
	/// Typically used for the creation of an EllipseBase for an OrbitPredictor and for
	/// the creation of solar system objects via SolarSystem. 
	///
	/// </summary>
	/// <param name="od">Od.</param>
	public void InitFromOrbitData(OrbitData od) {
		a_scaled = od.a; 
		ecc = od.ecc; 
		omega_lc = od.omega_lc;
		omega_uc = od.omega_uc; 
		inclination = od.inclination;
		phase = od.phase;
		initData = od;
		Init();
	}

	public OrbitData GetOrbitData() {
		return initData;
	}


	// Allow either P or A as params. Only one can be specified in editor at a time since they are related.
	// If the editor changes the value, the keep the related variable in sync.
	protected void UpdateOrbitParams() {
		if (paramBy == ParamBy.AXIS_A){
			p = a*(1-ecc);
		} else if (paramBy == ParamBy.CLOSEST_P) {
			a = p/(1-ecc);
		}
	}
	
	protected void CalculateRotation() {
		// Following Murray and Dermot Ch 2.8 Fig 2.14
		// Quaternions go L to R (matrices are R to L)
		ellipse_orientation = Quaternion.AngleAxis(omega_uc, zunit ) *
							  Quaternion.AngleAxis(inclination, xunit) * 
							  Quaternion.AngleAxis(omega_lc, zunit);
	}

	/// <summary>
	/// Sets the transform based on the orbit parameters.
	/// </summary>
	public void SetTransform() {
		float phaseRad = phase * Mathf.Deg2Rad;
		// position object using true anomoly (angle from  focus)
		float r = a_scaled * ( 1f - ecc* ecc)/(1f + ecc * Mathf.Cos(phaseRad));
		
		Vector3 pos = new Vector3( r * Mathf.Cos (phaseRad), r * Mathf.Sin (phaseRad), 0);
		// move from XY plane to the orbital plane
		Vector3 new_p = ellipse_orientation * pos; 
		// orbit position is WRT center
		transform.position = new_p + centerObject.transform.position;

	}

	private Vector3 PositionForTheta(float theta) {
		float r = a_scaled * ( 1f - ecc* ecc)/(1f + ecc * Mathf.Cos(theta));
		Vector3 position = new Vector3( r * Mathf.Cos (theta), r * Mathf.Sin (theta), 0);
		// move from XY plane to the orbital plane
		Vector3 newPosition = ellipse_orientation * position; 
		// orbit position is WRT center
		newPosition += centerObject.transform.position;
		return newPosition;
	}

	/// <summary>
	/// Calculate an array of points that describe the specified orbit
	/// </summary>
	/// <returns>The positions.</returns>
	/// <param name="numPoints">Number points.</param>
	public Vector3[] OrbitPositions(int numPoints) {
		Vector3[] emptyArray = {new Vector3(0,0,0), new Vector3(0,0,0)};
		// need to have a center to create positions.
		if (centerObject == null && transform.parent != null) {
			centerObject = transform.parent.gameObject;
		}
		if (centerObject == null) {
			return emptyArray;
		}
		Vector3[] points = new Vector3[numPoints];

		UpdateOrbitParams();
		CalculateRotation();
		// start at theta=0 on the ellipse and transform to world space
		float r = a_scaled * ( 1f - ecc* ecc)/(1f + ecc);
		Vector3 oldPosition = new Vector3( r, 0, 0);
		oldPosition = ellipse_orientation * oldPosition;
		oldPosition += centerObject.transform.position;

		float dtheta = 2f*Mathf.PI/numPoints;
		float theta = 0; 

		// add a fudge factor to ensure we go all the way around the circle
		for (int i=0; i < numPoints; i++) { 
			points[i] = PositionForTheta(theta);
			if (NUtils.VectorNaN(points[i])) {
				points[i] = Vector3.zero;
			}
			theta += dtheta;
		}
		// close the path (credit for fix to R. Vincent)
		points[numPoints-1]=points[0];
		return points;
	}

	/// <summary>
	/// Return the center object around which this ellipse is defined.
	/// </summary>
	/// <returns>The center object.</returns>
	public GameObject GetCenterObject() {
		// need to have a center to draw gizmo.
		if (centerObject == null && transform.parent != null) {
			centerObject = transform.parent.gameObject;
		}
		return centerObject;
	}

#if UNITY_EDITOR
	/// <summary>
	/// Displays the path of the elliptical orbit when the object is selected in the editor. 
	/// </summary>
	void OnDrawGizmosSelected()
	{
		// need to have a center to draw gizmo.
		if (GetCenterObject() == null) {
			return;
		}
		// only display if this object or parent is selected
		bool selected = Selection.Contains(transform.gameObject);
		if (transform.parent != null)
			selected |= Selection.Contains(transform.parent.gameObject);
		if (!selected) {
			return;
		}

		UpdateOrbitParams();
		CalculateRotation();

		const int NUM_STEPS = 100; 
		const int STEPS_PER_RAY = 10; 
		int rayCount = 0; 
		Gizmos.color = Color.white;

		Vector3[] positions = OrbitPositions(NUM_STEPS);
		for (int i=1; i < NUM_STEPS; i++) {
			Gizmos.DrawLine( positions[i], positions[i-1]);
			rayCount = (rayCount+1)%STEPS_PER_RAY;
			if (rayCount == 0) {
				Gizmos.DrawLine(centerObject.transform.position, positions[i] );
			}
		}
		// close the circle
		Gizmos.DrawLine( positions[NUM_STEPS-1], positions[0]);

		// Draw the axes in a different color
		Gizmos.color = Color.red;
		Gizmos.DrawLine(PositionForTheta(0.5f*Mathf.PI), PositionForTheta(-0.5f*Mathf.PI) );
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(PositionForTheta(0f), PositionForTheta(Mathf.PI) );


		// move body to location specified by parameters
		SetTransform();

		// Draw the Hill sphere
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, OrbitUtils.HillRadius(centerObject, transform.gameObject));
	}
#endif	
}
