using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{

	public static SoundEffectManager instance;	  
	private AudioSource _source;
	
	public AudioClip shapePlacement;
	public AudioClip shapeLock;
	public AudioClip shapeCorrectAcoustic;
	public AudioClip shapeCorrectElectronic;
	public AudioClip shapeRemove;
	public AudioClip winStingerAcoustic;
	public AudioClip winStingerElectronic;
	
	private void Awake ()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(this);
	}
		
	private void Start ()
	{		
		_source = GetComponent<AudioSource>();
		_source.spatialBlend = 0f;
	}

	public void PlaceShape()
	{
		RandomPitch();
		_source.PlayOneShot(shapePlacement, 1f);
	}
	
	public void LockShape()
	{
		RandomPitch();
		_source.PlayOneShot(shapeLock, 1f);
	}
	
	public void RemoveShape()
	{
		RandomPitch();
		_source.PlayOneShot(shapeRemove, 1f);
	}
	
	public void CorrectShape()
	{
		_source.pitch = 1f;
		if (SceneManager.GetActiveScene().name.Contains("Level_4"))
			_source.PlayOneShot(shapeCorrectElectronic, 1f);
		else
			_source.PlayOneShot(shapeCorrectAcoustic, 1f);
	}
	
	public void StingerWin()
	{
		_source.pitch = 1f;
		if (SceneManager.GetActiveScene().name.Contains("Level_4"))
			_source.PlayOneShot(winStingerElectronic, 1f);
		else
			_source.PlayOneShot(winStingerAcoustic, 1f);
	}
	
	private void RandomPitch()
	{
		_source.pitch = Random.Range(0.9f, 1.1f);
	}
}
