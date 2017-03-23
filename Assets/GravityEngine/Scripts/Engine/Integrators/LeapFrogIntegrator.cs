using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Standard Leapfrog algorithm 
// This is vastly better than the standard Euler approach ( x = x_0 + v dt ) because it is energy conserving
// (in the lingo "symplectic"). 
//
// Mark as sealed - may improve performance depending on compiler...

public sealed class LeapFrogIntegrator : INBodyIntegrator {

	private double dt; 
	private int numBodies; 
	private int maxBodies; 
	
	// per body physical parameters. Second index is the dimension. 
	// These are for massive bodies with interactions and NOT particles
	private double[,] v; 
	private double[,] a; 
	
	private double initialEnergy; 
	
	private const double EPSILON = 1E-4; 	// minimum distance for gravitatonal force

	// working variable for Evolve - allocate once
	private double[] rji;	

	private IForceDelegate forceDelegate;

	/// <summary>
	/// Initializes a new instance of the <see cref="LeapFrogIntegrator"/> class.
	/// An optional force delegate can be provided if non-Newtonian gravity is 
	/// desired.
	/// </summary>
	/// <param name="force">Force.</param>
	public LeapFrogIntegrator(IForceDelegate force) {
		#pragma warning disable 162		// disable unreachable code warning
		if (GravityEngine.DEBUG)
			Debug.Log("Instantiated with " + force);
		#pragma warning restore 162

		forceDelegate = force;
	}

	/// <summary>
	/// Setup the specified maxBodies and timeStep.
	/// </summary>
	/// <param name="maxBodies">Max bodies.</param>
	/// <param name="timeStep">Time step.</param>
	public void Setup(int maxBodies, double timeStep) {
		dt = timeStep; 
		numBodies = 0; 
		this.maxBodies = maxBodies;
		
		v = new double[maxBodies,GravityEngine.NDIM];
		a = new double[maxBodies,GravityEngine.NDIM];

		rji = new double[GravityEngine.NDIM]; 
		
	}
	
	// TODO - If add during sim - would be useful to do a PreEvolve()
	public void AddNBody( int bodyNum, NBody nbody, Vector3 position, Vector3 velocity) {

		if (numBodies > maxBodies) {
			Debug.LogError("Added more than maximum allocated bodies! max=" + maxBodies);
			return;
		}
		if (bodyNum != numBodies) {
			Debug.LogError("Body numbers are out of sync integrator=" + numBodies + " GE=" + bodyNum);
			return;
		}
		// r,m already in GravityEngine
		v[numBodies,0] = velocity.x; 
		v[numBodies,1] = velocity.y; 
		v[numBodies,2] = velocity.z; 
		
		numBodies++;
		
	}
	
	public void RemoveBodyAtIndex(int atIndex) {
	
		// shuffle the rest up + internal info
		for( int j=atIndex; j < (numBodies-1); j++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				v[j,k] = v[j+1, k]; 
				a[j,k] = a[j+1, k]; 
			}
		}
		numBodies--; 
	
	}

	public void GrowArrays(int growBy) {
		double[,] v_copy = new double[maxBodies, GravityEngine.NDIM]; 
		double[,] a_copy = new double[maxBodies, GravityEngine.NDIM];  
		for( int j=0; j < numBodies; j++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				v_copy[j,k] = v[j, k]; 
				a_copy[j,k] = a[j, k]; 
			}
		}
		v = new double[maxBodies+growBy, GravityEngine.NDIM];
		a = new double[maxBodies+growBy, GravityEngine.NDIM];
		for( int j=0; j < numBodies; j++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				v[j,k] = v_copy[j, k]; 
				a[j,k] = a_copy[j, k]; 
			}
		}
		maxBodies += growBy;
	}

	public Vector3 GetVelocityForIndex(int i) {
		return new Vector3( (float)v[i,0], (float)v[i,1], (float)v[i,2]);
	}

	public void SetVelocityForIndex(int i, Vector3 vel) {
		v[i,0] = vel.x;
		v[i,1] = vel.y;
		v[i,2] = vel.z;
	}

	public Vector3 GetAccelerationForIndex(int i) {
		return new Vector3( (float)a[i,0], (float)a[i,1], (float)a[i,2]);
	}

	public void PreEvolve(ref double[] m, ref double[,] r, ref byte[] info) {
		// Precalc initial acceleration
		double[] rji = new double[GravityEngine.NDIM]; 
		double r2; 
		double r3; 
		double r_sep;
		double accel;

		for (int i=0; i < numBodies; i++) {
			for (int k=0; k < GravityEngine.NDIM; k++) {
				a[i,k] = 0.0;
			}
		}
		for (int i=0; i < numBodies; i++) {
			for (int j=i+1; j < numBodies; j++) {
				r2 = 0; 
				for (int k=0; k < GravityEngine.NDIM; k++) {
					rji[k] = r[j,k] - r[i,k];
					r2 += rji[k] * rji[k]; 
				}
				if (forceDelegate == null) {
				r3 = r2 * System.Math.Sqrt(r2) + EPSILON; 
					for (int k=0; k < GravityEngine.NDIM; k++) {
						a[i,k] += m[j] * rji[k]/r3; 
						a[j,k] -= m[i] * rji[k]/r3;
					}
				} else {
					r_sep = System.Math.Sqrt(r2) + EPSILON; 
					accel = forceDelegate.CalcF(r_sep);
					for (int k=0; k < GravityEngine.NDIM; k++) {
						a[i,k] += m[j] * accel * (rji[k]/r_sep);
						a[j,k] -= m[i] * accel * (rji[k]/r_sep);
					}
				}
			}
		}	
		
		initialEnergy = NUtils.GetEnergy(numBodies, ref m, ref r, ref v);
	}
			
	public float GetEnergy(ref double[] m, ref double[,] r) {
		return (float) NUtils.GetEnergy(numBodies, ref m, ref r, ref v);
	}

	public float GetInitialEnergy(ref double[] m, ref double[,] r) {
		return (float) initialEnergy;
	}
	
	public double Evolve(double time, ref double[] m, ref double[,] r, ref byte[] info) {

		if (forceDelegate != null) {
			return EvolveForceDelegate(time, ref m, ref r, ref info);
		}
		int numSteps = 0;

		// If objects are fixed want to use their mass but not update their position
		// Better to calc their acceleration and ignore than add an if statement to core loop. 
		for (double t = 0; t < time; t += dt) {
			numSteps++;
			// Update v and r
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.FIXED_MOTION) == 0) {
					v[i,0] += a[i,0] * dt/2.0;
					r[i,0] += v[i,0] * dt;
					v[i,1] += a[i,1] * dt/2.0;
					r[i,1] += v[i,1] * dt;
					v[i,2] += a[i,2] * dt/2.0;
					r[i,2] += v[i,2] * dt;
				}				
			}
			// advance acceleration
			double r2; 
			double r3; 
			
			// a = 0 
			for (int i=0; i < numBodies; i++) {
				a[i,0] = 0.0;
				a[i,1] = 0.0;
				a[i,2] = 0.0;
			}
			// calc a
			for (int i=0; i < numBodies; i++) {
			   if ((info[i] & GravityEngine.INACTIVE) == 0) {					
			      for (int j=i+1; j < numBodies; j++) {
					 if ((info[j] & GravityEngine.INACTIVE) == 0) {	
					 	// O(N^2) in here, unpack loops to optimize				
						r2 = 0; 
						rji[0] = r[j,0] - r[i,0];
						r2 += rji[0] * rji[0]; 
						rji[1] = r[j,1] - r[i,1];
						r2 += rji[1] * rji[1]; 
						rji[2] = r[j,2] - r[i,2];
						r2 += rji[2] * rji[2]; 
						r3 = r2 * System.Math.Sqrt(r2) + EPSILON; 
						a[i,0] += m[j] * rji[0]/r3; 
						a[j,0] -= m[i] * rji[0]/r3;
						a[i,1] += m[j] * rji[1]/r3; 
						a[j,1] -= m[i] * rji[1]/r3;
						a[i,2] += m[j] * rji[2]/r3; 
						a[j,2] -= m[i] * rji[2]/r3;
					 }
			      }
			   }
			}
			// update velocity
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.FIXED_MOTION) == 0) {
					v[i,0] += a[i,0] * dt/2.0;
					v[i,1] += a[i,1] * dt/2.0;
					v[i,2] += a[i,2] * dt/2.0;
					//DebugBody(i, ref m, ref r, "evolve");			
				}	
			}
			// coll_time code
		}
		return (numSteps * dt);

		
	}
	
	private void DebugBody(int i, ref double[] m, ref double[,] r, string log) {
		Debug.Log (string.Format("{0} x=({1},{2},{3}) v=({4},{5},{6}) a=({7},{8},{9}) m={10}", log + i ,
		                         r[i,0], r[i,1], r[i,2], v[i,0], v[i,1], v[i,2], a[i,0], a[i,1], a[i,2], m[i]));
	}

	/// <summary>
	/// Evolves using the force delegate. Internals differ slightly and for effeciency do not want
	/// a conditional on forceDelegate in the inner loop. 
	///
	/// </summary>
	/// <returns>The force delegate.</returns>
	/// <param name="time">Time.</param>
	/// <param name="m">M.</param>
	/// <param name="r">The red component.</param>
	/// <param name="info">Info.</param>
	private double EvolveForceDelegate(double time, ref double[] m, ref double[,] r, ref byte[] info) {
	
		int numSteps = 0;
		// advance acceleration
		double r2; 
		double r_sep; 
		double f; 

		// If objects are fixed want to use their mass but not update their position
		// Better to calc their acceleration and ignore than add an if statement to core loop. 
		for (double t = 0; t < time; t += dt) {
			numSteps++;
			// Update v and r
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.FIXED_MOTION) == 0) {
					v[i,0] += a[i,0] * dt/2.0;
					r[i,0] += v[i,0] * dt;
					v[i,1] += a[i,1] * dt/2.0;
					r[i,1] += v[i,1] * dt;
					v[i,2] += a[i,2] * dt/2.0;
					r[i,2] += v[i,2] * dt;
				}				
			}

			// a = 0 
			for (int i=0; i < numBodies; i++) {
				a[i,0] = 0.0;
				a[i,1] = 0.0;
				a[i,2] = 0.0;
			}
			// calc a
			for (int i=0; i < numBodies; i++) {
			   if ((info[i] & GravityEngine.INACTIVE) == 0) {					
			      for (int j=i+1; j < numBodies; j++) {
					 if ((info[j] & GravityEngine.INACTIVE) == 0) {	
					 	// O(N^2) in here, unpack loops to optimize				
						r2 = 0; 
						rji[0] = r[j,0] - r[i,0];
						r2 += rji[0] * rji[0]; 
						rji[1] = r[j,1] - r[i,1];
						r2 += rji[1] * rji[1]; 
						rji[2] = r[j,2] - r[i,2];
						r2 += rji[2] * rji[2]; 
						r_sep = System.Math.Sqrt(r2) + EPSILON; 
						f = forceDelegate.CalcF(r_sep);
						a[i,0] += m[j] * f*(rji[0]/r_sep);
						a[i,1] += m[j] * f*(rji[1]/r_sep);
						a[i,2] += m[j] * f*(rji[2]/r_sep);

						a[j,0] -= m[i] * f* (rji[0]/r_sep);
						a[j,1] -= m[i] * f* (rji[1]/r_sep);
						a[j,2] -= m[i] * f* (rji[2]/r_sep);
					 }
			      }
			   }
			}
			// update velocity
			for (int i=0; i < numBodies; i++) {
				if ((info[i] & GravityEngine.FIXED_MOTION) == 0) {
					v[i,0] += a[i,0] * dt/2.0;
					v[i,1] += a[i,1] * dt/2.0;
					v[i,2] += a[i,2] * dt/2.0;
					//DebugBody(i, ref m, ref r, "evolve");			
				}	
			}
			// coll_time code
		}
		return (numSteps * dt);
	}

}
