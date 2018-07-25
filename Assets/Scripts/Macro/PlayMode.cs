using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMode : MonoBehaviour
{
	private void Awake()
	{
		PlayerPrefs.SetInt("sandbox", 0);
	}

	public void SetPlayMode(bool mode)
	{		
		PlayerPrefs.SetInt("sandbox", mode? 1 : 0);
	}
}
