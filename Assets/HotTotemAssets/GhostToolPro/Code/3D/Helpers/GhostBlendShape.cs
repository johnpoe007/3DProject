using UnityEngine;
using System.Collections;
using System;

namespace GhostToolPro {
	[Serializable]
	public struct GhostBlendShape
	{
		public float[] weights;
		public GhostBlendShape (float[] _weights)
		{
			weights = _weights;
		}
	}
}