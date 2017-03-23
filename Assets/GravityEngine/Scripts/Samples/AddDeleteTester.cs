using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Add/delete tester.
/// Create and remove massive and massless planet prefabs (with attached OrbitEllipse) at 
/// </summary>
public class AddDeleteTester : MonoBehaviour {

	public GameObject orbitingPrefab;
	public GameObject star;

	private const float MAX_RADIUS= 30f;
	private const float MIN_RADIUS = 5f;

	private List<GameObject> massiveObjects; 
	private List<GameObject> masslessObjects; 

	private Color[] colors = { Color.red, Color.white, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.yellow};
	private int colorIndex = 0; 

	// Use this for initialization
	void Awake () {
		massiveObjects = new List<GameObject>();
		masslessObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddBody(string bodyType) {

		GameObject go = Instantiate(orbitingPrefab) as GameObject;
		go.transform.parent = star.transform;
		NBody nbody = go.GetComponent<NBody>();
		if (bodyType == "massless") {
			nbody.mass = 0f;
			masslessObjects.Add(go);
		} else {
			nbody.mass = 1f;	// nominal
			massiveObjects.Add(go);
		}
		OrbitEllipse eb = go.GetComponent<OrbitEllipse>();
		if (eb == null) {
			Debug.LogError("Failed to get EllipseStart from prefab");
			return;
		}
		eb.paramBy = EllipseBase.ParamBy.AXIS_A;
		eb.a = Random.Range(MIN_RADIUS, MAX_RADIUS);
		eb.inclination = Random.Range(-30f, 30f);
		eb.Init();
		TrailRenderer[] trail = go.GetComponentsInChildren<TrailRenderer>(); 
		trail[0].material.color = colors[colorIndex];
		colorIndex = (colorIndex+1)%colors.Length;
		GravityEngine.instance.AddBody(go);

	}

	public void RemoveBody(string bodyType) {

		List<GameObject> bodyList = null;

		if (bodyType == "massless") {
			bodyList = masslessObjects; 
		} else {
			bodyList = massiveObjects;
		}
		if (bodyList.Count > 0) {
			int entry = (int)(Random.Range(0, (float) bodyList.Count));
			GameObject toDestroy = bodyList[entry];
			GravityEngine.instance.RemoveBody(toDestroy);
			bodyList.RemoveAt(entry);
			Destroy(toDestroy);
		} else {
			Debug.Log("All objects of that type removed.");
		}

	}
}
