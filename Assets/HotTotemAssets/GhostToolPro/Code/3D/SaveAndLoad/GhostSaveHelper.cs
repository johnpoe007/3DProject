using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GhostToolPro
{
	public class GhostSaveHelper: MonoBehaviour
	{

		public IEnumerator SaveGhostNonblocking (string _name, GhostRecordContainer _struct)
		{
			string filename = _name + ".ghost";
			FileStream _file;
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			_file = File.Create (filename);
			BinaryWriter ghostWriter = new BinaryWriter (_file);
			// Serialize an Employee object into the memory stream.
			ghostWriter.Write (_struct.name);
			ghostWriter.Write (_struct.recordCollection.Count);
			int j = 0;
			int frameSkip = 1;
			int _i = 0;
			while ((_i + j * frameSkip) < _struct.recordCollection.Count) {
				for (int i = 0; i < frameSkip; i++) {
					if ((i + j * frameSkip) < _struct.recordCollection.Count) {
						GhostRecordStruct _rS = _struct.recordCollection [i + j * frameSkip];

						ghostWriter.Write (_rS.name);
						ghostWriter.Write (_rS.skipStep);
						ghostWriter.Write (_rS.trackedObject.id);
						ghostWriter.Write (_rS.trackedObject.isSkinned);
						ghostWriter.Write (_rS.trackedObject.isBone);
						ghostWriter.Write (_rS.trackedObject.materialsPath.Length);
						foreach (string _path in _rS.trackedObject.materialsPath)
							ghostWriter.Write (_path);
						ghostWriter.Write (_rS.ghostMovements.Count);
						foreach (KeyValuePair<float,GhostTransform> pair in _rS.ghostMovements) {
							ghostWriter.Write (pair.Key);
							ghostWriter.Write (pair.Value.isEnabled);
							ghostWriter.Write (pair.Value.position.x);
							ghostWriter.Write (pair.Value.position.y);
							ghostWriter.Write (pair.Value.position.z);
							ghostWriter.Write (pair.Value.scale.x);
							ghostWriter.Write (pair.Value.scale.y);
							ghostWriter.Write (pair.Value.scale.z);
							ghostWriter.Write (pair.Value.rotation.x);
							ghostWriter.Write (pair.Value.rotation.y);
							ghostWriter.Write (pair.Value.rotation.z);
							if (pair.Value.shapes == null) {
								ghostWriter.Write (0);
							} else {
								ghostWriter.Write (pair.Value.shapes.Length);
								foreach (float weight in pair.Value.shapes) {
									ghostWriter.Write (weight);
								}
							}
						}
					}
					_i = i;
				}
				j++;
				yield return null;
			}
			ghostWriter.Close ();
			_file.Close ();
			GhostTool.isSaved (_name);
			Destroy (this);
		}
	}
}