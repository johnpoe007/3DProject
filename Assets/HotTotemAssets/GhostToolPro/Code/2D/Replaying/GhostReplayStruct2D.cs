using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GhostToolPro {
	public class GhostReplayStruct2D {
	public string name;
	public GameObject ghostObject; 
	private SpriteRenderer mySprite;
	private GhostObject2D myGhostObject;
	public float started;
	public float paused = 0f;
	private Dictionary<float,GhostTransform2D> ghostMovements = new Dictionary<float, GhostTransform2D>();
	private List<float> timeSteps;

	public GhostReplayStruct2D(Dictionary<float,GhostTransform2D> _ghostMovements,float _started,GameObject _ghost)
	{
		ghostMovements = _ghostMovements;
		ghostObject = _ghost;
		started = _started;
		timeSteps = new List<float> (ghostMovements.Keys);
		myGhostObject = ghostObject.GetComponent<GhostObject2D> ();
		timeSteps.Sort((a, b) => a.CompareTo(b));
	}
	public GhostReplayStruct2D(GhostRecordStruct2D _recording)
	{
		name = _recording.name;
		ghostMovements = _recording.ghostMovements;
		if (_recording.trackedObject != null) {
			ghostObject = Ghostable2D.InstantiateSavedSprites (
					_recording.trackedObject.id,
					_recording.trackedObject.materialsPath);
			mySprite = ghostObject.GetComponent<SpriteRenderer> ();
			myGhostObject = ghostObject.GetComponent<GhostObject2D> ();
		}
		timeSteps = ghostMovements.Keys.ToList ();
	}
	public GhostReplayStruct2D(GhostReplayStruct2D _copy)
	{
		name = _copy.name;
		ghostMovements = _copy.ghostMovements;
		if (_copy.ghostObject.GetComponent<SpriteRenderer> ()) {
				ghostObject = Ghostable2D.InstantiateCopy(_copy.ghostObject);
			mySprite = ghostObject.GetComponent<SpriteRenderer> ();
			myGhostObject = ghostObject.GetComponent<GhostObject2D> ();
		}
		timeSteps = _copy.timeSteps;
	}
	public GhostReplayStruct2D(GhostReplayStruct2D _copy,GameObject _obj)
	{
		name = _copy.name;
		ghostMovements = _copy.ghostMovements;
		ghostObject = _obj;
		timeSteps = _copy.timeSteps;
		myGhostObject = ghostObject.GetComponent<GhostObject2D> ();
	}
	public void ActivateGhostObject(bool _show)
	{
		started = Time.fixedTime;
		if(ghostObject != null)
			ghostObject.SetActive (_show);
	}
	public bool MoveToNext(float _time)
	{
		_time = _time - started;
		if (_time > timeSteps.Last ()) {
			return false;
		} else {
			var indexOfClosest = timeSteps.BinarySearch(_time);
			if (indexOfClosest < 0) // the value 10 wasn't found
			{    
				indexOfClosest = ~indexOfClosest;
				indexOfClosest -= 1;
			}
			var _closestTime = timeSteps[indexOfClosest];
			var _nextMove =  ghostMovements [_closestTime];
			ghostObject.transform.position = _nextMove.position;
			ghostObject.transform.rotation = Quaternion.Euler(_nextMove.rotation);
			ghostObject.transform.localScale = _nextMove.scale;
			mySprite.sprite = myGhostObject.GetSprite(_nextMove.spriteName);
			if (mySprite != null)
				mySprite.enabled = _nextMove.isEnabled;
			return true;
		}
	}
	public GameObject GetGhostObject()
	{
		return ghostObject;	
	}
}
}