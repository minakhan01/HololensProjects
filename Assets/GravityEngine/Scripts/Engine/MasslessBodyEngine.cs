using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///
/// This class handles massless test bodies (e.g a spaceship, satellite etc.) that are influenced
/// by the gravity in the GravityEngine but do not affect the gravitation field felt by others. 
///
/// If massless bodies are required, the GravityEngine engine will create an instance of this class to manage
/// their evolution IF the option OptimizeMassless has been set. 
///
/// Another good reason to enable OptimizeMAssless  is it allows test bodies to be used with an 
/// integrator with a limited number of massive bodies (e.g. AZTTriple)
///
/// Other benefits:
/// - reduces computational complexity (since GravityEngine runs as O(N^2) adding test bodies can be expensive)
/// - allows a simpler integration to be used 
/// - allows multiple complex objects (i.e. meshes) to be added when GravityParticles is not suitable
///
/// This engine has a "baked in" Leapfrog integrator; close encounters with masses will result in inaccurate
/// evolution. 
///
/// If higher precision is needed, use GravityEngine with Hermite and set the mass of the object to zero
///
/// This class is only called from GravityEngine and should not be called from user scripts. 
///
/// </summary>

public class MasslessBodyEngine  {


	private double dt = 1.0/(60.0 * 8.0);
				
	private int numBodies; 
	private double[,] r;		// position 
	private double[,] v;		// velocity
	private double[,] a;		// acceleration
	private byte[] info; 
	private GameObject[] bodies; 
	
	// following info
	private int inactiveCount; 
	private int ejectCount;  

	private int arraySize = 10; // 
	private const int GROW_ARRAY = 10; 
	
//	private static double RSQ_EJECTION = GravityEngine.instance.outOfBounds * GravityEngine.instance.outOfBounds;

	private const double EPSILON = 1E-4; 	// minimum distance for gravitatonal force
		
	// Parameters in init delegate have chanages, re-initialize the particle positions and
	// velocities
	public MasslessBodyEngine(double dt) {
		this.dt = dt;
		numBodies = 0;
		bodies = new GameObject[arraySize];
		r = new double[arraySize, GravityEngine.NDIM];
		v = new double[arraySize, GravityEngine.NDIM];
		a = new double[arraySize, GravityEngine.NDIM];
		info = new byte[arraySize];
	}

	/// <summary>
	/// Inactivates the body. Ensures body is skipped in force calculations and
	/// physics evolution.
	/// </summary>
	/// <returns>The body.</returns>
	/// <param name="toInactivate">To inactivate.</param>
	public Vector3 InactivateBody(GameObject toInactivate) {
		for (int i=0; i < numBodies; i++) {
			if (toInactivate == bodies[i]) {
				info[i] |= GravityEngine.INACTIVE; 
				return Vector3.zero;
			}		
		}
		Debug.LogWarning("Did not find " + toInactivate.name);
		return Vector3.zero;
	}

	/// <summary>
	/// Re-activates an inactive body
	/// </summary>
	/// <returns>The body.</returns>
	/// <param name="toInactivate">To inactivate.</param>
	public Vector3 ActivateBody(GameObject toInactivate) {
		for (int i=0; i < numBodies; i++) {
			if (toInactivate == bodies[i]) {
				info[i] &=  unchecked((byte)~GravityEngine.INACTIVE);
				return Vector3.zero;
			}		
		}
		Debug.LogWarning("Did not find " + toInactivate.name);
		return Vector3.zero;
	}

	public void AddBody(GameObject gameObject) {
		if (numBodies+1 >= arraySize) {
			GrowArrays(GROW_ARRAY);
		}
		bodies[numBodies] = gameObject;
		NBody nbody = bodies[numBodies].GetComponent<NBody>();
		nbody.engineRef = new GravityEngine.EngineRef(GravityEngine.BodyType.MASSLESS, numBodies);
		Vector3 physicsPosition = gameObject.transform.position/GravityEngine.instance.physToWorldFactor;
		r[numBodies,0] = physicsPosition.x;
		r[numBodies,1] = physicsPosition.y;
		r[numBodies,2] = physicsPosition.z; 
		v[numBodies,0] = nbody.vel_scaled.x; 
		v[numBodies,1] = nbody.vel_scaled.y; 
		v[numBodies,2] = nbody.vel_scaled.z; 
		info[numBodies] = 0;
		numBodies++;
		#pragma warning disable 162		// disable unreachable code warning
		if (GravityEngine.DEBUG)
			Debug.Log("Added " + gameObject.name);
		#pragma warning restore 162		
	}

	public void RemoveBody(GameObject gameObject) {
		NBody nbody = gameObject.GetComponent<NBody>();
		// shuffle rest of entries in array up
		#pragma warning disable 162		// disable unreachable code warning
		if (GravityEngine.DEBUG)
			Debug.Log("Remove body at " + nbody.engineRef.index);
		#pragma warning restore 162		

		// shuffle down array
		for( int j=nbody.engineRef.index; j < numBodies-1; j++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				r[j,k] = r[j+1, k]; 
				v[j,k] = v[j+1, k]; 
			}
			info[j] = info[j+1];
			bodies[j] = bodies[j+1];
			NBody nb = bodies[j].GetComponent<NBody>();
			nb.engineRef.index = j;
		}
		numBodies--;
	}

	public int NumBodies() {
		return numBodies;
	}

	private void GrowArrays(int growBy) {
		double[,] r_copy = new double[arraySize, GravityEngine.NDIM]; 
		double[,] v_copy = new double[arraySize, GravityEngine.NDIM];  
		GameObject[] bodies_copy = new GameObject[arraySize];
		byte[] info_copy = new byte[arraySize];

		for( int j=0; j < numBodies; j++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				r_copy[j,k] = r[j, k]; 
				v_copy[j,k] = v[j, k]; 
			}
			info_copy[j] = info[j];
			bodies_copy[j] = bodies[j];
		}
		r = new double[arraySize+growBy, GravityEngine.NDIM];
		v = new double[arraySize+growBy, GravityEngine.NDIM];
		a = new double[arraySize+growBy, GravityEngine.NDIM];
		info = new byte[arraySize+growBy];
		bodies = new GameObject[arraySize+growBy];

		for( int j=0; j < numBodies; j++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				r[j,k] = r_copy[j, k]; 
				v[j,k] = v_copy[j, k]; 
			}
			info[j] = info_copy[j];
			bodies[j] = bodies_copy[j];
		}
		arraySize += growBy;
	}
		
	/// <summary>
	/// Pre-evolve the massless bodies based in the array information about the massive bodies in the 
	/// system.
	/// </summary>
	/// <param name="numMasses">Number masses.</param>
	/// <param name="m">Mass array for massive bodies</param>
	/// <param name="r_m">Position array for massive bodies.</param>
	public void PreEvolve(int numMasses, ref double[] m, ref double[,] r_m) {
		if (numMasses == 0 && numBodies == 0) {
			return;
		}
		// Precalc initial acceleration
		double[] rji = new double[GravityEngine.NDIM]; 
		double r2; 
		double r3; 
		for (int i=0; i < numBodies; i++) {
			a[i,0] = 0.0;
			a[i,1] = 0.0;
			a[i,2] = 0.0;
		}
		// precalc initial acceleration due to massive bodies
		for (int i=0; i < numMasses; i++) {	
			for (int j=0; j < numBodies; j++) {
				for (int k=0; k < GravityEngine.NDIM; k++) {
					rji[k] = r[j,k] - r_m[i,k];
				}
				r2 = 0; 
				for (int k=0; k < GravityEngine.NDIM; k++) {
					r2 += rji[k] * rji[k]; 
				}
				r3 = r2 * System.Math.Sqrt(r2) + EPSILON; 
				for (int k=0; k < GravityEngine.NDIM; k++) {
					a[j,k] += m[i] * rji[k]/r3; 
				}
			}
		}	
		#pragma warning disable 162		// disable unreachable code warning
		if (GravityEngine.DEBUG)
			Debug.Log (string.Format ("PreEvolve: Initial a={0} {1} {2}", a[0,0], a[0,1], a[0,2]));		
		#pragma warning restore 162		
	}
		

	/// <summary>
	/// Evolve the position of the active massless bodies based on the gravitational field by the
	/// objects provided in the arguments m, r_m.
	///
	/// This does not use the standard integrators since the list of bodies and the loop structure
	/// differs.
	///
	/// </summary>
	/// <param name="evolveFor">Total time to evolve for</param>
	/// <param name="numMasses">Number masses.</param>
	/// <param name="m">M.</param>
	/// <param name="r_m">R m.</param>
	/// <param name="size2">Size2.</param>
	public double Evolve(double evolveFor, int numMasses, ref double[] m, ref double[,] r_m, ref byte[] massive_info) {
		if (numMasses == 0 && numBodies == 0) {
			return evolveFor;
		}
		// advance acceleration
		double[] rji = new double[GravityEngine.NDIM]; 
		double r2; 	// r squared
		double r3;  // r cubed
		double time = 0.0; 
		// Built in LeapFrog integrator
		while (time < evolveFor) {
			time += dt; 
			// Update v using a from last cycle (1/2 step), then update r
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.INACTIVE) == 0) {
					for (int k=0; k < GravityEngine.NDIM; k++) {
						v[i,k] += a[i,k] * dt/2.0;
						r[i,k] += v[i,k] * dt;
					}		
				}			
			}
			// a = 0 
			for (int i=0; i < numBodies; i++) {
				a[i,0] = 0.0;
				a[i,1] = 0.0;
				a[i,2] = 0.0;
			}
			// calc a
			for (int i=0; i < numMasses; i++) {
				if ((massive_info[i] & GravityEngine.INACTIVE) == 0) {
					for (int j=0; j < numBodies; j++) {
						double rsq_wrt0 = 0; 		// r^2 wrt origin
						if ((info[j] & GravityEngine.INACTIVE) == 0) {
							rji[0] = r[j,0] - r_m[i,0];
							rsq_wrt0 += r[j,0] * r[j,0];
							rji[1] = r[j,1] - r_m[i,1];
							rsq_wrt0 += r[j,1] * r[j,1];
							rji[2] = r[j,2] - r_m[i,2];
							rsq_wrt0 += r[j,2] * r[j,2];

							r2 = 0; 
								r2 += rji[0] * rji[0]; 
								r2 += rji[1] * rji[1]; 
								r2 += rji[2] * rji[2]; 
							r3 = r2 * System.Math.Sqrt(r2) + EPSILON; 
							// Force equation
							a[j,0] -= m[i] * rji[0]/r3;
							a[j,1] -= m[i] * rji[1]/r3;
							a[j,2] -= m[i] * rji[2]/r3;
						} // j
					} // if info
				} // i
			}
			// update velocity with other half step
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.INACTIVE) == 0) {
					v[i,0] += a[i,0] * dt/2.0;
					v[i,1] += a[i,1] * dt/2.0;
					v[i,2] += a[i,2] * dt/2.0;
				}				
			}
		}
		return time;
	}

	/// <summary>
	/// Evolve the position of the active massless bodies based on the gravitational field by the
	/// objects provided in the arguments m, r_m and the specified force.
	///
	/// This does not use the standard integrators since the list of bodies and the loop structure
	/// differs.
	/// </summary>
	/// <returns>The with force.</returns>
	/// <param name="evolveFor">Evolve for.</param>
	/// <param name="numMasses">Number masses.</param>
	/// <param name="m">M.</param>
	/// <param name="r_m">R m.</param>
	/// <param name="massive_info">Massive info.</param>
	/// <param name="force">Force.</param>
	public double EvolveWithForce(double evolveFor, 
					int numMasses, 
					ref double[] m, 
					ref double[,] r_m, 
					ref byte[] massive_info, 
					IForceDelegate force) {

		if (numMasses == 0 && numBodies == 0) {
			return evolveFor;
		}
		// advance acceleration
		double[] rji = new double[GravityEngine.NDIM]; 
		double r2; 	// r squared
		double r_sep; // r
		double accel;
		double time = 0.0; 
		// Built in LeapFrog integrator
		while (time < evolveFor) {
			time += dt; 
			// Update v using a from last cycle (1/2 step), then update r
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.INACTIVE) == 0) {
					for (int k=0; k < GravityEngine.NDIM; k++) {
						v[i,k] += a[i,k] * dt/2.0;
						r[i,k] += v[i,k] * dt;
					}		
				}			
			}
			// a = 0 
			for (int i=0; i < numBodies; i++) {
				a[i,0] = 0.0;
				a[i,1] = 0.0;
				a[i,2] = 0.0;
			}
			// calc a
			for (int i=0; i < numMasses; i++) {
				if ((massive_info[i] & GravityEngine.INACTIVE) == 0) {
					for (int j=0; j < numBodies; j++) {
						if ((info[j] & GravityEngine.INACTIVE) == 0) {
							rji[0] = r[j,0] - r_m[i,0];
							rji[1] = r[j,1] - r_m[i,1];
							rji[2] = r[j,2] - r_m[i,2];

							r2 = 0; 
							r2 += rji[0] * rji[0]; 
							r2 += rji[1] * rji[1]; 
							r2 += rji[2] * rji[2]; 
							r_sep = System.Math.Sqrt(r2) + EPSILON; 
							// Force equation
							accel = force.CalcF(r_sep);
							a[j,0] -= m[i] * accel* (rji[0]/r_sep);
							a[j,1] -= m[i] * accel* (rji[1]/r_sep);
							a[j,2] -= m[i] * accel* (rji[2]/r_sep);
						} // j
					} // if info
				} // i
			}
			// update velocity with other half step
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.INACTIVE) == 0) {
					v[i,0] += a[i,0] * dt/2.0;
					v[i,1] += a[i,1] * dt/2.0;
					v[i,2] += a[i,2] * dt/2.0;
				}				
			}
		}
		return time;
	}

	public void UpdateBodies(float physicalScale) {
		for (int i=0; i < numBodies; i++) {
			if ((info[i] & GravityEngine.INACTIVE) == 0) {
				Vector3 position = new Vector3((float) r[i,0], (float) r[i,1], (float) r[i,2]); 
				bodies[i].transform.position = physicalScale * position;
			}
		}
	}	

	public Vector3 GetVelocity(GameObject body) {
		NBody nbody = body.GetComponent<NBody>();
		int i = nbody.engineRef.index;
		return new Vector3((float) v[i,0], (float) v[i,1], (float) v[i,2]);
	}

	public void SetVelocity(GameObject body, Vector3 velocity) {
		NBody nbody = body.GetComponent<NBody>();
		int i = nbody.engineRef.index;
		v[i,0] = velocity.x;
		v[i,1] = velocity.y;
		v[i,2] = velocity.z;
	}

	public void SetVelocityAtIndex(int i, Vector3 velocity) {
		v[i,0] = velocity.x;
		v[i,1] = velocity.y;
		v[i,2] = velocity.z;
	}

	public void SetPositionAtIndex(int i, Vector3 pos) {
		r[i,0] = pos.x;
		r[i,1] = pos.y;
		r[i,2] = pos.z;
	}

	public Vector3 GetAcceleration(GameObject body) {
		NBody nbody = body.GetComponent<NBody>();
		int i = nbody.engineRef.index;
		return new Vector3((float) a[i,0], (float) a[i,1], (float) a[i,2]);
	}
		
}
