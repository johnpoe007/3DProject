using UnityEngine;
using System.Collections;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using GhostToolPro;

public class Ghostable2D : MonoBehaviour {
	[SerializeField]
	[HideInInspector]
	public string id;
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
	public SpriteRenderer spriteRender;

	public Ghostable2D()
	{
	}
	public Ghostable2D(string _id, string[] _matPath)
	{
		id = _id;
		materialsPath = _matPath;
	}

	#if UNITY_EDITOR
	public void Ghostify(SpriteRenderer _renderer)
	{
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.3f) );
		id = (Guid.NewGuid()).ToString();
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.4f) );
		if (!Directory.Exists(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro"))
		{
			Directory.CreateDirectory(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro");
		} 
		if (!Directory.Exists(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro/" + id))
		{
			Directory.CreateDirectory(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro/" + id);
		} 
		if (!Directory.Exists(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro/" + id + "/Sprites"))
		{
			Directory.CreateDirectory(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro/" + id + "/Sprites");
		} 
		if (!Directory.Exists(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro/" + id + "/Materials"))
		{
			Directory.CreateDirectory(Application.dataPath +"/HotTotemAssets/Resources/GhostToolPro/" + id + "/Materials");
		} 
		var _animator = _renderer.gameObject.GetComponent<Animator> ();
		if (_animator == null) {
			var _sprite = _renderer.sprite;
			Bounds bounds = _sprite.bounds;
			var pivotX = - bounds.center.x / bounds.extents.x / 2 + 0.5f;
			var pivotY = - bounds.center.y / bounds.extents.y / 2 + 0.5f;
			var normalizedPivot = new Vector2 (pivotX, pivotY);
			Sprite _tmpSprite = Sprite.Create (_sprite.texture, _sprite.rect,normalizedPivot,_sprite.pixelsPerUnit,0,SpriteMeshType.Tight,_sprite.border);
			_tmpSprite.name = _sprite.name;
			AssetDatabase.CreateAsset (_tmpSprite,"Assets/HotTotemAssets/Resources/GhostToolPro/" + id + "/Sprites/" +_tmpSprite.name + ".asset");
		} else {
			var _sprites = GhostTool2DAnalyzer.GetSpritesFromAnimator (_animator);
			int progressbar = 0;
			foreach (var _sprite in _sprites) {
				EditorUtility.DisplayProgressBar( "Making "+_sprite.name+" Ghostable", "Please wait a few seconds",((float)(progressbar)/_sprites.Count + (1f/3f)/_sprites.Count) );
				Bounds bounds = _sprite.bounds;
				var pivotX = - bounds.center.x / bounds.extents.x / 2 + 0.5f;
				var pivotY = - bounds.center.y / bounds.extents.y / 2 + 0.5f;
				var normalizedPivot = new Vector2 (pivotX, pivotY);
				Sprite _tmpSprite = Sprite.Create (_sprite.texture, _sprite.rect,normalizedPivot,_sprite.pixelsPerUnit,0,SpriteMeshType.Tight,_sprite.border);
				EditorUtility.DisplayProgressBar( "Making "+_sprite.name+" Ghostable", "Please wait a few seconds",((float)(progressbar)/_sprites.Count + (2f/3f)/_sprites.Count) );
				_tmpSprite.name = _sprite.name;
				AssetDatabase.CreateAsset (_tmpSprite,"Assets/HotTotemAssets/Resources/GhostToolPro/" + id + "/Sprites/" +_tmpSprite.name + ".asset");
				EditorUtility.DisplayProgressBar( "Making "+_sprite.name+" Ghostable", "Please wait a few seconds",((float)(progressbar)/_sprites.Count + (3f/3f)/_sprites.Count) );
				progressbar++;
			}
		}
		int i = 1;
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.6f) );
		materialsPath = new string[_renderer.sharedMaterials.Length];
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(0.8f) );
		foreach (Material mat in _renderer.sharedMaterials) {
			var _mat = new Material (mat);
			string path = "Assets/HotTotemAssets/Resources/GhostToolPro/" + id + "/Materials/Mat-" + i.ToString () + ".asset";
			AssetDatabase.CreateAsset (_mat, path);
			materialsPath [i - 1] = "GhostToolPro/" + id + "/Materials/Mat-" + i.ToString ();
			i++;
		}
		EditorUtility.DisplayProgressBar( "Making "+_renderer.name+" Ghostable", "Please wait a few seconds",(float)(1f) );
		spriteRender = _renderer;
		EditorUtility.ClearProgressBar( );
	}

	[MenuItem("CONTEXT/SpriteRenderer/Make 2D Object Ghostable")]
	private static void MakeGhostable(MenuCommand menuCommand)
	{
		var _spriteRenderer = menuCommand.context as SpriteRenderer;
		EditorUtility.DisplayProgressBar( "Making "+_spriteRenderer.name+" Ghostable", "Please wait a few seconds",(float)(0.1f) );
		var _ghostable = _spriteRenderer.gameObject.AddComponent<Ghostable2D> ();
		EditorUtility.DisplayProgressBar( "Making "+_spriteRenderer.name+" Ghostable", "Please wait a few seconds",(float)(0.2f) );
		_ghostable.Ghostify (_spriteRenderer);
		_ghostable.placeHolderMaterials = new Material[_ghostable.materialsPath.Length];
		_ghostable.alternativeMaterials = new Material[_ghostable.materialsPath.Length];
	}
	#endif
	public static GameObject InstantiateSavedSprites(string _id,string[] _mat)
	{
		
		GameObject ghostObject = new GameObject ();
		var _ghost = ghostObject.AddComponent<Ghostable2D> ();
		_ghost.id = _id;
		_ghost.materialsPath = _mat;
		var _tmpSprites = Resources.LoadAll ("GhostToolPro/"  + _ghost.id +"/Sprites");
		var _sprites = new Sprite[_tmpSprites.Length];
		for (int j = 0; j < _tmpSprites.Length; j++)
			_sprites [j] = (Sprite)_tmpSprites [j];
		var _materials = new Material[_ghost.materialsPath.Length];
		int i = 0;
		foreach (string _path in _ghost.materialsPath) {
			_materials [i] = Resources.Load (_path) as Material;
			i++;
		}
		ghostObject.name = "Ghost2D - " + _ghost.id;
		ghostObject.AddComponent<GhostObject2D> ();
		ghostObject.GetComponent<GhostObject2D> ().Init (_ghost.id,_sprites.ToList());
		var _spriteRenderer = ghostObject.AddComponent<SpriteRenderer> ();
		_spriteRenderer.sharedMaterials = _materials;
		_spriteRenderer.sprite = _sprites[0];
		ghostObject.SetActive (false);
		ghostObject.transform.SetParent (GhostTool2D.ghostParent);
		return ghostObject;
	}

	public static GameObject InstantiateCopy(GameObject _copy)
	{
		var ghostObject = Instantiate (_copy);
		ghostObject.transform.SetParent (GhostTool2D.ghostParent);
		return ghostObject;
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(Ghostable2D))]
public class Ghostable2DEditor : Editor
{
	//Editor Code
	public override void OnInspectorGUI(){
		Ghostable2D myTarget = (Ghostable2D)target;
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
						string path = "Assets/HotTotemAssets/Resources/GhostToolPro/" + myTarget.id + "/Materials/Mat-" + i.ToString () + ".asset";
						AssetDatabase.CreateAsset (_mat, path);
						myTarget.materialsPath [i - 1] = "GhostToolPro/" + myTarget.id + "/Materials/Mat-" + i.ToString ();
						Debug.Log ("Material Changed");
					}
				}
			} 
		}

	}
}
#endif