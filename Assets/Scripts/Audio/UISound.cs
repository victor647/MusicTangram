using UnityEngine;

public class UISound : MonoBehaviour
{

	public AudioClip soundEffect;

	private AudioSource _source;
	// Use this for initialization
	void Start ()
	{
		_source = GetComponent<AudioSource>();
		if (!_source)
			_source = gameObject.AddComponent<AudioSource>();
	}

	public void TriggerSound()
	{
		_source.PlayOneShot(soundEffect);
	}
}
