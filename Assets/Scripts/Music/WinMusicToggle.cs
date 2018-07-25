using InteractiveMusicPlayer;
using UnityEngine;

public class WinMusicToggle : MonoBehaviour
{

	public MusicComponent winMusic;

	private void Start()
	{
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
