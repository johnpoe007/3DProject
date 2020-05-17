using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace GhostToolPro
{
	public class GhostDataHandler : MonoBehaviour
	{
		private static Dictionary<string,GhostReplayContainer> CachedReplays = new Dictionary<string, GhostReplayContainer> ();
		public static Dictionary<string,GhostRecordContainer> SaveCache = new Dictionary<string, GhostRecordContainer> ();

		public static void Init ()
		{
			System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/HotTotem/");
			System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/HotTotem/GhostToolPro");
			System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays");
			Resources.LoadAll ("GhostToolPro/");
		}

		public static bool IsCached (string _name)
		{
			return CachedReplays.ContainsKey (_name);
		}

		public static GhostReplayContainer LoadGhost (string _name)
		{
			if (!CachedReplays.ContainsKey (_name)) {
				LoadToCache (_name);
			}
			return CachedReplays [_name];
		}

		public static void AddToCache (string name, GhostRecordContainer replay)
		{
			SaveCache [name] = new GhostRecordContainer (replay);
			var newReplayContainer = new GhostReplayContainer(SaveCache[name]);
			newReplayContainer.Interpolate ();
			CachedReplays [name] = newReplayContainer;
		}

		public static byte[] ShareReplay (string _name)
		{
			string filename = _name + ".ghost";
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			FileStream _file;
			if (File.Exists (filename)) {
				_file = File.Open (filename, FileMode.Open);
				byte[] fileBytes = new byte[_file.Length];

				_file.Read (fileBytes, 0, fileBytes.Length);
				_file.Close ();
				return fileBytes;
			} else {
				Debug.LogError ("No such file");
				return null;
			}
		}

		public static void DeleteSavedReplay (string _name)
		{
			string filename = _name + ".ghost";
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			if (File.Exists (filename)) {
				File.Delete (filename);
			} else {
				Debug.LogWarning ("No such replay: " + _name + " - Deleting aborted");
			}
		}

		public static bool ReceiveReplay (string _name, byte[] _data)
		{
			string filename = _name + ".ghost";
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			using (FileStream file = File.OpenWrite (filename)) {
				file.Write (_data, 0, _data.Length);
			}
			return true;
		}

		public static bool CheckIfExists (string _name)
		{
			string filename = _name + ".ghost";
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			if (File.Exists (filename)) {
				return true;
			} else {
				return false;		
			}
		}

		private static void LoadToCache (string _name)
		{
			string filename = _name + ".ghost";
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			FileStream _file;
			if (File.Exists (filename)) {
				_file = File.Open (filename, FileMode.Open);
				BinaryReader ghostReader = new BinaryReader (_file);
				try {
					// Deserialize the Employee object from the memory stream.
					GhostRecordContainer tmpContainer = new GhostRecordContainer ();
					tmpContainer.name = ghostReader.ReadString ();
					int structCount = ghostReader.ReadInt32 ();
					tmpContainer.recordCollection = new List<GhostRecordStruct> ();
					Ghostable skinned = null;
					for (int i = 0; i < structCount; i++) {
						var _n = ghostReader.ReadString ();
						var _sS = ghostReader.ReadInt32 ();
						var _iD = ghostReader.ReadString ();
						var _sK = ghostReader.ReadBoolean ();
						var _iB = ghostReader.ReadBoolean ();
						var _mL = ghostReader.ReadInt32 ();
						string[] _mP = new string[_mL];
						for (int m = 0; m < _mL; m++) {
							_mP [m] = ghostReader.ReadString ();
						}
						var _movC = ghostReader.ReadInt32 ();
						Dictionary<float,GhostTransform> _dict = new Dictionary<float, GhostTransform> ();
						for (int m = 0; m < _movC; m++) {
							var _t = ghostReader.ReadSingle ();
							var _isE = ghostReader.ReadBoolean ();
							var _x = ghostReader.ReadSingle ();
							var _y = ghostReader.ReadSingle ();
							var _z = ghostReader.ReadSingle ();
							var _posi = new Vector3 (_x, _y, _z);
							_x = ghostReader.ReadSingle ();
							_y = ghostReader.ReadSingle ();
							_z = ghostReader.ReadSingle ();
							var _scal = new Vector3 (_x, _y, _z);
							_x = ghostReader.ReadSingle ();
							_y = ghostReader.ReadSingle ();
							_z = ghostReader.ReadSingle ();
							var _rota = new Vector3 (_x, _y, _z);
							var _shapeC = ghostReader.ReadInt32 ();
							if(_shapeC == 0){
								_dict [_t] = new GhostTransform (_posi, _scal, _rota, _isE);
							} else {
								var _shapes = new float[_shapeC];
								for (int s = 0; s < _shapeC; s++) {
									_shapes[s] = ghostReader.ReadSingle ();
								}
								_dict [_t] = new GhostTransform (_posi, _scal, _rota, _isE,_shapes);
							}
						}
						if ((_sK && !_iB)) {
							tmpContainer.recordCollection.Add (new GhostRecordStruct (_n, _sS, _dict, _iD, _mP, _sK, _iB));
							skinned = tmpContainer.recordCollection [tmpContainer.recordCollection.Count - 1].trackedObject;
						} else {
							if (_iB) {
								tmpContainer.recordCollection.Add (new GhostRecordStruct (_n, _sS, _dict, _iD, _mP, _sK, _iB, skinned));
							} else {
								tmpContainer.recordCollection.Add (new GhostRecordStruct (_n, _sS, _dict, _iD, _mP, _sK, _iB));
							}
						}
					}
					AddToCache(_name,tmpContainer);
					tmpContainer.DestroyCollection();
				} catch (SerializationException e) {
					Debug.LogError ("Deserialization failed - eC12 : " + e.Message);
					throw;
				}
				_file.Close ();
			} else {
				Debug.LogWarning ("No such replay saved " + _name);
			}
		}

		public static void SaveGhostNonBlocking (string _name, GhostRecordContainer _struct)
		{
			var _saverObj = new GameObject ();
			_saverObj.hideFlags = HideFlags.HideInHierarchy;
			var _saver = _saverObj.AddComponent<GhostSaveHelper> ();
			_saver.StartCoroutine (_saver.SaveGhostNonblocking (_name, _struct));
		}

		public static void SaveGhost (string _name, GhostRecordContainer _struct)
		{
			string filename = _name + ".ghost";
			FileStream _file;
			filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
			_file = File.Create (filename);
			BinaryWriter ghostWriter = new BinaryWriter (_file);
			try {
				// Serialize an Employee object into the memory stream.
				ghostWriter.Write (_struct.name);
				ghostWriter.Write (_struct.recordCollection.Count);
				foreach (GhostRecordStruct _rS in _struct.recordCollection) {
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
						if(pair.Value.shapes == null){
							ghostWriter.Write (0);
						} else {
							ghostWriter.Write (pair.Value.shapes.Length);
							foreach(float weight in pair.Value.shapes){
								ghostWriter.Write (weight);
							}
						}
					}
				}
			} catch (SerializationException e) {
				Debug.LogError ("Serialization failed - eC01 : " + e.Message);
				throw;
			}
			ghostWriter.Close ();
			_file.Close ();
		}
	}
}