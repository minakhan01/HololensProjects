using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// Binary Pair
///
/// Configures the initial velocities for two roughly equal masses to establish their
/// elliptical orbits around the center of mass of the pair. 
///
/// Must have two NBody objects as immediate children.
/// </summary>
public class BinaryPair :  EllipseBase, IOrbitScalable {

	//! Velocity of center of mass of the binary pair
	public Vector3 velocity;

	private NBody body1; 
	private NBody body2; 

	// Use this for initialization
	void Start() {
		SetupBodies();
	}

	private void SetupBodies () {

		if (transform.childCount != 2) {
			// can have more than 2 if 
			Debug.LogError("Must have exactly two Nbody objects attached");
			return;
		}
		body1 = transform.GetChild(0).GetComponent<NBody>();
		body2 = transform.GetChild(1).GetComponent<NBody>();
		if (body1 == null || body2 == null) {
			Debug.LogError("Binary requires children to have NBody scripts attached.");
			return;
		}
		// mass is scaled by GE
		float m_total = (body1.mass + body2.mass);
		float mu1 = body1.mass/m_total;
		float mu2 = body2.mass/m_total;
		SetupBody( body1, a_scaled * mu2,  mu2 * mu2 * body2.mass, false);
		SetupBody( body2, a_scaled * mu1,  mu1 * mu1 * body1.mass, true);
	}

	/// <summary>
	/// Calculate velocity for binary members and assign.
	/// reflect: One of the bodies positions and velocities must be reflected to opposite side of ellipse
	/// </summary>
	private void SetupBody(NBody nbody, float a_n, float mu, bool reflect) {
		float a_phy = a_n/GravityEngine.instance.physToWorldFactor;
		// Phase is TRUE anomoly f
		float f = phase * Mathf.Deg2Rad;
		float reflect_in_y = 1f;
		if (reflect) {
			reflect_in_y = -1f;
			//f = f + Mathf.PI;
		}

		// Murray and Dermott (2.20)
		float r = a_phy * (1f - ecc*ecc)/(1f + ecc * Mathf.Cos(f));
		// (2.26) mu = n^2 a^3  (n is period, aka T)
		float n = Mathf.Sqrt( mu * GravityEngine.Instance().massScale/(a_phy*a_phy*a_phy));
		// (2.36)
		float denom = Mathf.Sqrt( 1f - ecc*ecc);
		float xdot = -1f * n * a_phy * Mathf.Sin(f)/denom;
		float ydot = n * a_phy * (ecc + Mathf.Cos(f))/denom;

		Vector3 position_xy = new Vector3( reflect_in_y * r * Mathf.Cos(f), r* Mathf.Sin(f), 0);
		// move from XY plane to the orbital plane and scale to world space
		// orbit position is WRT center
		Vector3 position =  ellipse_orientation * position_xy;
		position += transform.position/GravityEngine.instance.physToWorldFactor;
		nbody.initialPos = position;

		Vector3 v_xy = new Vector3( xdot, ydot, 0);
		v_xy *= reflect_in_y;
		// velocity will be scaled when NBody is scaled
		nbody.vel = ellipse_orientation * v_xy + velocity;
		// Debug.Log("body=" + nbody.name + " pos=" + position + " vxy=" + v_xy + " n=" + n);

	}

	/// <summary>
	/// Apply scale to the orbit. This is used by the inspector scripts during
	/// scene setup. Do not use at run-time.
	/// </summary>
	/// <param name="scale">Scale.</param>
	public void ApplyScale(float scale) {
		if (paramBy == ParamBy.AXIS_A){
			a_scaled = a * scale;
		} else if (paramBy == ParamBy.CLOSEST_P) {
			p_scaled = p * scale; 
		}
		UpdateOrbitParams();
		SetupBodies();
	}

#if UNITY_EDITOR

	/// <summary>
	/// Displays the path of the elliptical orbit when the object is selected in the editor. 
	/// </summary>
	void OnDrawGizmosSelected()
	{
		// need to have a center to draw gizmo.
		body1 = transform.GetChild(0).GetComponent<NBody>();
		body2 = transform.GetChild(1).GetComponent<NBody>();
		if (body1 == null || body2 == null) {
			return;
		}
		// only display if this object is directly selected
		if (Selection.activeGameObject != transform.gameObject) {
			return;
		}
		float m_total = (float) (body1.mass + body2.mass);
		float mu1 = body1.mass/m_total;
		float mu2 = body2.mass/m_total;

		UpdateOrbitParams();
		CalculateRotation();

		DrawEllipse( a_scaled * mu2, transform.position, false );
		DrawEllipse( a_scaled * mu1, transform.position, true );
		// move bodies to location specified by parameters
		SetTransform( a_scaled * mu2, body1, false);
		SetTransform( a_scaled * mu1, body2, true);
	}

	private void DrawEllipse(float a_n, Vector3 focusPos, bool reflect) {
		Vector3 oldPosition = transform.position;
		const float NUM_STEPS = 100f; 
		const int STEPS_PER_RAY = 10; 
		float reflect_in_y = 1f;
		if (reflect) {
			reflect_in_y = -1f;
		}
		int rayCount = 0; 
		float dtheta = 2f*Mathf.PI/NUM_STEPS;
		float r = 0; 
		float phaseRad = phase * Mathf.Deg2Rad;
		for (float theta=0f; theta < 2f*Mathf.PI; theta += dtheta) {
			// draw a ellipe - using equation centered on focus (true anomoly)
			r = a_n * ( 1f - ecc * ecc)/(1f + ecc * Mathf.Cos(theta+phaseRad));
			
			Vector3 position = new Vector3(reflect_in_y * r * Mathf.Cos (theta+phaseRad), 
			                               r * Mathf.Sin (theta+phaseRad), 
			                               0);
			// move from XY plane to the orbital plane
			Vector3 newPosition = ellipse_orientation * position; 
			// orbit position is WRT center
			newPosition += focusPos;
			Gizmos.DrawLine(oldPosition, newPosition );
			rayCount = (rayCount+1)%STEPS_PER_RAY;
			if (rayCount == 0) {
				Gizmos.DrawLine(focusPos, newPosition );
			}
			oldPosition = newPosition;
		}

	}

	private void SetTransform(float a_n, NBody nbody, bool reflect) {
		float phaseRad = phase * Mathf.Deg2Rad;
		float reflect_in_y = 1f;
		if (reflect) {
			reflect_in_y = -1f;
			phaseRad = 2f * Mathf.PI - phaseRad;
		}
		// evolution uses true anomoly (angle from  center)
		float r = a_n * ( 1f - ecc* ecc)/(1f + ecc * Mathf.Cos(phaseRad));
		
		Vector3 pos = new Vector3( reflect_in_y * r * Mathf.Cos(phaseRad), 
		                          r * Mathf.Sin(phaseRad), 
		                          0);
		// move from XY plane to the orbital plane
		Vector3 new_p = ellipse_orientation * pos; 
		// orbit position is WRT center
		nbody.transform.position = new_p + transform.position;

	}

#endif
}
