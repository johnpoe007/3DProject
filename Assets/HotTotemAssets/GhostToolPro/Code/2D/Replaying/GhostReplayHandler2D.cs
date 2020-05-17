using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GhostToolPro {
	public class GhostReplayHandler2D : MonoBehaviour {

	public static List<GhostReplayContainer2D> recordedObjects = new List<GhostReplayContainer2D>();
	static List<int> deletePositions = new List<int>();
	void ReplayMovement(int _pos,float _time)
	{
		if (recordedObjects [_pos].replay) {
			foreach (GhostReplayStruct2D _struct in recordedObjects [_pos].replayCollection) {
				if (!_struct.MoveToNext (_time)) {
					if (!deletePositions.Contains (_pos))
						deletePositions.Add (_pos);
				}
			}
		}
	}	
	void Update () {
		for (int i = 0; i < recordedObjects.Count; i++) 
		{
			ReplayMovement (i, Time.time);
		}
		foreach (int i in deletePositions)
			Finished (i);
		deletePositions.Clear ();
	}
	public static string Add(GhostReplayContainer2D _cont)
	{
		recordedObjects.Add (_cont);
		return _cont.uniqueID;
	}

	public static void Finished(int _pos)
	{
		string uniq = recordedObjects[_pos].uniqueID;
		foreach (GhostReplayStruct2D _struct in recordedObjects[_pos].replayCollection) {
			Destroy (_struct.GetGhostObject ());
		}
		recordedObjects.RemoveAt (_pos);
		GhostTool2D.isFinished (uniq);
	}
}
}