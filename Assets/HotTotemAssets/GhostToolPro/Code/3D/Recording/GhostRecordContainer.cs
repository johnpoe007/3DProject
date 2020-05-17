using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GhostToolPro {
	[Serializable]
	public class GhostRecordContainer {
		public string name;
		public List<GhostRecordStruct> recordCollection = new List<GhostRecordStruct>();
		public GhostRecordContainer()
		{
		}
		public GhostRecordContainer(GhostRecordStruct _struct)
		{
			name = _struct.name;
			recordCollection.Add (_struct);
		}
		public GhostRecordContainer(string _name, int _accuracy,float _startTime,Ghostable[] _targets)
		{
			name = _name;
			foreach (Ghostable _ghost in _targets) {
				recordCollection.Add(new GhostRecordStruct(_name,_accuracy,_startTime,_ghost));
			}
		}
		public GhostRecordContainer(GhostRecordContainer _conti)
		{
			// TBD : Change from temporary vars to persisten ones !!!
			name = _conti.name;
			/*for(int m = 0;m< _conti.recordCollection.Count;m++) {
				var _ghost = _conti.recordCollection [m];
				var _movs = _ghost.ghostMovements;
				int addFrames = _ghost.skipStep;
				int j = 0;
				int _j = 0;
				var keys = new List<float>(_movs.Keys);
				var moves = new List<GhostTransform>(_movs.Values);
				while (j < _movs.Count-1)
				{
					for (int i = 1; i <= addFrames; i++) {
						var _time = Mathf.Lerp (keys [_j], keys [_j + 1], ((float)i / (1f+addFrames)));
						var _check = new GhostTransform (moves [_j], moves [_j + 1], ((float)i / (1f + addFrames)));
						_movs [_time] = new GhostTransform(moves[_j],moves[_j+1], ((float)i / (1f+addFrames)));
					}
					_j++;
					j += 1 + addFrames;
				}
				_ghost.ghostMovements = new Dictionary<float,GhostTransform>(_movs);
				_ghost.skipStep = 0;
				_conti.recordCollection [m] = _ghost;
			}*/
			recordCollection = _conti.recordCollection;
		}
		public void DestroyCollection(){
			foreach (var _struct in recordCollection) {
				var obj = _struct.trackedObject.gameObject;
				if (obj.transform.parent.name == "BonesParent" || obj.transform.parent.name == "MeshParent") {
					UnityEngine.Object.Destroy (obj.transform.parent.gameObject);
				}
				UnityEngine.Object.Destroy (obj);
			}
		}
	}
}