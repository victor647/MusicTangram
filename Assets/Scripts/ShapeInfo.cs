using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeInfo : MonoBehaviour
{
	public enum ShapeType
	{
		sqr, tri, rct, pll, plr, tpz
	}

	public ShapeType type;
	public int rotationIndex;
	public int sizeIndex;
	public string color;
	
	// Use this for initialization
	void Start () {
		GameManager.instance.AddShape(this);	
	}

	private void OnDestroy()
	{
		GameManager.instance.RemoveShape(this);
	}


	public void ChangeRotation(int increment)
	{
		rotationIndex += increment;
		if (rotationIndex < 0) rotationIndex = 7;
		
		switch (type)
		{
			case ShapeType.sqr:				
				rotationIndex %= 2;
				break;
			case ShapeType.rct:
			case ShapeType.pll:
			case ShapeType.plr:
				rotationIndex %= 4;
				break;
		}
	}
		
}
