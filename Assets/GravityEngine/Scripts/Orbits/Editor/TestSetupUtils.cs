using UnityEngine;
using System.Collections;

public class TestSetupUtils : MonoBehaviour {

	public static GameObject CreateNBody(float mass, Vector3 position) {
		GameObject gameObject = new GameObject();
		gameObject.transform.position = position;

		NBody nbody = gameObject.AddComponent<NBody>();
		nbody.mass = mass;
		return gameObject;
	}

	// Create a planet in orbit around center object with semi-major axis a
	public static GameObject CreatePlanetInOrbit(GameObject center, float mass, float a) {
		// position will be trumped by orbit
		GameObject planet = CreateNBody(mass, new Vector3(1,0,0));

		OrbitEllipse orbitEllipse = planet.AddComponent<OrbitEllipse>();
		orbitEllipse.a = a;
		orbitEllipse.SetCenterBody(center);
		orbitEllipse.ApplyScale(1f);
		orbitEllipse.InitNBody(1f, 1f);
		return planet;
	}

	// Create a planet in orbit around center object with semi-major axis a
	public static GameObject CreatePlanetInHyper(GameObject center, float mass) {
		// position will be trumped by orbit
		GameObject planet = CreateNBody(mass, new Vector3(1,0,0));

		OrbitHyper orbitHyper = planet.AddComponent<OrbitHyper>();
		orbitHyper.SetCenterBody(center);
		orbitHyper.InitNBody(1f, 1f);
		return planet;
	}
}
