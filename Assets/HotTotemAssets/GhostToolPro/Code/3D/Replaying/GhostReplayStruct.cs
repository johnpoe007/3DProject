using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GhostToolPro
{
	public class GhostReplayStruct
	{
		public string name;
		public GameObject ghostObject;
		private MeshRenderer myMesh;
		private SkinnedMeshRenderer mySkinned;
		public float started;
		public int skipStep;
		public float paused = 0f;
		private Dictionary<float,GhostTransform> ghostMovements = new Dictionary<float, GhostTransform> ();
		private List<float> timeSteps;

		public GhostReplayStruct (GhostRecordStruct _recording)
		{
			name = _recording.name;
			skipStep = _recording.skipStep;
			ghostMovements = new Dictionary<float, GhostTransform>(_recording.ghostMovements);
			if (_recording.trackedObject != null) {
				if (!_recording.trackedObject.isBone) {
					ghostObject = Ghostable.InstantiateSavedMesh (
						_recording.trackedObject.id,
						_recording.trackedObject.materialsPath,
						_recording.trackedObject.isSkinned);
					if (_recording.trackedObject.isSkinned) {
						mySkinned = ghostObject.GetComponent<SkinnedMeshRenderer> ();
					} else {
						myMesh = ghostObject.GetComponent<MeshRenderer> ();
					}
				} 
			}
			timeSteps = ghostMovements.Keys.ToList ();
			//timeSteps.Sort((a, b) => a.CompareTo(b));
		}

		public GhostReplayStruct (GhostReplayStruct _copy)
		{
			name = _copy.name;
			skipStep = _copy.skipStep;
			ghostMovements = new Dictionary<float, GhostTransform>(_copy.ghostMovements);
			if (_copy.mySkinned != null) {
				var ghostable = _copy.ghostObject.GetComponent<Ghostable> ();
				ghostObject = Ghostable.InstantiateSavedMesh (
					ghostable.id,
					ghostable.materialsPath,
					ghostable.isSkinned);
				mySkinned = ghostObject.GetComponent<SkinnedMeshRenderer> ();
			} else {
				if (_copy.ghostObject.GetComponent<MeshRenderer> ()) {
					ghostObject = Ghostable.InstantiateCopy (_copy.ghostObject);
					myMesh = ghostObject.GetComponent<MeshRenderer> ();
				}
			}
			timeSteps = new List<float>(_copy.timeSteps);
		}

		public GhostReplayStruct (GhostReplayStruct _copy, GameObject _obj)
		{
			name = _copy.name;
			skipStep = _copy.skipStep;
			ghostMovements = _copy.ghostMovements;
			ghostObject = _obj;
			timeSteps = _copy.timeSteps;
		}

		public void ActivateGhostObject (bool _show)
		{
			started = Time.fixedTime;
			if (ghostObject != null)
				ghostObject.SetActive (_show);
		}

		public bool MoveToNext (float _time)
		{
			_time = _time - started;

			if (_time > timeSteps.Last ()) {
				return false;
			} else {
				var indexOfClosest = timeSteps.BinarySearch (_time);
				if (indexOfClosest < 0) { // the value 10 wasn't found
					indexOfClosest = ~indexOfClosest;
					indexOfClosest -= 1;
				}
				var _closestTime = timeSteps [indexOfClosest];
				var _nextMove = ghostMovements [_closestTime];
				ghostObject.transform.localScale = _nextMove.scale;
				ghostObject.transform.position = _nextMove.position;
				ghostObject.transform.rotation = Quaternion.Euler (_nextMove.rotation);
				if (myMesh != null)
					myMesh.enabled = _nextMove.isEnabled;
				if (mySkinned != null) {
					mySkinned.enabled = _nextMove.isEnabled;
					if (_nextMove.shapes != null) {
						for (int i = 0; i < _nextMove.shapes.Length; i++) {
							mySkinned.SetBlendShapeWeight (i, _nextMove.shapes [i]);
						}
					}
				}
				return true;
			}
		}

		public GameObject GetGhostObject ()
		{
			return ghostObject;	
		}

		public void Interpolate(){
			var stepSize = 0f;
			switch (skipStep) {
			case 1:
				stepSize = 0.5f;
				break;
			case 2: 
				stepSize = 0.33f;
				break;
			case 10:
				stepSize = 0.091f;
				break;
			case 20:
				stepSize = 0.0455f;
				break;
			default: 
				stepSize = 0f;
				break;
			}
			if (stepSize == 0f) {
				return;
			}
			var newTimeSteps = new List<float> (timeSteps);
			for (int i = 0; i < timeSteps.Count - 1; i++) {
				var counter = stepSize;
				var previousMovement = ghostMovements [timeSteps [i]];
				var nextMovement = ghostMovements [timeSteps [i + 1]];
				for(int j= 0;j<skipStep;j++){
					var interpolatedMovement = new GhostTransform (previousMovement, nextMovement, counter);
					var timeStep = timeSteps [i] + counter * (timeSteps [i + 1] - timeSteps [i]);
					ghostMovements [timeStep] = interpolatedMovement;
					counter += stepSize;
					newTimeSteps.Insert (i*(skipStep + 1) + j + 1, timeStep);
				}
			}
			timeSteps = newTimeSteps;
		}
	}
}