using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace GhostToolPro {
	public class GhostDataHandler2D : MonoBehaviour {
	private static Dictionary<string,GhostReplayContainer2D> CachedReplays = new Dictionary<string, GhostReplayContainer2D>();
	public static Dictionary<string,GhostRecordContainer2D> SaveCache = new Dictionary<string, GhostRecordContainer2D>();

	public static void Init()
	{
		System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/HotTotem/");
		System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/HotTotem/GhostToolPro");
		System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays");
		Resources.LoadAll ("GhostToolPro/");
	}
	public static bool IsCached(string _name)
	{
		return SaveCache.ContainsKey (_name);
	}
	public static GhostReplayContainer2D LoadGhost(string _name)
	{
		if (!CachedReplays.ContainsKey (_name)) {
			LoadToCache (_name);
		}
		return CachedReplays [_name];
	}
	public static void AddToCache(string name,GhostRecordContainer2D replay)
	{
		SaveCache [name] = new GhostRecordContainer2D(replay);
		CachedReplays [name] = new GhostReplayContainer2D (SaveCache [name]);
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
	public static void DeleteSavedReplay(string _name)
	{
		string filename = _name + ".ghost";
		filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
		if (File.Exists (filename)) {
			File.Delete (filename);
		} else {
			Debug.LogWarning ("No such replay: " + _name + " - Deleting aborted");
		}
	}
	public static bool ReceiveReplay(string _name, byte[] _data)
	{
		string filename = _name + ".ghost";
		filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
		using (FileStream file = File.OpenWrite(filename))
		{
			file.Write (_data, 0, _data.Length);
		}
		return true;
	}
	public static bool CheckIfExists(string _name)
	{
		string filename = _name + ".ghost";
		filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
		if (File.Exists (filename)) {
			return true;
		} else {
			return false;		
		}
	}
	private static void LoadToCache(string _name)
	{
		string filename = _name + ".ghost";
		filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
		FileStream _file;
		if (File.Exists (filename)) {
			_file = File.Open (filename, FileMode.Open);
			BinaryReader ghostReader = new BinaryReader (_file);
			try {
				// Deserialize the Employee object from the memory stream.
				GhostRecordContainer2D tmpContainer = new GhostRecordContainer2D ();
				tmpContainer.name = ghostReader.ReadString ();
				int structCount = ghostReader.ReadInt32 ();
				tmpContainer.recordCollection = new List<GhostRecordStruct2D> ();
				for (int i = 0; i < structCount; i++) {
					var _n = ghostReader.ReadString ();
					var _sS = ghostReader.ReadInt32 ();
					var _iD = ghostReader.ReadString ();
					var _mL = ghostReader.ReadInt32 ();
					string[] _mP = new string[_mL];
					for (int m = 0; m < _mL; m++) {
						_mP [m] = ghostReader.ReadString ();
					}
					var _movC = ghostReader.ReadInt32 ();
					Dictionary<float,GhostTransform2D> _dict = new Dictionary<float, GhostTransform2D> ();
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
						var _spriteName = ghostReader.ReadString();
						_dict [_t] = new GhostTransform2D (_posi, _scal, _rota, _isE,_spriteName);
					}
					tmpContainer.recordCollection.Add (new GhostRecordStruct2D (_n, _sS, _dict, _iD, _mP));
				}
				CachedReplays [_name] = (new GhostReplayContainer2D (tmpContainer));
			} catch (SerializationException e) {
				Debug.LogError ("Deserialization failed - eC12 : " + e.Message);
				throw;
			}
			_file.Close ();
		} else {
			Debug.LogWarning ("No such replay saved " + _name);
		}
	}
	public static void SaveGhostNonBlocking (string _name, GhostRecordContainer2D _struct)
	{
		var _saverObj = new GameObject ();
		var _saver = _saverObj.AddComponent<GhostSaveHelper2D> ();
		_saver.StartCoroutine(_saver.SaveGhostNonblocking(_name,_struct));
	}
	public static void SaveGhost(string _name, GhostRecordContainer2D _struct)
	{
		string filename = _name + ".ghost";
		FileStream _file;
		filename = Application.persistentDataPath + "/HotTotem/GhostToolPro/Replays/" + filename;
		_file = File.Create(filename);
		BinaryWriter ghostWriter = new BinaryWriter(_file);
		try
		{
			// Serialize an Employee object into the memory stream.
			ghostWriter.Write(_struct.name);
			ghostWriter.Write(_struct.recordCollection.Count);
			foreach(GhostRecordStruct2D _rS in _struct.recordCollection)
			{
				ghostWriter.Write(_rS.name);
				ghostWriter.Write(_rS.skipStep);
				ghostWriter.Write(_rS.trackedObject.id);
				ghostWriter.Write(_rS.trackedObject.materialsPath.Length);
				foreach(string _path in _rS.trackedObject.materialsPath)
					ghostWriter.Write(_path);
				ghostWriter.Write(_rS.ghostMovements.Count);
				foreach(KeyValuePair<float,GhostTransform2D> pair in _rS.ghostMovements){
					ghostWriter.Write(pair.Key);
					ghostWriter.Write(pair.Value.isEnabled);
					ghostWriter.Write(pair.Value.position.x);
					ghostWriter.Write(pair.Value.position.y);
					ghostWriter.Write(pair.Value.position.z);
					ghostWriter.Write(pair.Value.scale.x);
					ghostWriter.Write(pair.Value.scale.y);
					ghostWriter.Write(pair.Value.scale.z);
					ghostWriter.Write(pair.Value.rotation.x);
					ghostWriter.Write(pair.Value.rotation.y);
					ghostWriter.Write(pair.Value.rotation.z);
					ghostWriter.Write(pair.Value.spriteName);
				}
			}
		}
		catch (SerializationException e)
		{
			Debug.LogError("Serialization failed - eC01 : " + e.Message);
			throw;
		}
		ghostWriter.Close ();
		_file.Close();
	}
}
}