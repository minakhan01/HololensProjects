using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class HyperDataTest {

	private static bool FloatEqual(float a, float b) {
		return (Mathf.Abs(a-b) < 1E-3); 
	}

	private static bool FloatEqual(float a, float b, double error) {
		return (Mathf.Abs(a-b) < error); 
	}

    // HYPERBOLAS

	[Test]
    public void HyperBasic()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		// Try some values of om
		float[] eccValues = { 1.1f, 1.3f, 2f, 2.2f, 3f, 10f};
		foreach (float ecc in eccValues) {
			orbitHyper.ecc = ecc ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("ecc = " + ecc + " od.ecc=" + od.ecc);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(ecc, od.ecc, 0.02) );
		}
    }

	[Test]
    public void HyperInclination()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.r_initial = 20f;

		// Try some values of om
		float[] inclValues = { 30f, 45f, 60f, 90f, 135f, 180f, 0f};
		foreach (float incl in inclValues) {
			orbitHyper.inclination = incl ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("incl = " + incl + " od.incl=" + od.inclination);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(incl, od.inclination, 0.02) );
		}
    }

	[Test]
    public void HyperOmegaLNoInclNoPhase()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
        const float orbitPeri = 15f; 
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.perihelion = orbitPeri;
		orbitHyper.r_initial = orbitPeri;

		// Try some values of om
		float[] omegaValues = { 30f, 45f, 60f, 90f, 135f, 180f, 0f};
		foreach (float omega in omegaValues) {
			orbitHyper.omega_lc = omega ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("omega = " + omega + " od.omega_l=" + od.omega_lc);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(omega, od.omega_lc, 0.05) );
			TestRV(od, planet, star);
		}
    }

	[Test]
    public void HyperPhaseNoIncl()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
        const float orbitPeri = 15f; 
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.perihelion = orbitPeri;
		orbitHyper.r_initial = orbitPeri;

		// Try some values of om
		float[] rinit_values = { orbitPeri, orbitPeri+2f, orbitPeri+5f, orbitPeri+10f, orbitPeri+20f};
		foreach (float rinit in rinit_values) {
			orbitHyper.r_initial = rinit ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("rinit = " + rinit + " od.r_initial=" + od.r_initial);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(rinit, od.r_initial, 0.02) );
			//TestRV(od, planet, star);
		}
    }

	[Test]
    public void HyperOmegaLNoIncl()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.r_initial = 20f;

		// Try some values of om
		float[] omegaValues = { 30f, 45f, 60f, 90f, 135f, 180f, 0f};
		foreach (float omega in omegaValues) {
			orbitHyper.omega_lc = omega ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("omega = " + omega + " od.omega_l=" + od.omega_lc);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(omega, od.omega_lc, 0.02) );
			TestRV(od, planet, star);
		}
    }

	[Test]
    public void HyperOmegaLIncl()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.r_initial = 20f;
		orbitHyper.inclination = 40f;

		// Try some values of om
		float[] omegaValues = { 30f, 45f, 60f, 90f, 135f, 180f, 1f, 358f};
		foreach (float omega in omegaValues) {
			orbitHyper.omega_lc = omega ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("omega = " + omega + " od.omega_l=" + od.omega_lc);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(omega, od.omega_lc, 0.02) );
		}
    }

	[Test]
    public void HyperOmegaUInclNoPhase()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.perihelion = 20f;
		orbitHyper.r_initial = 20f;
		orbitHyper.inclination = 35f;

		// Try some values of om
		float[] omegaValues = { 30f, 45f, 60f, 90f, 135f, 180f, 210f, 275f,  355f};
		foreach (float omega in omegaValues) {
			orbitHyper.omega_uc = omega ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("omega = " + omega + " od.omega_uc=" + od.omega_uc);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(omega, od.omega_uc, 0.05) );
		}
    }
	[Test]
    public void HyperOmegaUIncl()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.ecc = 1.4f;
		orbitHyper.r_initial = 20f;
		orbitHyper.inclination = 35f;

		// Try some values of om
		float[] omegaValues = { 30f, 45f, 60f, 90f, 135f, 180f, 210f, 275f,  355f};
		foreach (float omega in omegaValues) {
			orbitHyper.omega_uc = omega ; 
			orbitHyper.Init();
			orbitHyper.InitNBody(1f, 1f);
			orbitHyper.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
			OrbitData od = new OrbitData();
			od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
			Debug.Log("omega = " + omega + " od.omega_uc=" + od.omega_uc);
			// Need a bit of leeway at 0 with error
			Assert.IsTrue( FloatEqual(omega, od.omega_uc, 0.02) );
		}
    }

	[Test]
    public void HyperInclOmega()
    {
        const float mass = 1000f; 
        GameObject star = TestSetupUtils.CreateNBody(mass, new Vector3(0,0,0));
		GameObject planet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper orbitHyper = planet.GetComponent<OrbitHyper>();
		orbitHyper.r_initial = 20f;
		orbitHyper.ecc = 2.5f;

		// Try some values of phase
		float[] inclinationValues = { 0f, 30f, 45f, 60f, 90f, 135f, 180f};
		float[] omegaUValues = { 0f, 30f, 45f, 60f, 90f, 135f, 180f, 210f, 320f};
		foreach (float incl in inclinationValues) {
			foreach (float omegau in omegaUValues) {
				orbitHyper.inclination = incl ; 
				orbitHyper.omega_uc = omegau;
				orbitHyper.Init();
				orbitHyper.InitNBody(1f, 1f);
				orbitHyper.Log("Initial circle:");
				OrbitData od = new OrbitData();
				od.SetOrbitForVelocity(planet.GetComponent<NBody>(), star.GetComponent<NBody>());
				Debug.Log("incl = " + incl + " od.incl=" + od.inclination);
				Debug.Log("omegaU = " + omegau + " od.omegau=" + od.omega_uc);
				TestRV(od, planet, star);
			}
		}
    }

    private void TestRV(OrbitData od, GameObject planet, GameObject star)  {
    	Vector3 r_initial = planet.transform.position;
		Vector3 v_initial = planet.GetComponent<NBody>().vel;

		GameObject testPlanet = TestSetupUtils.CreatePlanetInHyper(star, 1f);
		OrbitHyper testHyper = testPlanet.GetComponent<OrbitHyper>();
		testHyper.InitFromOrbitData( od);
		testHyper.Init();
		testHyper.InitNBody(1f, 1f);
		Vector3 r = testPlanet.transform.position;
		Vector3 v = testPlanet.GetComponent<NBody>().vel;
		Debug.Log(" r_i=" + r_initial + " r=" + r + " delta=" + Vector3.Distance(r_initial, r));
		Debug.Log(" v_i=" + v_initial + " v=" + v + " delta=" + Vector3.Distance(v_initial, v));
		Assert.IsTrue( FloatEqual( Vector3.Distance(r_initial, r), 0f, 5E-2));
		Assert.IsTrue( FloatEqual( Vector3.Distance(v_initial, v), 0f, 5E-2));

    }

}
