using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GhostToolPro {
	public class GhostObject : MonoBehaviour {
		public static List<GhostObject> GhostObjects = new List<GhostObject>();
		public string ghostId,objectId = (Guid.NewGuid()).ToString();
		public void Init(string _id)
		{
			ghostId = _id;
			if(!GhostObjects.Any(i=>i.objectId == objectId))
			{
				GhostObjects.Add (this);
			}
		}
	}
}