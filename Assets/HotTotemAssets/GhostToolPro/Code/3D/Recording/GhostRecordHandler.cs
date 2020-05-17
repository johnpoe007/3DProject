using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GhostToolPro
{
	public class GhostRecordHandler : MonoBehaviour
	{

		public static List<GhostRecordContainer> trackedObjects = new List<GhostRecordContainer> ();

		void RecordMovement (int _pos, float _time)
		{
			foreach (GhostRecordStruct _struct in trackedObjects [_pos].recordCollection)
				_struct.AddMovement (_time);
		}

		void FixedUpdate ()
		{
			for (int i = 0; i < trackedObjects.Count; i++) {
				var _obj = (trackedObjects [i]).recordCollection [0];
				if (_obj.skipped == _obj.skipStep) {
					RecordMovement (i, Time.fixedTime);
				} else {
					trackedObjects [i].recordCollection [0].skipped++;
				}
			}
		}
		public static void Remove(GhostRecordContainer _removedContainer){
			if (_removedContainer.recordCollection [0].skipped != 0) {
				foreach (GhostRecordStruct _struct in _removedContainer.recordCollection)
					_struct.AddMovement (Time.fixedTime);
			}
			trackedObjects.Remove (_removedContainer);
		}
	}
}