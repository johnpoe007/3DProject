using UnityEngine;
using System.Collections;

namespace GhostToolPro {
	public class GhostBone : MonoBehaviour {
		[SerializeField]
		public int position = -1;
		public int size = 0;
		[SerializeField]
		[HideInInspector]
		public string id;
		[SerializeField]
		[HideInInspector]
		public string ObjectID;
	}
}
