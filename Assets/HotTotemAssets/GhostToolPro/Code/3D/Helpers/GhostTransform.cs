using UnityEngine;
using System.Collections;
using System;

namespace GhostToolPro {
	[Serializable]
	public struct GhostTransform
	{
		public Vector3 position,scale,rotation;
		public bool isEnabled;
		public float[] shapes;
		public GhostTransform (Vector3 _pos, Vector3 _scale, Vector3 _rot,bool _isEnabled)
		{
			isEnabled = _isEnabled;
			position = _pos;
			scale = _scale;
			rotation = _rot;
			shapes = null;
		}
		public GhostTransform (Vector3 _pos, Vector3 _scale, Vector3 _rot,bool _isEnabled,float[] _shapes)
		{
			isEnabled = _isEnabled;
			position = _pos;
			scale = _scale;
			rotation = _rot;
			shapes = _shapes;
		}

		//Lerping check rotation maybe something wrong
		public GhostTransform (GhostTransform a,GhostTransform b, float t)
		{
			isEnabled = b.isEnabled;
			position = Vector3.Lerp (a.position, b.position, t);
			scale = Vector3.Lerp (a.scale, b.scale, t);
			var quatRot = Quaternion.Lerp (Quaternion.Euler (a.rotation), Quaternion.Euler (b.rotation), t);
			rotation = quatRot.eulerAngles;
			shapes = null;
			if (a.shapes != null && b.shapes != null) {
				shapes = new float[a.shapes.Length];
				for (int i = 0; i < a.shapes.Length; i++) {
						shapes[i] = Mathf.Lerp(a.shapes[i],b.shapes[i],t);
				}
			}
		}
	}
}