using System.Collections;
using System.Collections.Generic;
using InteractiveMusicPlayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{	
	
	public void ChangeScene(string nextScene)
	{		
		if (nextScene == "Menu")
			Destroy(MusicManager.Instance.gameObject);
		SceneManager.LoadScene(nextScene);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void EndGame()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
