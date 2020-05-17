using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GhostToolPro
{
	public class GhostReplayHandler : MonoBehaviour
	{

		public static Dictionary<string,GhostReplayContainer> recordedObjects = new Dictionary<string,GhostReplayContainer> ();
		static List<string> deletePositions = new List<string> ();

		void ReplayMovement (string _pos, float _time)
		{
			if (recordedObjects [_pos].replay) {
				foreach (GhostReplayStruct _struct in recordedObjects [_pos].replayCollection) {
					if (!_struct.MoveToNext (_time)) {
						if (!deletePositions.Contains (_pos))
							deletePositions.Add (_pos);
					}
				}
			}
		}

		void Update ()
		{
			foreach (var key in recordedObjects.Keys) {
				ReplayMovement (key, Time.time);
			}
			foreach (string i in deletePositions)
				Finished (i);
			deletePositions.Clear ();
		}

		public static string Add (GhostReplayContainer _cont)
		{
			if(recordedObjects.ContainsKey(_cont.uniqueID)){
				Debug.LogError("GhosToolPro eC201: Something went wrong");
				return "";
			}
			recordedObjects [_cont.uniqueID] = _cont;
			return _cont.uniqueID;
		}

		public static void Finished (string _pos)
		{
			// Deleting objects from list makes them shift in index --> differnet way to delete (Maybe dict with ID ) 
			foreach (GhostReplayStruct _struct in recordedObjects[_pos].replayCollection) {
				var obj  = _struct.GetGhostObject ();
				if (obj.transform.parent.name == "BonesParent" || obj.transform.parent.name == "MeshParent") {
					Destroy (obj.transform.parent.gameObject);
				}
				Destroy (obj);
			}
			recordedObjects.Remove (_pos);
			GhostTool.isFinished (_pos);
		}
	}
}