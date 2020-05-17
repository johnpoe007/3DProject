using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using GhostToolPro;

public class GhostTool : MonoBehaviour {
	public static GhostTool instance;
	public static Transform ghostParent;
	public event Saved OnSaved;
	public delegate void Saved(string name);
	public event Finished OnFinished;
	public delegate void Finished(string id);

	void Awake()
	{
		if (instance == null) 
		{
			DontDestroyOnLoad (this);
			instance = this;
			ghostParent = transform.GetChild (0);
			GhostDataHandler.Init ();
		}
		else
			Destroy (this);
	}

	/// <summary>
	/// Starts the recording.
	/// </summary>
	/// <param name="_target">The target object to be recorded.</param>
	/// <param name="_name">The temporary name used to stop the recording later on.</param>
	/// <param name="_resolution">Optional parameter to set the accuracy ( and filesize ) of the ghost</param>
	public void startRecording(Ghostable _target,string _name,GhostToolResolution _resolution = GhostToolResolution.VeryHigh)
	{
		var _skipStep = GetSkipStepFromResolution (_resolution);
		if (!_target.isBone) {
			if (_target.isSkinned) {
				Ghostable[] _skinnedTarget = new Ghostable[_target.boneCount+1];
				_skinnedTarget [0] = _target;
				for (int i = 0; i < _target.bones.Length; i++) {
					_skinnedTarget [1 + i] = _target.bones [i];
				}
				GhostRecordHandler.trackedObjects.Add (new GhostRecordContainer (_name,_skipStep,Time.fixedTime,_skinnedTarget));
			} else {
				GhostRecordHandler.trackedObjects.Add (new GhostRecordContainer (new GhostRecordStruct (_name, _skipStep, Time.fixedTime, _target)));
			}
		}
	}
	/// <summary>
	/// Starts the recording.
	/// </summary>
	/// <param name="_target">The target object to be recorded.</param>
	/// <param name="_name">The temporary name used to stop the recording later on.</param>
	/// <param name="recordBlendShapes">If set to <c>true</c> record blendshapes.</param>
	/// <param name="_resolution">Optional parameter to set the accuracy ( and filesize ) of the ghost</param>
	public void startRecording(Ghostable _target,string _name,bool recordBlendShapes,GhostToolResolution _resolution = GhostToolResolution.VeryHigh)
	{
		var _skipStep = GetSkipStepFromResolution (_resolution);
		if (!_target.isBone) {
			if (_target.isSkinned) {
				Ghostable[] _skinnedTarget = new Ghostable[_target.boneCount+1];
				_skinnedTarget [0] = _target;
				_skinnedTarget [0].trackBlendShapes = recordBlendShapes;
				for (int i = 0; i < _target.bones.Length; i++) {
					_skinnedTarget [1 + i] = _target.bones [i];
				}
				GhostRecordHandler.trackedObjects.Add (new GhostRecordContainer (_name,_skipStep,Time.fixedTime,_skinnedTarget));
			} else {
				GhostRecordHandler.trackedObjects.Add (new GhostRecordContainer (new GhostRecordStruct (_name, _skipStep, Time.fixedTime, _target)));
			}
		}
	}
	/// <summary>
	/// Starts the recording.
	/// </summary>
	/// <param name="_target">The targets object to be recorded.</param>
	/// <param name="_name">The temporary name used to stop the recording later on.</param>
	/// <param name="_resolution">Optional parameter to set the accuracy ( and filesize ) of the ghost</param>
	public void startRecording(Ghostable[] _targets,string _name,GhostToolResolution _resolution = GhostToolResolution.VeryHigh)
	{
		var _skipStep = GetSkipStepFromResolution (_resolution);
		var _t = new List<Ghostable> ();
		foreach (Ghostable _target in _targets) {
			if (!_target.isBone) {
				if (_target.isSkinned) {
					Ghostable[] _skinnedTarget = new Ghostable[_target.boneCount + 1];
					_skinnedTarget [0] = _target;
					var rootBone = (_target.GetComponent<SkinnedMeshRenderer>()).rootBone;
					var allBones = rootBone.GetComponentsInChildren<Transform> (true);
					int i = 0;
					foreach (var _b in allBones) {
						var _ghostBoneComponentsInBone = _b.GetComponents<GhostBone> ();
						if (_ghostBoneComponentsInBone != null) {
							foreach (var _ghostBoneComponentInBone in _ghostBoneComponentsInBone) {
								if (_ghostBoneComponentInBone.ObjectID == _target.id) {
									_skinnedTarget [1 + i] = _target.bones.Where (p => p.id == _ghostBoneComponentInBone.id).FirstOrDefault ();
									i++;
								}
							}
						}
					}
					foreach (var item in _skinnedTarget) {
						if (item != null) {
							_t.Add (item);
						}
					}
				} else {
					_t.Add (_target);
				}
			}
		}
		if (_t.Count != 0) {
			var _g = _t.ToArray ();
			GhostRecordHandler.trackedObjects.Add (new GhostRecordContainer (_name,_skipStep,Time.fixedTime,_g));
		}
	}
	/// <summary>
	/// Starts the recording.
	/// </summary>
	/// <param name="_target">The targets object to be recorded.</param>
	/// <param name="_name">The temporary name used to stop the recording later on.</param>
	/// <param name="recordBlendShapes">If set to <c>true</c> record blendshapes.</param>
	/// <param name="_resolution">Optional parameter to set the accuracy ( and filesize ) of the ghost</param>
	public void startRecording(Ghostable[] _targets,string _name,bool recordBlendShapes,GhostToolResolution _resolution = GhostToolResolution.VeryHigh)
	{
		var _skipStep = GetSkipStepFromResolution (_resolution);
		var _t = new List<Ghostable> ();
		foreach (Ghostable _target in _targets) {
			if (!_target.isBone) {
				if (_target.isSkinned) {
					Ghostable[] _skinnedTarget = new Ghostable[_target.boneCount + 1];
					_skinnedTarget [0] = _target;
					_skinnedTarget [0].trackBlendShapes = recordBlendShapes;
					var rootBone = (_target.GetComponent<SkinnedMeshRenderer>()).rootBone;
					var allBones = rootBone.GetComponentsInChildren<Transform> (true);
					int i = 0;
					foreach (var _b in allBones) {
						var _ghostBoneComponentsInBone = _b.GetComponents<GhostBone> ();
						if (_ghostBoneComponentsInBone != null) {
							foreach (var _ghostBoneComponentInBone in _ghostBoneComponentsInBone) {
								if (_ghostBoneComponentInBone.ObjectID == _target.id) {
									_skinnedTarget [1 + i] = _target.bones.Where (p => p.id == _ghostBoneComponentInBone.id).FirstOrDefault ();
									i++;
								}
							}
						}
					}
					foreach (var item in _skinnedTarget) {
						if (item != null) {
							_t.Add (item);
						}
					}
				} else {
					_t.Add (_target);
				}
			}
		}
		if (_t.Count != 0) {
			var _g = _t.ToArray ();
			GhostRecordHandler.trackedObjects.Add (new GhostRecordContainer (_name,_skipStep,Time.fixedTime,_g));
		}
	}
	/// <summary>
	/// Stops the recording and caches it for replay.
	/// </summary>
	/// <param name="_name">The temporary name used to start the recording.</param>
	public void stopRecording(string _name)
	{
		var recordContainer = GhostRecordHandler.trackedObjects.Where(p => p.name == _name).FirstOrDefault();
		if (recordContainer != null) {
			GhostRecordHandler.Remove (recordContainer);
			GhostDataHandler.AddToCache (_name, recordContainer);
		} else {
			Debug.LogWarning (_name + " is not a valid ID - Aborting");
		}
	}
	/// <summary>
	/// Saves the recording from cache.
	/// </summary>
	/// <param name="_name">The temporary name used to start the recording.</param>
	/// <param name="blocking">If set to <c>true</c> the replay is saved synchroniously and may cause hiccups for large replays. Else it is written between frames and emits OnSaved event when completed.</param>
	/// <param name="_saveName">The name used to save and lateron load the replay.</param>
	public void saveRecordingFromCache (string _name,bool blocking,string _saveName)
	{
		if (GhostDataHandler.IsCached (_name)) {
			if (blocking)
				GhostDataHandler.SaveGhost (_saveName, GhostDataHandler.SaveCache[_name]);
			else
				GhostDataHandler.SaveGhostNonBlocking (_saveName, GhostDataHandler.SaveCache[_name]);
		}
		else 
		{
			Debug.LogError ("Error E1 - Cache does not contain " + _name);		
		}
	}
	/// <summary>
	/// Loads the replay.
	/// </summary>
	/// <returns>The temporary id needed to start,pause and resume the replay.</returns>
	/// <param name="_name">The name used to save the replay.</param>
	public string loadReplay(string _name)
	{
		var _replay = GhostDataHandler.LoadGhost (_name);
		return GhostReplayHandler.Add (new GhostReplayContainer(_replay));
	}
	/// <summary>
	/// Deletes a saved replay.
	/// </summary>
	/// <param name="_name">The replay's name.</param>
	public void deleteSavedReplay(string _name)
	{
		GhostDataHandler.DeleteSavedReplay (_name);
	}
	/// <summary>
	/// Check if a saved replay is existing.
	/// </summary>
	/// <returns><c>true</c>, if a saved replay exists under this name, <c>false</c> otherwise.</returns>
	/// <param name="_name">The name of the saved replay.</param>
	public bool isReplayExisting(string _name)
	{
		return GhostDataHandler.CheckIfExists (_name);
	}
	/// <summary>
	/// Starts the replaying.
	/// </summary>
	/// <param name="_id">Identifier returned by loadReplay.</param>
	public void startReplaying(string _id)
	{
		var _replay = GhostReplayHandler.recordedObjects.Where (p => p.Key == _id).FirstOrDefault ();
		if (!_replay.Equals(default(KeyValuePair<string,GhostReplayContainer>))) {
			foreach (GhostReplayStruct _obj in _replay.Value.replayCollection) {
				_obj.ActivateGhostObject (true);
			}
			_replay.Value.replay = true;
		} else {
			Debug.LogWarning (_id + " is not a valid ID - Aborting");
		}
	}
	/// <summary>
	/// Checks if the replay associated to the ID is replaying.
	/// </summary>
	/// <returns><c>true</c>, if id is replaying, <c>false</c> otherwise.</returns>
	/// <param name="_id">Identifier returned by loadReplay.</param>
	public bool isReplaying(string _id)
	{
		var _replay = GhostReplayHandler.recordedObjects.Where (p => p.Key == _id).FirstOrDefault ();
		if (!_replay.Equals(default(KeyValuePair<string,GhostReplayContainer>))) {
			return true;
		} else {
			return false;
		}
	}
	/// <summary>
	/// Pauses the replaying.
	/// </summary>
	/// <param name="_id">Identifier returned by loadReplay.</param>
	public void pauseReplaying(string _id)
	{
		var _replay = GhostReplayHandler.recordedObjects.Where(p => p.Key == _id).FirstOrDefault();
		if (!_replay.Equals(default(KeyValuePair<string,GhostReplayContainer>))) {
			foreach (var _rs in _replay.Value.replayCollection)
			_rs.paused = Time.time;
			_replay.Value.replay = false;
		} else {
			Debug.LogWarning (_id + " is not a valid ID - Aborting");
		}
	}
	/// <summary>
	/// Stops the replaying.
	/// </summary>
	/// <param name="_id">Identifier returned by loadReplay.</param>
	public void stopReplaying(string _id)
	{
		var _replay = GhostReplayHandler.recordedObjects.Where (p => p.Key == _id).FirstOrDefault ();
		if (!_replay.Equals(default(KeyValuePair<string,GhostReplayContainer>))) {
			GhostReplayHandler.Finished (_id);
		} else {
			Debug.LogWarning (_id + " is not a valid ID - Aborting");
		}
	}
	/// <summary>
	/// Resumes the replaying.
	/// </summary>
	/// <param name="_id">Identifier returned by loadReplay.</param>
	public void resumeReplaying(string _id)
	{
		var _replay = GhostReplayHandler.recordedObjects.Where(p => p.Key == _id).FirstOrDefault();
		if (!_replay.Equals(default(KeyValuePair<string,GhostReplayContainer>))) {
			if (!_replay.Value.replay) {
				foreach (var _rs in _replay.Value.replayCollection)
					_rs.started = _rs.started + Time.time - _rs.paused;
				_replay.Value.replay = true;
			}
			else {
				Debug.LogWarning (_id + " already playing.");
			}
		} else {
			Debug.LogWarning (_id + " is not a valid ID - Aborting");
		}
	}
	/// <summary>
	/// Shares the replay.
	/// </summary>
	/// <returns>The raw data that can be transferred over network e.g.</returns>
	/// <param name="_name">The name used to save the replay.</param>
	public byte[] shareReplay(string _name)
	{
		return GhostDataHandler.ShareReplay (_name);
	}
	/// <summary>
	/// Receives the replay.
	/// </summary>
	/// <param name="_name">The name at which the replay will be saved. If already existing it will be overwritten.</param>
	/// <param name="_data">The raw data emitted by shareReplay.</param>
	public void receiveReplay(string _name, byte[] _data)
	{
		GhostDataHandler.ReceiveReplay (_name, _data);
	}

	public static void isSaved(string _name)
	{
		if (instance.OnSaved != null)
		{
			instance.OnSaved(_name);
		}
	}
	public static void isFinished(string _id)
	{
		if (instance.OnFinished != null)
		{
			instance.OnFinished(_id);
		}
	}
	private int GetSkipStepFromResolution(GhostToolResolution _resolution){
		var _skipStep = 100;
		switch (_resolution) {
		case GhostToolResolution.VeryHigh:
			_skipStep = 100;
			break;
		case GhostToolResolution.High:
			_skipStep = 50;
			break;
		case GhostToolResolution.Medium:
			_skipStep = 25;
			break;
		case GhostToolResolution.Low:
			_skipStep = 5;
			break;
		case GhostToolResolution.VeryLow:
			_skipStep = 1;
			break;
		default:
			_skipStep = 100;
			break;
		}
		return _skipStep;
	}
}

public enum GhostToolResolution{
	VeryLow,
	Low,
	Medium,
	High,
	VeryHigh
}
