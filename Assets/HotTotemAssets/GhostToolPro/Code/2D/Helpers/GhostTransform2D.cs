using UnityEngine;
using System.Collections;
using System;

namespace GhostToolPro {
	[Serializable]
	public struct GhostTransform2D
{
	public Vector3 position,scale,rotation;
	public string spriteName;
	public bool isEnabled;
	public GhostTransform2D (Vector3 _pos, Vector3 _scale, Vector3 _rot,bool _isEnabled,string _name)
	{
		isEnabled = _isEnabled;
		spriteName = _name;
		position = _pos;
		scale = _scale;
		rotation = _rot;
	}
	public GhostTransform2D (GhostTransform2D a,GhostTransform2D b, float t)
	{
		isEnabled = b.isEnabled;
		position = Vector3.Lerp (a.position, b.position, t);
		spriteName = a.spriteName;
		scale = Vector3.Lerp (a.scale, b.scale, t);
		rotation = Vector3.Lerp (a.rotation, b.rotation, t);
	}
}
}