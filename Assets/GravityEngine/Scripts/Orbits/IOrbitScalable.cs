﻿using UnityEngine;
using System.Collections;

public interface IOrbitScalable  {

	/// <summary>
	/// Interface to apply the scale to the distance paramter of an orbit. 
	/// </summary>
	/// <param name="scale">Scale.</param>
	void ApplyScale(float scale);

}
