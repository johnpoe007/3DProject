using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GhostToolPro
{
	public class GhostRecordStruct
	{
		public string name;
		public int skipStep, skipped;
		public float startedTime;
		public Ghostable trackedObject;
		public Dictionary<float,GhostTransform> ghostMovements = new Dictionary<float, GhostTransform> ();

		public GhostRecordStruct (string _name, int _accuracy, float _startTime, Ghostable _obj)
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
			case 1: 
				skipStep = 20;
				break;
			default: 
				skipStep = 0;
				break;
			}
			skipped = skipStep;
			startedTime = _startTime;
			trackedObject = _obj;
			AddMovement (_startTime);
		}

		public GhostRecordStruct (string _name, int _skipStep, Dictionary<float,GhostTransform> _movements, string _id, string[] _materials, bool _isSkinned, bool _isBone)
		{
			name = _name;
			skipStep = _skipStep;
			skipped = skipStep;
			trackedObject = Ghostable.InstantiateSavedMesh (_id, _materials, _isSkinned).GetComponent<Ghostable> ();
			ghostMovements = _movements;
		}

		public GhostRecordStruct (string _name, int _skipStep, Dictionary<float,GhostTransform> _movements, string _id, string[] _materials, bool _isSkinned, bool _isBone, Ghostable _skinnedMesh)
		{
			name = _name;
			skipStep = _skipStep;
			skipped = skipStep;
			trackedObject = _skinnedMesh.bones.Where (p => p.id == _id).FirstOrDefault ();
			ghostMovements = _movements;
		}

		public void AddMovement (float _time)
		{
			_time = _time - startedTime;
			if (trackedObject != null) {
				if (trackedObject.meshRender != null) {
					ghostMovements [_time] = new GhostTransform (
						trackedObject.transform.position,
						trackedObject.transform.lossyScale,
						trackedObject.transform.rotation.eulerAngles,
						trackedObject.meshRender.enabled
					);
				} else {
					if (trackedObject.skinnedMeshRender != null) {
						if (trackedObject.trackBlendShapes) {
							var bsCount = trackedObject.skinnedMeshRender.sharedMesh.blendShapeCount;
							var bsWeights = new float[bsCount];
							for (int i = 0; i < bsCount; i++) {
								bsWeights [i] = trackedObject.skinnedMeshRender.GetBlendShapeWeight (i);
							}
							ghostMovements [_time] = new GhostTransform (
								trackedObject.transform.position,
								trackedObject.transform.localScale,
								trackedObject.transform.rotation.eulerAngles,
								trackedObject.skinnedMeshRender.enabled,
								bsWeights
							);
						} else {
							ghostMovements [_time] = new GhostTransform (
								trackedObject.transform.position,
								trackedObject.transform.localScale,
								trackedObject.transform.rotation.eulerAngles,
								trackedObject.skinnedMeshRender.enabled
							);
						}
					} else {
						ghostMovements [_time] = new GhostTransform (
							trackedObject.transform.position,
							trackedObject.transform.localScale,
							trackedObject.transform.rotation.eulerAngles,
							trackedObject.gameObject.activeSelf
						);
					}
				}
			} 
			this.skipped = 0;
		}
	}
}