using System.Collections;
using System.Collections.Generic;
using InteractiveMusicPlayer;
using UnityEngine;

public class MusicInstance : MonoBehaviour
{
	[SerializeField] 
	private bool _overriteComponentName = false;
	
	[Tooltip("The name of the singleton instance of the music")]
	[ConditionalHide("_overriteComponentName", true)]
	[SerializeField]
	private string _name;

	private MusicComponent _linkedComponent;

	private void OnEnable ()
	{
		var linkedComponent = GetComponent<MusicComponent>();
		if (!linkedComponent)		
			Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Music instance can't find linked music component!");
		else
		{
			if (!_overriteComponentName)
				_name = linkedComponent.Name;
			MusicManager.Instance.RegisterMusicInstance(_name, linkedComponent);
		}
	}

	private void OnDisable()
	{
		if (MusicManager.Instance) 
			MusicManager.Instance.UnRegisterMusicInstance(_name);
	}
}
