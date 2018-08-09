using System.Collections;
using System.Collections.Generic;
using InteractiveMusicPlayer;
using UnityEngine;

public class UIFadeEffect : MonoBehaviour
{

	private MusicComponent _affectedMusic;

	private void Start()
	{
		if (gameObject.name == "WinPanel") 
			_affectedMusic = MusicManager.FindMusicByName("win");
	}

	public void Fade(bool isFadeIn)
	{
		gameObject.SetActive(true);		
		StartCoroutine(isFadeIn ? FadeIn() : FadeOut());
	}
	
	private IEnumerator FadeIn()
	{
		transform.localScale = Vector3.zero;		
		for (float scale = 0f; scale < 1f; scale += 0.05f)
		{						
			transform.localScale = new Vector3(scale, scale, 0f);
			CheckMusic(scale);
			yield return new WaitForFixedUpdate();							
		}
	}
	
	private IEnumerator FadeOut()
	{
		transform.localScale = Vector3.one;		
		for (float scale = 1f; scale > 0f; scale -= 0.05f)
		{						
			transform.localScale = new Vector3(scale, scale, 0f);
			CheckMusic(scale);
			yield return new WaitForFixedUpdate();							
		}
		gameObject.SetActive(false);
	}

	void CheckMusic(float scale)
	{
		if (!_affectedMusic) return;
		
		_affectedMusic.SetVolume(0.6f + scale / 2f);
		_affectedMusic.SetLPFCutoff(4000f + scale * 18000f);
	}
}
