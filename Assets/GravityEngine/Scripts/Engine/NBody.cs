using UnityEngine;
using System.Collections;

/// <summary>
/// N body.
///
/// Specifies the information required for NBody physics evolution of the associated game object. 
///
/// </summary>
public class NBody : MonoBehaviour {

	//! mass of object (mass scale in GravityEngine will be applied to get value used in simulation)
	public float mass;	

	//! Velocity - as set in the inspector
	public Vector3 vel; 
	//! Velocity adjusted for the units/scale chosen by Gravity Engine
	public Vector3 vel_scaled; 

	/// <summary>
	/// The initial position/velocity
	/// This indicates the position in the active units used by the Gravity engine (m or AU). 
	/// If the units are DIMENSIONLESS, then this field is not active and the transform position
	/// is not affected by scaling. 
	///
	/// When m or AU are active, a change in the scale factor of the gravity engine in the editor will
	/// change all the associated transform positions and velocities but the initialPos/initialVel will not be changed. 
	/// 
	/// Positions are affected by changes in the lengthScale
	/// Velocities are affected by changes in both the length scale and the timeScale. (See ApplyScale() )
	/// </summary>
	public Vector3 initialPos; 

	//! Automatically detect particle capture size from a child with a mesh
	public bool automaticParticleCapture = true;

	//! Particle capture radius. Particles closer than this will be inactivated.
	public double size = 0.1; 

	//! Opaque data maintained by the GravityEngine. Do not modify.
	public GravityEngine.EngineRef engineRef;  

	public void Awake() {
		// automatic size detection if there is an attached mesh filter
		if (automaticParticleCapture) {
			size = CalculateSize();
		}
	}

	public float CalculateSize() {
		foreach( Transform t in transform) {
			MeshFilter mf = t.gameObject.GetComponent<MeshFilter>();
			if (mf != null) {
				// cannot be sure it's a sphere, but assume it is
				return t.localScale.x/2f;
			}
		}
		// compound objects may not have a top level mesh
		return 1; 
	}

	/// <summary>
	/// Updates the velocity.
	/// The Gravity Engine does not copy back velocity updates during evolution. Calling this method causes
	/// an update to the scaled velocity. 
	/// </summary>
	public void UpdateVelocity() {
		vel_scaled = GravityEngine.instance.GetVelocity(transform.gameObject);
	}

	static public bool IsIndependent(NBody nbody) {
		bool independent = true; 
		if ((nbody.transform.gameObject.GetComponent<OrbitEllipse>() != null) ||
		    (nbody.transform.gameObject.GetComponent<OrbitHyper>() != null)) {
			independent = false;
		} else if (nbody.transform.parent != null) {
			if ((nbody.transform.parent.gameObject.GetComponent<BinaryPair>() != null) ||
			    (nbody.transform.parent.gameObject.GetComponent<ThreeBodySolution>() != null)) {
				independent = false;
			}
		}
		return independent;
	}

	/// <summary>
	/// Rescale with specified lengthScale.
	/// </summary>
	/// <param name="lengthScale">Length scale.</param>
	/// <param name="velocityScale">Length scale.</param>
	public void ApplyScale(float lengthScale, float velocityScale) {
		transform.position = lengthScale * initialPos;
		vel_scaled = vel * velocityScale;
		#pragma warning disable 162		// disable unreachable code warning
			if (GravityEngine.DEBUG) {
				Debug.Log(string.Format("Nbody scale: {0} r=[{1} {2} {3}] v=[{4} {5} {6}] initial=[{7} {8} {9}]",
					gameObject.name, transform.position.x, transform.position.y, transform.position.z, 
					vel_scaled.x, vel_scaled.y, vel_scaled.z, 
					initialPos.x, initialPos.y, initialPos.z));
			}
		#pragma warning restore 162		// enable unreachable code warning

	}

}
