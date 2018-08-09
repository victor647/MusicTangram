using UnityEngine.UI;
using UnityEngine;
using InteractiveMusicPlayer;
using UnityEngine.EventSystems;

public class ReferenceMix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public static ReferenceMix instance;				
	private Toggle _toggle;
	public Toggle Toggle
	{
		get { return _toggle; }
	}
	
	private Image _icon;
	[HideInInspector] public bool mouseOver;
		
	private void Awake()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(instance);
	}	
	
	private void Start()
	{
		_toggle = GetComponent<Toggle>();
		_icon = GetComponent<Image>();
		RefMusic(true);
	}
	
	public void RefMusic(bool isOn)
	{
		var refMix = MusicManager.FindMusicByName("ref");
		if (isOn){
			refMix.UnMute(1f);
			if (SampleMusic.HighlightedObject) SampleMusic.HighlightedObject.Reset();
			MixerManager.instance.SetMixerSnapshot(MixerManager.instance.refOnly, 0.8f);
			_icon.color = _toggle.colors.normalColor;
		}
		else
		{
			refMix.Mute(1f);
			MixerManager.instance.SetMixerSnapshot(MixerManager.instance.allOn, 0.8f);
			if (SampleMusic.HighlightedObject) SampleMusic.HighlightedObject.Reset();
			_icon.color = _toggle.colors.disabledColor;
		}		
	}		

	public void OnPointerEnter(PointerEventData eventData)
	{		
		mouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{	
		mouseOver = false;
	}
}
