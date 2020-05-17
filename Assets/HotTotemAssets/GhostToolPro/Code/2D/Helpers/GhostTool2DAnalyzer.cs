#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// Editor window for listing all object reference curves in an animation clip
using System.Collections.Generic;

namespace GhostToolPro {
	public class GhostTool2DAnalyzer
{
	#if UNITY_EDITOR
	public static List<Sprite> GetSpritesFromAnimator(Animator anim)
	{
		List<Sprite> _allSprites = new List<Sprite> ();
		foreach(AnimationClip ac in anim.runtimeAnimatorController.animationClips)
		{
			_allSprites.AddRange(GetSpritesFromClip(ac));
		}
		return _allSprites;
	}

	private static List<Sprite> GetSpritesFromClip(AnimationClip clip)
	{
		var _sprites = new List<Sprite> ();
		if (clip != null)
		{
			foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings (clip))
			{
				ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve (clip, binding);
				foreach (var frame in keyframes) {
					_sprites.Add ((Sprite)frame.value);
				}
			}
		}
		return _sprites;
	}
	#endif
}
}