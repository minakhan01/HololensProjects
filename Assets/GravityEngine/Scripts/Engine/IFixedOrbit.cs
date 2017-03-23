using UnityEngine;
using System.Collections;


/// <summary>
/// Interface defining the fixed motion of an NBody object. 
///
/// Scripts implementing this interface must be attached to a game object that also has an NBody component.
///
/// Objects with fixed motion will have their mass
/// used to create the global gravitational field but will not be affected by that field. There motion is
/// defined by the Evolve() method. When called they are responsible for updating their position. 
/// </summary>
public interface IFixedOrbit  {

	/// <summary>
	/// Check if body is configured at scene start to be fixed. (Allows objects to be optionally configured
	/// as not fixed, to allow e.g. Kepler eqn vs initial velocity in OrbitEllipse)
	/// </summary>
	/// <returns><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</returns>
	bool IsFixed();

	/// <summary>
	/// Called for each NBody object prior to evolution beginning. Allows a chance to setup internal state. 
	/// </summary>
	/// <param name="physicalScale">Physical scale.</param>
	/// <param name="massScale">Mass scale.</param>
	void PreEvolve(float physicalScale, float massScale);

	/// <summary>
	/// Evolve the NBody. Implementating method uses physics time and scale to compute the new position, 
	/// placing it in r.
	/// </summary>
	/// <param name="physicsTime">Current Physics time.</param>
	/// <param name="physicalScale">Physical scale.</param>
	/// <param name="r">Position in physics space (x, y, z). OUTPUT by the method </param>
	void Evolve(float physicsTime, float physicalScale, ref float[] r);
}
