using System.Collections;
using System.Threading;
using InteractiveMusicPlayer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

    Slider _slider;

	public string levelName;
	// Use this for initialization
	void Start ()
	{
		_slider = GetComponent<Slider>();
		StartCoroutine(Loading());
	}

	IEnumerator Loading()
	{		
		AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
		operation.allowSceneActivation = false;		
		float progress = 0f;
		while (progress < 1f)
		{						
			_slider.value = progress;
			progress += .05f;
			yield return new WaitForFixedUpdate();
		}

		_slider.value = 1f;
		yield return new WaitForSecondsRealtime(0.2f);
		operation.allowSceneActivation = true;
		MusicManager.Instance.PostEvent("Play_main_menu");
	}
}
