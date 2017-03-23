using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(OrbitHyper), true)]
public class OrbitHyperEditor : Editor {

	private static string eTip = "Hyperbola eccentricity (e > 1)";
	private static string pTip = "Distance from focus to closest approach";
	private static string centerTip = "Object at focus (center) of orbit";
	private static string omega_lcTip = "Rotation of pericenter from ascending node\nWhen inclination=0 will act in the same way as \u03a9";
	private static string omega_ucTip = "Rotation of ellipse from x-axis (degrees)\nWhen inclination=0 will act in the same way as \u03c9";
	
	private static string inclinationTip = "Inclination angle of ellipse to x-y plane (degrees)";
	private static string phaseTip = "Initial position specified by distance from focus. Inbound by default.";
	private static string obTip = "Initial position on outbound leg";

	public override void OnInspectorGUI()
	{
		GUI.changed = false;
		OrbitHyper hyperBase = (OrbitHyper) target;
		// fields in class
		GameObject centerObject = null;
		float ecc = 0; 
		float perihelion = 0; 
		float omega_uc = 0; 
		float omega_lc = 0; 
		float inclination = 0; 
		float r_initial = 0; 
		bool r_initial_outbound = false;

		if (!(target is BinaryPair)) {
			centerObject = (GameObject) EditorGUILayout.ObjectField(
				new GUIContent("CenterObject", centerTip), 
				hyperBase.centerObject,
				typeof(GameObject), 
				true);
		}

		EditorGUIUtility.labelWidth = 200;
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Ellipse Parameters", EditorStyles.boldLabel);


		ecc = EditorGUILayout.FloatField(new GUIContent("Eccentricity", eTip), hyperBase.ecc );
		if (ecc <= 1f) {
			ecc = 1.01f;
		}

		perihelion = EditorGUILayout.FloatField(new GUIContent("Perihelion", pTip), hyperBase.perihelion);
		// implementation uses AngleAxis, so degrees are more natural
		omega_uc = EditorGUILayout.Slider(new GUIContent("\u03a9 (Longitude of AN)", omega_ucTip), hyperBase.omega_uc, 0, 360f);
		omega_lc = EditorGUILayout.Slider(new GUIContent("\u03c9 (AN to Pericenter)", omega_lcTip), hyperBase.omega_lc, 0, 360f);
		inclination = EditorGUILayout.Slider(new GUIContent("Inclination", inclinationTip), hyperBase.inclination, 0f, 180f);
		r_initial = EditorGUILayout.FloatField(new GUIContent("Initial Distance", phaseTip), hyperBase.r_initial);
		if (r_initial < perihelion)
			r_initial = perihelion;
		r_initial_outbound = EditorGUILayout.Toggle(new GUIContent("Initial Distance Outbound", obTip), hyperBase.r_initial_outbound);

		if (GUI.changed) {
			Undo.RecordObject(hyperBase, "OrbitHyper Change");
			hyperBase.perihelion = perihelion; 
			hyperBase.ecc = ecc; 
			hyperBase.centerObject = centerObject;
			hyperBase.omega_lc = omega_lc;
			hyperBase.omega_uc = omega_uc;
			hyperBase.inclination = inclination;
			hyperBase.r_initial = r_initial;
			hyperBase.r_initial_outbound = r_initial_outbound;
			EditorUtility.SetDirty(hyperBase);
		}		
	}
}
