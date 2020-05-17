using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GhostToolPro {
	public class GhostRecordHandler2D : MonoBehaviour {

	public static List<GhostRecordContainer2D> trackedObjects = new List<GhostRecordContainer2D>();

	void RecordMovement(int _pos,float _time)
	{
		foreach(GhostRecordStruct2D _struct in trackedObjects [_pos].recordCollection)
			_struct.AddMovement (_time);
	}	
	void FixedUpdate () {
		for (int i = 0; i < trackedObjects.Count; i++) 
		{
			var _obj = (trackedObjects [i]).recordCollection[0];
			if (_obj.skipped == _obj.skipStep) {
				RecordMovement (i, Time.fixedTime);
			} 
			else 
			{
				trackedObjects [i].recordCollection[0].skipped++;
			}
		}
	}
}
}