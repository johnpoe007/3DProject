using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GhostToolPro {
	public class GhostObject2D : MonoBehaviour {
	public static List<GhostObject2D> GhostObjects = new List<GhostObject2D>();
	public string ghostId,objectId = (Guid.NewGuid()).ToString();
	public List<Sprite> sprites;
	public void Init(string _id,List<Sprite> _sprites)
	{
		ghostId = _id;
		sprites = _sprites;
		if(!GhostObjects.Any(i=>i.objectId == objectId))
		{
			GhostObjects.Add (this);
		}
	}
	public Sprite GetSprite(string _name)
	{
		return sprites.Where (p => p.name == _name).FirstOrDefault();
	}
}
}