using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NutilsTest {

    [Test]
    public void AngleFromSinCos()
    {
    	// check each quadrant
		float[] angles = { 0, 0.3f*Mathf.PI, 0.7f*Mathf.PI, 1.3f*Mathf.PI, 1.7f*Mathf.PI};
        foreach (float angle in angles ) {
			float a = NUtils.AngleFromSinCos( Mathf.Sin(angle), Mathf.Cos(angle));
			Debug.Log( "angle=" + angle + " a=" + a);
			Assert.IsTrue( Mathf.Abs(angle - a) < 1E-2);
        }
    }

	[Test]
    public void Mod360()
    {
    	// check each quadrant
		float[] angles = { -90f, 20f, 355f, 270f+2*360f, 14f*360f+5f};
		float[] answer = { 270f, 20f, 355f, 270f, 5f};
        for (int i=0; i < angles.Length; i++) {
			float a = NUtils.DegressMod360( angles[i]);
			Debug.Log( "angle=" + angles[i] + " a=" + a);
			Assert.IsTrue( Mathf.Abs(a - answer[i]) < 1E-2);
        }
    }
}
