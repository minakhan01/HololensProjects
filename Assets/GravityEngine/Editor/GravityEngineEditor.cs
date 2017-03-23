using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GravityEngine), true)]
public class GravityEngineEditor : Editor {


	private const string mTip = "Scale applied to all masses in the scene. Increasing will result in larger forces and faster evolution.";
	private const string algoTip = "Integration algorithm used to evolve massive bodies.\nLeapfrog is default. AZTTriple is for exactly three massive bodies.";
	private const string forceTip = "Select the force to be used in the scene.";
	private const string timeTip = "Timescale controls the difference between game time and physics time in Gravity Engine. Larger values result in faster evolution BUT more calculations being performed. ";
	private const string mlessTip = "Evolve massless bodies seperately using a built-in Leapfrog algorithm.";
	private const string ppTip = "Particle accuracy (0=best performance/lower accuracy, 1 = highest accuracy/most CPU)";
	private const string autoStartTip = "Begin evolving bodies as soon as the scene starts. If false then starting is via script API.";
	private const string autoAddTip = "Automatically detect Nbodies in the scene and add them to the Gravity Engine.";
	private const string phyWTip = "Scale factor applied to physics position to scale up to world view. Typically 1 unless initializing from known solutions via scripts (e.g. ThreeBodySolution)";

	private const string stepTip = "Number of integration steps gravity engine will target for each 60fps frame. " + 
				"The value depends in how much accuracy is desired and the masses in the scene. Larger values may impact the frame rate.\n" +
				"Default value is 8.";

	private const string pstepTip = "Number of particle integration steps gravity engine will target for each 60fps frame. " + 
				"The value depends in how much accuracy is desired and the masses in the scene." +
				" Larger values can slow the simulation significantly if many particles are used.\n" + 
				"Default value is 2.";

	private const string unitsTip = "Units specifies the interpretation of the distance and mass\n" +
				"in NBody objects.\n\n" + 
				"GravityEngine uses G=1 and choice of units defines an intrinsic timescale.\n\n" +
				"Game time to physics time can then be specified in the inspector.";

	[MenuItem("GameObject/3D Object/GravityEngine")]
	static void Init()
    {
    	if (FindObjectOfType(typeof(GravityEngine))) {
    		Debug.LogWarning("NBodyEngine already in scene.");
    		return;
    	}
		GameObject nbodyEngine = new GameObject();
		nbodyEngine.name = "GravityEngine";
		nbodyEngine.AddComponent<GravityEngine>();
    }

	public override void OnInspectorGUI()
	{
		GUI.changed = false;
		GravityEngine gravityEngine = (GravityEngine) target;
		float massScale = gravityEngine.massScale; 
		float timeScale = gravityEngine.timeScale; 
		float lengthScale = gravityEngine.lengthScale;

		float physToWorldFactor = gravityEngine.physToWorldFactor; 

		bool optimizeMassless = gravityEngine.optimizeMassless;
		bool detectNbodies = gravityEngine.detectNbodies;
		bool evolveAtStart = gravityEngine.evolveAtStart;
		bool scaling = gravityEngine.editorShowScale;
		bool showAdvanced = gravityEngine.editorShowAdvanced;
		bool cmFoldout = gravityEngine.editorCMfoldout;

		int stepsPerFrame = gravityEngine.stepsPerFrame;
		int particleStepsPerFrame = gravityEngine.particleStepsPerFrame;

		GravityEngine.Algorithm algorithm = GravityEngine.Algorithm.LEAPFROG;
		ForceChooser.Forces force = ForceChooser.Forces.Gravity;
		GravityScaler.Units units = GravityScaler.Units.DIMENSIONLESS;

		EditorGUIUtility.labelWidth = 150;

		scaling = EditorGUILayout.Foldout(scaling, "Scaling");
		if (scaling) {
			units =
				(GravityScaler.Units)EditorGUILayout.EnumPopup(new GUIContent("Units", unitsTip), gravityEngine.units);
//			float lscale = 0f;
			switch(units) {
			case GravityScaler.Units.DIMENSIONLESS:
					// only have mass scale in DL case
					massScale = EditorGUILayout.FloatField(new GUIContent("Mass Scale", mTip), gravityEngine.massScale);
					timeScale = EditorGUILayout.FloatField(new GUIContent("Time Scale", timeTip), gravityEngine.timeScale);
					break;
			case GravityScaler.Units.SI:
					// no mass scale is controlled by time exclusivly
					EditorGUILayout.LabelField("m/kg/sec.");
					// meters per Unity unit in the case of meters
					lengthScale = EditorGUILayout.DelayedFloatField(new GUIContent("m per Unity unit", timeTip), 1f/gravityEngine.lengthScale);
					timeScale = EditorGUILayout.DelayedFloatField(new GUIContent("Game sec. per sec.", timeTip), gravityEngine.timeScale);
					break;
			case GravityScaler.Units.ORBITAL:
					EditorGUILayout.LabelField("km/1E24 kg/hr");
					// Express in km per Unity unit km/U
					lengthScale = EditorGUILayout.DelayedFloatField(new GUIContent("Unity unit per km", timeTip), gravityEngine.lengthScale);
					timeScale = EditorGUILayout.DelayedFloatField(new GUIContent("Game sec. per hour", timeTip), gravityEngine.timeScale);
					break;
			case GravityScaler.Units.SOLAR:
					EditorGUILayout.LabelField("AU/1E24 kg/year");
					lengthScale = EditorGUILayout.DelayedFloatField(new GUIContent("Unity unit per AU", timeTip), gravityEngine.lengthScale);
					timeScale = EditorGUILayout.DelayedFloatField(new GUIContent("Game Sec per year", timeTip), gravityEngine.timeScale);
					break;
			}
		}

		showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced");
		if (showAdvanced) {
			algorithm =
				(GravityEngine.Algorithm)EditorGUILayout.EnumPopup(new GUIContent("Algorithm", algoTip), gravityEngine.algorithm);

			// Force selection is tangled with choice of integrator and scale - so needs to be here
			force = 
				(ForceChooser.Forces)EditorGUILayout.EnumPopup(new GUIContent("Force", forceTip), gravityEngine.force);
			if (force == ForceChooser.Forces.Custom) {
				IForceDelegate force_delegate = gravityEngine.GetComponent<IForceDelegate>();
				if (force_delegate == null) {
					EditorGUILayout.LabelField("  Attach a Force Delegate to this object.", EditorStyles.boldLabel);
				} else {
					EditorGUILayout.LabelField("  Force delegate: " + force_delegate.GetType());
				}
			}
			if (force != ForceChooser.Forces.Gravity) {
				EditorGUILayout.LabelField("    Note: Orbit predictors assume Newtonian gravity");
				EditorGUILayout.LabelField("    They are not accurate for other forces.");
			}
			
			optimizeMassless = EditorGUILayout.Toggle(new GUIContent("Optimize Massless Bodies", mlessTip), 
					gravityEngine.optimizeMassless);
			detectNbodies = EditorGUILayout.Toggle(new GUIContent("Automatically Add NBody objects", autoAddTip), gravityEngine.detectNbodies);
			evolveAtStart = EditorGUILayout.Toggle(new GUIContent("Evolve at Start", autoStartTip), gravityEngine.evolveAtStart);
			physToWorldFactor = EditorGUILayout.FloatField(new GUIContent("Physics to World Scale", phyWTip), gravityEngine.physToWorldFactor);

			stepsPerFrame = EditorGUILayout.IntField(new GUIContent("Physics steps per frame", stepTip), stepsPerFrame);
			particleStepsPerFrame = EditorGUILayout.IntField(new GUIContent("Physics (particles) steps per frame", pstepTip), particleStepsPerFrame);
		}
		// Switch bodies list on/off based on option 
		if (!gravityEngine.detectNbodies) {
			// use native Inspector look & feel for bodies object
			EditorGUILayout.LabelField("Control Nbody in following gameObjects (and children)", EditorStyles.boldLabel);
         	SerializedProperty bodiesProp = serializedObject.FindProperty ("bodies");
         	EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(bodiesProp, true);
         	if(EditorGUI.EndChangeCheck())
             	serializedObject.ApplyModifiedProperties();
		} else {
			EditorGUILayout.LabelField("NBody objects will be detected automatically", EditorStyles.boldLabel);
		}
		// Show the CM and the velocity of the CM
		cmFoldout = EditorGUILayout.Foldout(cmFoldout, "Center of Mass Info");
		if (cmFoldout) {
			EditorGUILayout.LabelField("Center of Mass:" + gravityEngine.GetWorldCenterOfMass());
			EditorGUILayout.LabelField("CM Velocity:" + gravityEngine.GetWorldCenterOfMassVelocity());
		}


		// Checking the Event type lets us update after Undo and Redo commands.
		// A redo updates _lengthScale but does not run the setter
        if (Event.current.type == EventType.ExecuteCommand &&
            Event.current.commandName == "UndoRedoPerformed") {
            // explicitly re-set so setter code will run. 
			gravityEngine.lengthScale = lengthScale;
        }

		if (GUI.changed) {
			Undo.RecordObject(gravityEngine, "GE Change");
			gravityEngine.timeScale = timeScale; 
			gravityEngine.massScale = massScale; 
			gravityEngine.lengthScale = lengthScale;
			gravityEngine.units = units;
			gravityEngine.physToWorldFactor = physToWorldFactor; 
			gravityEngine.optimizeMassless = optimizeMassless; 
			gravityEngine.detectNbodies = detectNbodies;
			gravityEngine.algorithm = algorithm;
			gravityEngine.force = force;
			gravityEngine.evolveAtStart = evolveAtStart;
			gravityEngine.editorShowScale = scaling; 
			gravityEngine.editorShowAdvanced = showAdvanced;
			gravityEngine.editorCMfoldout = cmFoldout;
			gravityEngine.stepsPerFrame = stepsPerFrame;
			gravityEngine.particleStepsPerFrame = particleStepsPerFrame;
			EditorUtility.SetDirty(gravityEngine);
		}

	}
}
