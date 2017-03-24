// For touchscreen devices -- draw a line with your finger
#pragma strict
import Vectrosity;
import System.Collections.Generic;

var lineTex : Texture;
var maxPoints = 5000;
var lineWidth = 4.0;
var minPixelMove = 5;	// Must move at least this many pixels per sample for a new segment to be recorded
var useEndCap = false;
var capLineTex : Texture;
var capTex : Texture;
var capLineWidth = 20.0;

private var line : VectorLine;
private var previousPosition : Vector2;
private var sqrMinPixelMove : int;
private var canDraw = false;
private var touch : Touch;

function Start () {
	if (useEndCap) {
		VectorLine.SetEndCap ("RoundCap", EndCap.Mirror, capLineTex, capTex);
		var tex = capLineTex;
		var useLineWidth = capLineWidth;
	}
	else {
		tex = lineTex;
		useLineWidth = lineWidth;
	}
	
	line = new VectorLine("DrawnLine", new List.<Vector2>(), tex, useLineWidth, LineType.Continuous, Joins.Weld);
	line.endPointsUpdate = 1;	// Optimization for updating only the last point of the line, and the rest is not re-computed
	if (useEndCap) {
		line.endCap = "RoundCap";
	}
	// Used for .sqrMagnitude, which is faster than .magnitude
	sqrMinPixelMove = minPixelMove*minPixelMove;
}

function Update () {
	if (Input.touchCount > 0) {
		touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			line.points2.Clear();
			line.Draw();
			previousPosition = touch.position;
			line.points2.Add (touch.position);
			canDraw = true;
		}
		else if (touch.phase == TouchPhase.Moved && (touch.position - previousPosition).sqrMagnitude > sqrMinPixelMove && canDraw) {
			previousPosition = touch.position;
			line.points2.Add (touch.position);
			if (line.points2.Count >= maxPoints) {
				canDraw = false;
			}
			line.Draw();
		}
	}
}