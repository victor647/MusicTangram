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
		
		if (nextScene.Contains("Level"))		
			StartCoroutine(LoadLevelDelay(nextScene));		
		else if (SceneManager.GetActiveScene().name.Contains("Level"))
		{
			StartCoroutine(LoadLevelDelay(nextScene));	
			Invoke("MainMenuMusic", 0.9f);
		}
		else		
			SceneManager.LoadScene(nextScene);											
	}

	IEnumerator LoadLevelDelay(string scene)
	{
		FadeScreen.fadeDirection = 1;
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		operation.allowSceneActivation = false;
		MusicManager.StopAll(1f);
		yield return new WaitForSeconds(1f);		
		operation.allowSceneActivation = true;
		
	}

	private void MainMenuMusic()
	{
		MusicManager.PostEvent("Play_main_menu");
	} 
	
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			StartCoroutine(EndGame());
		}
	}

	IEnumerator EndGame()
	{
		FadeScreen.fadeDirection = 1;
		MusicManager.StopAll(1f);			
		
		yield return new WaitForSeconds(1.2f);		
		
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
