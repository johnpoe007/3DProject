using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using GhostToolPro;

public class Ghostable : MonoBehaviour {
	[SerializeField]
	[HideInInspector]
	public string id;
	[SerializeField]
	[HideInInspector]
	public string ObjectID;
	[SerializeField]
	[HideInInspector]
	public string[] materialsPath;
	[SerializeField]
	[HideInInspector]
	public Material[] alternativeMaterials;
	[SerializeField]
	[HideInInspector]
	public Material[] placeHolderMaterials;
	[SerializeField]
	[HideInInspector]
	public bool isSkinned;
	[SerializeField]
	[HideInInspector]
	public bool trackBlendShapes;
	[SerializeField]
	[HideInInspector]
	public bool isBone;
	[SerializeField]
	[HideInInspector]
	public int boneCount;
	[SerializeField]
	[HideInInspector]
	public MeshRenderer meshRender;
	[SerializeField]
	[HideInInspector]
	public SkinnedMeshRenderer skinnedMeshRender;
	[SerializeField]
	[HideInInspector]
	public Ghostable[] bones;

	public Ghostable()
	{
	}
	public Ghostable(string _id, string[] _matPath, bool _skinned)
	{
		id = _id;
		materialsPath = _matPath;
		isSkinned = _skinned;
	}

	#if UNITY_EDITOR
	public void Ghostify(MeshRenderer _renderer)
	{
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.3f) );
		id = (Guid.NewGuid()).ToString();
		isSkinned = false;
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.4f) );
		if (!Directory.Exists(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro"))
		{
			Directory.CreateDirectory(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro");
		} 
		var _filter = (Mesh)Instantiate (_renderer.GetComponent<MeshFilter> ().sharedMesh);
		AssetDatabase.CreateAsset (_filter, "Assets/HotTotemAssets/Resources/GhostToolPro/" + id + ".asset");
		int i = 1;
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.6f) );
		materialsPath = new string[_renderer.sharedMaterials.Length];
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.8f) );
		foreach (Material mat in _renderer.sharedMaterials) {
			var _mat = new Material (mat);
			string path = "Assets/HotTotemAssets/Resources/GhostToolPro/" + id + "-mat-" + i.ToString () + ".asset";
			AssetDatabase.CreateAsset (_mat, path);
			materialsPath [i - 1] = "GhostToolPro/" + id + "-mat-" + i.ToString ();
			i++;
		}
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(1f) );
		meshRender = GetComponent<MeshRenderer> ();
		EditorUtility.ClearProgressBar( );
	}
	public void Ghostify(SkinnedMeshRenderer _renderer)
	{
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.3f) );
		id = (Guid.NewGuid()).ToString();
		isSkinned = true;
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.4f) );
		if (!Directory.Exists(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro"))
		{
			Directory.CreateDirectory(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro");
		} 
		int i = 1;
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.6f) );
		materialsPath = new string[_renderer.sharedMaterials.Length];
		var _allBones = _renderer.bones;
		var _rootBone = _renderer.rootBone;
		bones = new Ghostable[_renderer.bones.Length];
		for (int k = 0; k < _renderer.bones.Length; k++) {
			var _b = _allBones[k];
			var _boneComp = _b.gameObject.AddComponent<GhostBone> ();
			var _ghostableB = _b.gameObject.AddComponent<Ghostable> ();
			bones [k] = _ghostableB;
			_ghostableB.isBone = true;
			_ghostableB.id = (Guid.NewGuid()).ToString();
			_boneComp.id = _ghostableB.id;
			_boneComp.size = _allBones.Length;
			_boneComp.position = k;
			_boneComp.ObjectID = id;
			_ghostableB.ObjectID = id;
		}
		_renderer.GetComponent<Ghostable> ().boneCount = _allBones.Length;
		var _bonesToInstantiateIndividually = new List<Transform> ();
		_bonesToInstantiateIndividually.AddRange (_renderer.bones);
		for (int k = 0; k < _bonesToInstantiateIndividually.Count; k++) {
			if (_bonesToInstantiateIndividually [k] != null) {
				for (int j = 0; j < _bonesToInstantiateIndividually.Count; j++) {
					if (_bonesToInstantiateIndividually [j] != null && j!= k) {
						if (_bonesToInstantiateIndividually [j].IsChildOf (_bonesToInstantiateIndividually [k]))
							_bonesToInstantiateIndividually [j] = null;
					}
				}
			}
		}
		// Instantiating a copy and saving the prefab
		if (!_bonesToInstantiateIndividually.Contains (_rootBone))
			Debug.LogError ("ERROR: The rootbone MUST be parented to other bones");
		var _bones = Instantiate(_rootBone.gameObject) as GameObject;
		_bones.SetActive (false);
		_bones.name = "Bones-" + id;
		for(int k = 0;k<_bonesToInstantiateIndividually.Count;k++) {
			if(_bonesToInstantiateIndividually[k] != null && _bonesToInstantiateIndividually[k] != _renderer.rootBone){
				var _boneCopy = Instantiate(_bonesToInstantiateIndividually[k].gameObject) as GameObject;
				_boneCopy.transform.parent = _bones.transform;
			}
		}
		var allGhostBones = _bones.GetComponentsInChildren<GhostBone> (true);
		for (int k=0; k< allGhostBones.Length; k++) {
			if(allGhostBones[k].ObjectID != id)	
				DestroyImmediate(allGhostBones[k]);
		}
		var allGhostable = _bones.GetComponentsInChildren<Ghostable> (true);
		for (int k=0; k< allGhostable.Length; k++) {
			if(allGhostable[k].ObjectID != id)	
				DestroyImmediate(allGhostable[k]);
		}
		var _allChildBones = _bones.GetComponentsInChildren<Transform> (true);
		for(int k =0;k<_allChildBones.Length;k++)
		{
			if (_allChildBones [k] != null) {
				if (_allChildBones [k].GetComponentsInChildren<GhostBone> (true).Length == 0) {
					DestroyImmediate(_allChildBones[k].gameObject);
				}
			}
		}
		if (_rootBone.parent != null) {
			_bones.transform.localScale = _rootBone.parent.transform.localScale;
		}
		PrefabUtility.CreatePrefab("Assets/HotTotemAssets/Resources/GhostToolPro/Bones-" + id + ".prefab",_bones);
		DestroyImmediate (_bones);
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.8f) );
		foreach (Material mat in _renderer.sharedMaterials) {
			var _mat = new Material (mat);
			string path = "Assets/HotTotemAssets/Resources/GhostToolPro/" + id + "-mat-" + i.ToString () + ".asset";
			AssetDatabase.CreateAsset (_mat, path);
			materialsPath [i - 1] = "GhostToolPro/" + id + "-mat-" + i.ToString ();
			i++;
		}
		GameObject _meshObject = Instantiate (_renderer.gameObject);
		var comps = _meshObject.GetComponents(typeof(Component));
		foreach (Component _comp in comps) {
			if ((_comp.GetType ()) != typeof(SkinnedMeshRenderer))
			if((_comp.GetType () != typeof(Ghostable)))
			if((_comp.GetType () != typeof(Transform)))
				DestroyImmediate (_comp);
		}
		PrefabUtility.CreatePrefab("Assets/HotTotemAssets/Resources/GhostToolPro/" + id + ".prefab",_meshObject);
		DestroyImmediate (_meshObject);
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(1f) );
		skinnedMeshRender = GetComponent<SkinnedMeshRenderer> ();
		EditorUtility.ClearProgressBar( );
	}
	[MenuItem("CONTEXT/MeshRenderer/Make Mesh Ghostable")]
	private static void MakeGhostable(MenuCommand menuCommand)
	{
		var _meshRenderer = menuCommand.context as MeshRenderer;
		EditorUtility.DisplayProgressBar( "Making "+_meshRenderer.name+" Ghostable", "Please wait a few seconds",(float)(0.1f) );
		_meshRenderer.gameObject.AddComponent<Ghostable> ();
		EditorUtility.DisplayProgressBar( "Making "+_meshRenderer.name+" Ghostable", "Please wait a few seconds",(float)(0.2f) );
		var _ghostable = _meshRenderer.GetComponent<Ghostable> ();
		_ghostable.Ghostify (_meshRenderer);
		_ghostable.placeHolderMaterials = new Material[_ghostable.materialsPath.Length];
		_ghostable.alternativeMaterials = new Material[_ghostable.materialsPath.Length];
	}

	[MenuItem("CONTEXT/SkinnedMeshRenderer/Make Skinned Mesh Ghostable")]
	private static void MakeSkinnedGhostable(MenuCommand menuCommand)
	{
		var _meshRenderer = menuCommand.context as SkinnedMeshRenderer;
		EditorUtility.DisplayProgressBar( "Making "+_meshRenderer.name+" Ghostable", "Please wait a few seconds",(float)(0.1f) );
		_meshRenderer.gameObject.AddComponent<Ghostable> ();
		EditorUtility.DisplayProgressBar( "Making "+_meshRenderer.name+" Ghostable", "Please wait a few seconds",(float)(0.2f) );
		var _ghostable = _meshRenderer.GetComponent<Ghostable> ();
		_ghostable.Ghostify (_meshRenderer);
		_ghostable.placeHolderMaterials = new Material[_ghostable.materialsPath.Length];
		_ghostable.alternativeMaterials = new Material[_ghostable.materialsPath.Length];
	}
	[MenuItem("CONTEXT/Ghostable/Instantiate Saved Mesh")]
	private static void InstantiateSaved(MenuCommand menuCommand)
	{
		// The RigidBody component can be extracted from the menu command using the context field.
		var _ghost = menuCommand.context as Ghostable;
		var _mesh = Resources.Load ("GhostToolPro/" + _ghost.id,typeof(Mesh)) as Mesh;
		var _materials = new Material[_ghost.materialsPath.Length];
		int i = 0;
		foreach(string _path in _ghost.materialsPath){
			_materials[i] = Resources.Load (_path) as Material;
			i++;
		}
		GameObject ghostObject = new GameObject ();
		ghostObject.name = "Ghost - " + _ghost.id;
		ghostObject.AddComponent<GhostObject> ();
		ghostObject.GetComponent<GhostObject> ().Init (_ghost.id);
		ghostObject.AddComponent<MeshRenderer> ();
		ghostObject.AddComponent<MeshFilter> ();
		ghostObject.GetComponent<MeshRenderer> ().sharedMaterials = _materials;
		ghostObject.GetComponent<MeshFilter> ().sharedMesh = _mesh;
	}
	#endif
	public static GameObject InstantiateSavedMesh(string _id,string[] _mat,bool _isSkinned)
	{
		// The RigidBody component can be extracted from the menu command using the context field.
		if (_isSkinned) {
			GameObject ghostObject = Instantiate (Resources.Load ("GhostToolPro/" + _id, typeof(GameObject)) as GameObject);
			GameObject _bones = null;
			_bones = Instantiate (Resources.Load ("GhostToolPro/Bones-" + _id, typeof(GameObject)) as GameObject);
			_bones.transform.SetParent (GhostTool.ghostParent);
			ghostObject.name = "Ghost - " + _id;
			ghostObject.AddComponent<GhostObject> ();
			ghostObject.GetComponent<GhostObject> ().Init (_id);
			var _skinnedMeshRenderer = ghostObject.GetComponent<SkinnedMeshRenderer> ();
			_skinnedMeshRenderer.rootBone = _bones.transform;
			var allBones = _bones.GetComponentsInChildren<Transform> (true);
			var _bonesAll = new Transform[_bones.GetComponent<GhostBone> ().size];
			var ghostBones = new Ghostable[_bones.GetComponent<GhostBone> ().size];
			foreach (var _b in allBones) {
				var _ghostBoneComponentInBone = _b.GetComponent<GhostBone> ();
				if (_ghostBoneComponentInBone != null) {
					var pos = _ghostBoneComponentInBone.position;
					if (pos >= 0 && pos < _bonesAll.Length) {
						_bonesAll [pos] = _b;
					}
				}
			}
			for (int m = 0; m < ghostBones.Length; m++) {
				ghostBones [m] = _bonesAll [m].gameObject.GetComponent<Ghostable> ();
			}
			_skinnedMeshRenderer.bones = _bonesAll;
			ghostObject.GetComponent<Ghostable> ().bones = ghostBones;
			var _materials = new Material[_mat.Length];
			int i = 0;
			foreach (string _path in _mat) {
				_materials [i] = Resources.Load (_path) as Material;
				i++;
			}
			_skinnedMeshRenderer.sharedMaterials = _materials;
			ghostObject.SetActive (false);
			var localParent = new GameObject ();
			var scale=  _bones.transform.localScale;
			localParent.transform.SetParent (GhostTool.ghostParent);
			_bones.transform.localScale = new Vector3 (1, 1, 1);
			localParent.transform.localScale = scale;
			localParent.name = "BonesParent";
			_bones.transform.SetParent (localParent.transform);
			ghostObject.transform.SetParent (GhostTool.ghostParent);
			return ghostObject;
		} else {
			GameObject ghostObject = new GameObject ();
			ghostObject.AddComponent<Ghostable> ();
			var _ghost = ghostObject.GetComponent<Ghostable> ();
			_ghost.id = _id;
			_ghost.materialsPath = _mat;
			var _mesh = Resources.Load ("GhostToolPro/" + _ghost.id, typeof(Mesh)) as Mesh;
			var _materials = new Material[_ghost.materialsPath.Length];
			int i = 0;
			foreach (string _path in _ghost.materialsPath) {
				_materials [i] = Resources.Load (_path) as Material;
				i++;
			}
			ghostObject.name = "Ghost - " + _ghost.id;
			ghostObject.AddComponent<GhostObject> ();
			ghostObject.GetComponent<GhostObject> ().Init (_ghost.id);
			ghostObject.AddComponent<MeshRenderer> ();
			ghostObject.AddComponent<MeshFilter> ();
			ghostObject.GetComponent<MeshRenderer> ().sharedMaterials = _materials;
			ghostObject.GetComponent<MeshFilter> ().sharedMesh = _mesh;
			ghostObject.SetActive (false);
			var localParent = new GameObject ();
			var scale=  ghostObject.transform.lossyScale;
			localParent.transform.SetParent (GhostTool.ghostParent);
			ghostObject.transform.localScale = new Vector3 (1, 1, 1);
			localParent.transform.localScale = scale;
			localParent.name = "MeshParent";
			ghostObject.transform.SetParent (localParent.transform);
			return ghostObject;
		}
	}

	public static GameObject InstantiateCopy(GameObject _copy)
	{
		GameObject ghostObject = Instantiate (_copy);
		ghostObject.transform.SetParent (GhostTool.ghostParent);
		return ghostObject;
	}
	public static T GetCopyOf<T>(Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) return null; // type mis-match
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach (var pinfo in pinfos) {
			if (pinfo.CanWrite) {
				try {
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos) {
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}

}
#if UNITY_EDITOR
[CustomEditor(typeof(Ghostable))]
public class GhostableEditor : Editor
{
	//Editor Code
	public override void OnInspectorGUI(){
		Ghostable myTarget = (Ghostable)target;
		if (myTarget.placeHolderMaterials != null) {
			for(int j=0;j<myTarget.placeHolderMaterials.Length;j++)
			{
				myTarget.placeHolderMaterials[j] = (Material)EditorGUILayout.ObjectField(
					"Custom Ghost Material "+j.ToString()+":", 
					myTarget.placeHolderMaterials[j],
					typeof(Material), 
					false);
			}
			if (myTarget.placeHolderMaterials != myTarget.alternativeMaterials) {
				myTarget.alternativeMaterials = myTarget.placeHolderMaterials;
				for (int i=1;i<=myTarget.placeHolderMaterials.Length;i++)
				{
					if(myTarget.placeHolderMaterials[i-1] != null)
					{
						var _mat = new Material (myTarget.placeHolderMaterials[i-1]);
						string path = "Assets/HotTotemAssets/Resources/GhostToolPro/" + myTarget.id + "-mat-" + i.ToString () + ".asset";
						AssetDatabase.CreateAsset (_mat, path);
						myTarget.materialsPath [i - 1] = "GhostToolPro/" + myTarget.id + "-mat-" + i.ToString ();
						Debug.Log ("Material Changed");
					}
				}
			} 
		}

	}
}
#endif