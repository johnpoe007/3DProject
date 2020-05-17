using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GhostToolPro;

public class GhostTool2D : MonoBehaviour {
	public static GhostTool2D instance;
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
			GhostDataHandler2D.Init ();
		}
		else
			Destroy (this);
	}
	/// <summary>
	/// Starts the recording.
	/// </summary>
	/// <param name="_target">The target object to be recorded.</param>
	/// <param name="_name">The temporary name used to stop the recording later on.</param>
	public void startRecording(Ghostable2D _target,string _name)
	{
		GhostRecordHandler2D.trackedObjects.Add (new GhostRecordContainer2D (new GhostRecordStruct2D (_name, 100, Time.fixedTime, _target)));
	}
	/// <summary>
	/// Starts the recording.
	/// </summary>
	/// <param name="_target">The targets object to be recorded.</param>
	/// <param name="_name">The temporary name used to stop the recording later on.</param>
	public void startRecording(Ghostable2D[] _targets,string _name)
	{
		var _t = new List<Ghostable2D> ();
		foreach (Ghostable2D _target in _targets) {
			_t.Add (_target);
		}
		if (_t.Count != 0) {
			var _g = _t.ToArray ();
			GhostRecordHandler2D.trackedObjects.Add (new GhostRecordContainer2D (_name,100,Time.fixedTime,_g));
		}
	}
	/// <summary>
	/// Stops the recording and caches it for replay.
	/// </summary>
	/// <param name="_name">The temporary name used to start the recording.</param>
	public void stopRecording(string _name)
	{
		var recordContainer = GhostRecordHandler2D.trackedObjects.Where(p => p.name == _name).FirstOrDefault();
		if (recordContainer != null) {
			GhostRecordHandler2D.trackedObjects.Remove (recordContainer);
			GhostDataHandler2D.AddToCache (_name, recordContainer);
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
		if (GhostDataHandler2D.IsCached (_name)) {
			if (blocking)
				GhostDataHandler2D.SaveGhost (_saveName, GhostDataHandler2D.SaveCache[_name]);
			else
				GhostDataHandler2D.SaveGhostNonBlocking (_saveName, GhostDataHandler2D.SaveCache[_name]);
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
		var _replay = GhostDataHandler2D.LoadGhost (_name);
		return GhostReplayHandler2D.Add (new GhostReplayContainer2D(_replay));
	}
	/// <summary>
	/// Deletes a saved replay.
	/// </summary>
	/// <param name="_name">The replay's name.</param>
	public void deleteSavedReplay(string _name)
	{
		GhostDataHandler2D.DeleteSavedReplay (_name);
	}
	/// <summary>
	/// Check if a saved replay is existing.
	/// </summary>
	/// <returns><c>true</c>, if a saved replay exists under this name, <c>false</c> otherwise.</returns>
	/// <param name="_name">The name of the saved replay.</param>
	public bool isReplayExisting(string _name)
	{
		return GhostDataHandler2D.CheckIfExists (_name);
	}
	/// <summary>
	/// Starts the replaying.
	/// </summary>
	/// <param name="_id">Identifier returned by loadReplay.</param>
	public void startReplaying(string _id)
	{
		var _replay = GhostReplayHandler2D.recordedObjects.Where(p => p.uniqueID == _id).FirstOrDefault();
		if (_replay != null) {
			foreach (GhostReplayStruct2D _obj in _replay.replayCollection) {
				_obj.ActivateGhostObject (true);
			}
			_replay.replay = true;
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
		var _replay = GhostReplayHandler2D.recordedObjects.Where (p => p.uniqueID == _id).FirstOrDefault ();
		if (_replay != null) {
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
		var _replay = GhostReplayHandler2D.recordedObjects.Where(p => p.uniqueID == _id).FirstOrDefault();
		if (_replay != null) {
			foreach (var _rs in _replay.replayCollection)
				_rs.paused = Time.time;
			_replay.replay = false;
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
		var _replay = GhostReplayHandler2D.recordedObjects.Where (p => p.uniqueID == _id).FirstOrDefault ();
		if (_replay != null) {
			var _replayID = GhostReplayHandler2D.recordedObjects.IndexOf(_replay);
			GhostReplayHandler2D.Finished (_replayID);
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
		var _replay = GhostReplayHandler2D.recordedObjects.Where(p => p.uniqueID == _id).FirstOrDefault();
		if (_replay != null) {
			foreach (var _rs in _replay.replayCollection)
				_rs.started = _rs.started + Time.time - _rs.paused;
			_replay.replay = true;
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
		return GhostDataHandler2D.ShareReplay (_name);
	}
	/// <summary>
	/// Receives the replay.
	/// </summary>
	/// <param name="_name">The name at which the replay will be saved. If already existing it will be overwritten.</param>
	/// <param name="_data">The raw data emitted by shareReplay.</param>
	public void receiveReplay(string _name, byte[] _data)
	{
		GhostDataHandler2D.ReceiveReplay (_name, _data);
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
}
