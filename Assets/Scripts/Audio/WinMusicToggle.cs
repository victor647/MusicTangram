﻿using InteractiveMusicPlayer;
using UnityEngine;

public class WinMusicToggle : MonoBehaviour
{

	MusicComponent winMusic;

	private void Start()
	{
		winMusic = MusicManager.FindMusicByName("win");
		SetMute(false);
	}

	public void SetMute(bool toggle)
	{
		if (toggle)
			winMusic.UnMute(1f);
		else
			winMusic.Mute(1f);
	}
}
