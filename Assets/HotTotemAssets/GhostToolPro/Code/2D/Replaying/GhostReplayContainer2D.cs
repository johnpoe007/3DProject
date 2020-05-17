using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GhostToolPro {
	public class GhostReplayContainer2D {
	public string name;
	public string uniqueID = Guid.NewGuid().ToString();
	public bool replay = false;
	public List<GhostReplayStruct2D> replayCollection = new List<GhostReplayStruct2D>();
	public GhostReplayContainer2D(GhostReplayStruct2D _struct)
	{
		name = _struct.name;
		replayCollection.Add (_struct);
	}
	public GhostReplayContainer2D(GhostRecordContainer2D _struct)
	{
		name = _struct.name;
		foreach (GhostRecordStruct2D _str in _struct.recordCollection) {
				replayCollection.Add (new GhostReplayStruct2D (_str));
		}
	}
	public GhostReplayContainer2D(GhostReplayContainer2D _copy)
	{
		name = _copy.name;
		if (_copy.replayCollection [0].ghostObject.GetComponent<SpriteRenderer> ()) {
			foreach (GhostReplayStruct2D _str in _copy.replayCollection) {
				if (_str.ghostObject != null)
					replayCollection.Add (new GhostReplayStruct2D (_str));
			}
		}
	}
	public GhostReplayContainer2D(List<GhostReplayStruct2D> _structList)
	{
		name = _structList[0].name;
		replayCollection = _structList;
	}
}
}