using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GhostToolPro
{
	public class GhostReplayContainer
	{
		public string name;
		public string uniqueID = Guid.NewGuid ().ToString ();
		public bool replay = false;
		public List<GhostReplayStruct> replayCollection = new List<GhostReplayStruct> ();

		public GhostReplayContainer (GhostReplayStruct _struct)
		{
			name = _struct.name;
			replayCollection.Add (_struct);
		}

		public GhostReplayContainer (GhostRecordContainer _struct)
		{
			name = _struct.name;
			for (int i = 0; i < _struct.recordCollection.Count; i++) {
				if (_struct.recordCollection [i].trackedObject != null && _struct.recordCollection [i].trackedObject.isSkinned) {
					replayCollection.Add (new GhostReplayStruct (_struct.recordCollection [i]));
					var rootBone = replayCollection [i].ghostObject.GetComponent<SkinnedMeshRenderer> ().rootBone;
					var allBones = rootBone.GetComponentsInChildren<Transform> (true);
					int k = 0;
					foreach (var _b in allBones) {
						var _ghostBoneComponentInBone = _b.GetComponent<GhostBone> ();
						if (_ghostBoneComponentInBone != null && (k + i + 1 < _struct.recordCollection.Count)) {
							replayCollection.Add (new GhostReplayStruct (_struct.recordCollection [k + i + 1]));
							replayCollection [k + i + 1].ghostObject = _b.gameObject;
							k++;
						}
					}
				} else {
					if (_struct.recordCollection [i].trackedObject != null && !_struct.recordCollection [i].trackedObject.isBone) {
						replayCollection.Add (new GhostReplayStruct (_struct.recordCollection [i]));
					}
				}
			}
		}

		public GhostReplayContainer (GhostReplayContainer _copy)
		{
			name = _copy.name;
			for (int i = 0; i < _copy.replayCollection.Count; i++) {
				if (_copy.replayCollection [i].ghostObject.GetComponent<SkinnedMeshRenderer> () != null) {
					replayCollection.Add (new GhostReplayStruct (_copy.replayCollection [i]));
					var rootBone = replayCollection [i].ghostObject.GetComponent<SkinnedMeshRenderer> ().rootBone;
					var allBones = rootBone.GetComponentsInChildren<Transform> (true);
					int k = 0;
					foreach (var _b in allBones) {
						var _ghostBoneComponentInBone = _b.GetComponent<GhostBone> ();
						if (_ghostBoneComponentInBone != null && (k + i + 1 < _copy.replayCollection.Count)) {
							replayCollection.Add (new GhostReplayStruct (_copy.replayCollection [k + i + 1]));
							replayCollection [k + i + 1].ghostObject = _b.gameObject;
							k++;
						}
					}
				} else {
					if (_copy.replayCollection [i].ghostObject.GetComponent<MeshRenderer> () != null) {
						replayCollection.Add (new GhostReplayStruct (_copy.replayCollection [i]));
					}
				}
			}
		}

		public GhostReplayContainer (List<GhostReplayStruct> _structList)
		{
			name = _structList [0].name;
			replayCollection = _structList;
		}
		public void Interpolate(){
			foreach (var replayStruct in replayCollection) {
				replayStruct.Interpolate ();
			}
		}
	}
}