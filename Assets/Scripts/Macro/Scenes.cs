using System.Collections;
using System.Collections.Generic;
using InteractiveMusicPlayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{

	public static Scenes instance;
	private bool isLoading;
	
	private void Awake ()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(this);
	}
	
	public void ChangeScene(string nextScene)
	{
		if (nextScene == "this")
		{
			StartCoroutine(LoadLevelDelay(SceneManager.GetActiveScene().name));
			return;
		}

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
		if (isLoading) yield break;
		isLoading = true;	
		FadeScreen.fadeDirection = 1;
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		operation.allowSceneActivation = false;
		MusicManager.StopAll(1f);
		yield return new WaitForSeconds(1f);
		isLoading = false;
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

	public void Exit()
	{
		StartCoroutine(EndGame());
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
