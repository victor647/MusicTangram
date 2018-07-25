using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

using InteractiveMusicPlayer;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;
	[ReadOnly] public int totalShapesInGame;	
	
	public delegate void Callback();

	public Callback FirstShapeAdded;
	public Callback LastShapeRemoved;
	private List<ShapeInfo> _shapes = new List<ShapeInfo>();
	public UIFadeEffect panel;
	public List<ShapeInfo.ShapeInformation> correctShapes = new List<ShapeInfo.ShapeInformation>();	


	private void Awake ()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(instance.gameObject);
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
			panel.Fade(true);			
		else		
			MusicManager.SetParameterValue("WinProgress", correctShapesNumber);		
	}

}
