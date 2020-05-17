using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GhostToolPro {
	public class GhostSaveHelper2D: MonoBehaviour {

	public IEnumerator SaveGhostNonblocking(string _name, GhostRecordContainer2D _struct)
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
					GhostRecordStruct2D _rS = _struct.recordCollection [i + j * frameSkip];

					ghostWriter.Write (_rS.name);
					ghostWriter.Write (_rS.skipStep);
					ghostWriter.Write (_rS.trackedObject.id);
					ghostWriter.Write (_rS.trackedObject.materialsPath.Length);
					foreach (string _path in _rS.trackedObject.materialsPath)
						ghostWriter.Write (_path);
					ghostWriter.Write (_rS.ghostMovements.Count);
					foreach (KeyValuePair<float,GhostTransform2D> pair in _rS.ghostMovements) {
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
						ghostWriter.Write (pair.Value.spriteName);
					}
				}
				_i = i;
			}
			j++;
			yield return null;
		}
		ghostWriter.Close ();
		_file.Close ();
		GhostTool2D.isSaved (_name);
	}
}
}