using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InteractiveMusicPlayer;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	[ReadOnly] public int totalShapesInGame;	
	
	public delegate void Callback();

	public Callback FirstShapeAdded;
	public Callback LastShapeRemoved;
	private List<ShapeInfo> _shapes = new List<ShapeInfo>();
	public GameObject panel;
	public List<ShapeInfo.ShapeInformation> correctShapes = new List<ShapeInfo.ShapeInformation>();
	public MusicTrack flute;


	private void Awake ()
	{
		if (!instance)
			instance = this;
		else
		{
			Destroy(instance.gameObject);
		}
	}

	private void Start()
	{
		MusicManager.PostEvent("Stop_main_menu");
	}

	public void AddShape(ShapeInfo shape)
	{
		totalShapesInGame++;
		_shapes.Add(shape);
		if (totalShapesInGame == 1)
		{
			if (FirstShapeAdded != null) FirstShapeAdded();
		}
	}

	public void RemoveShape(ShapeInfo shape)
	{
		totalShapesInGame--;
		_shapes.Remove(shape);
		if (totalShapesInGame == 0)
		{
			if (LastShapeRemoved != null) LastShapeRemoved();
		}
		CheckAnswer();
	}

	public void CheckAnswer()
	{
		int correctShapesNumber = 0;
		foreach (var shape in _shapes)
		{
			foreach (var correctShape in correctShapes)
			{
				if (shape.Info == correctShape)
				{
					if (shape.gameObject.layer == 0)
					{
						shape.gameObject.layer = 1; //transparentFX
						Destroy(shape.GetComponent<Draggable>());
						Destroy(shape.GetComponent<Paintable>());
						Destroy(shape.GetComponent<Removable>());
						Destroy(shape.GetComponent<Snappable>());
						Destroy(shape.GetComponent<ControlPanel>());
					}
					correctShapesNumber++;
				}					
			}
		}
		WinProgress.UpdateProgress(correctShapesNumber);		
		if (correctShapesNumber == correctShapes.Count && correctShapesNumber == totalShapesInGame)		
			LevelComplete();			
		else		
			MusicManager.SetParameterValue("WinProgress", correctShapesNumber);		
	}

	private void LevelComplete()
	{				
		StartCoroutine(PanelFadeIn());
	}

	private IEnumerator PanelFadeIn()
	{
		panel.transform.localScale = Vector3.zero;
		panel.SetActive(true);
		float scale = 0f;
		while (scale < 1f)
		{
			scale += 0.05f;			
			panel.transform.localScale = new Vector3(scale, scale, 0f);
			flute.SetVolume(0.6f + scale / 2f);
			flute.SetLPFCutoff(4000f + scale * 18000f);
			yield return new WaitForFixedUpdate();							
		}
	}
}
