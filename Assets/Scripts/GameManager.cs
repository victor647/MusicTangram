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
	
	void Awake ()
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
		MusicManager.Instance.PostEvent("Stop_main_menu");
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
	}
}
