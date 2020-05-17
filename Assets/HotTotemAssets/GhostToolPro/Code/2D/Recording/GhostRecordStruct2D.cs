using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GhostToolPro {
	public class GhostRecordStruct2D {
	public string name;
	public int skipStep,skipped;
	public float startedTime;
	public Ghostable2D trackedObject; 
	public Dictionary<float,GhostTransform2D> ghostMovements = new Dictionary<float, GhostTransform2D>();

	public GhostRecordStruct2D(string _name,int _accuracy,float _startTime,Ghostable2D _obj)
	{
		name = _name;
		switch (_accuracy) {
		case 100:
			skipStep = 0;
			break;
		case 50: 
			skipStep = 1;
			break;
		case 25:
			skipStep = 2;
			break;
		case 5: 
			skipStep = 10;
			break;
		default: 
			skipStep = 0;
			break;
		}
		startedTime = _startTime;
		trackedObject = _obj;
		AddMovement (_startTime);
	}
	public GhostRecordStruct2D(string _name,int _skipStep,Dictionary<float,GhostTransform2D> _movements,string _id,string[] _materials)
	{
		name = _name;
		skipped = 0;
		skipStep = _skipStep;
		trackedObject = Ghostable2D.InstantiateSavedSprites (_id, _materials).GetComponent<Ghostable2D>();
		ghostMovements = _movements;
	}

	public void AddMovement(float _time)
	{
		_time = _time - startedTime;
		if (trackedObject != null) {
			if (trackedObject.spriteRender != null) {
				ghostMovements [_time] = new GhostTransform2D (
					trackedObject.transform.position,
					trackedObject.transform.localScale,
					trackedObject.transform.rotation.eulerAngles,
					trackedObject.spriteRender.enabled,
					trackedObject.spriteRender.sprite.name
				);
			} 
		} 
		this.skipped = 0;
	}
}
}