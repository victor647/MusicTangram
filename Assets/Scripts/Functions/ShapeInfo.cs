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
	
	[System.Serializable]
	public struct ShapeInformation
	{
		public ShapeType Type;
		public int RotationIndex;
		public int SizeIndex;
		public string Color;
		public Vector2 Position;
		
		public static bool operator == (ShapeInformation x, ShapeInformation y)
		{
			return x.Type == y.Type && x.RotationIndex == y.RotationIndex 
				&& x.SizeIndex == y.SizeIndex && x.Color == y.Color && (y.Position - x.Position).magnitude < 0.01f;
		}
		
		public static bool operator != (ShapeInformation x, ShapeInformation y)
		{
			return x.Type != y.Type || x.RotationIndex != y.RotationIndex 
				|| x.SizeIndex != y.SizeIndex || x.Color != y.Color || (y.Position - x.Position).magnitude > 0.01f;
		}		
	}

	private ShapeInformation _info;
	public ShapeInformation Info
	{
		get { return _info; }		
	}

	public string Color
	{
		set
		{
			_info.Color = value;
			if (LevelManager.instance) LevelManager.instance.CheckAnswer();
		}
	}
	
	public Vector2 Position
	{
		set
		{
			_info.Position = value;
			if (LevelManager.instance) LevelManager.instance.CheckAnswer();
		}
	}
	
	public int SizeIndex
	{	
		get { return _info.SizeIndex;}
		set
		{
			_info.SizeIndex = value;
			if (LevelManager.instance) LevelManager.instance.CheckAnswer();
		}
	}
	
	public int RotationIndex
	{
		get { return _info.RotationIndex;}
		set
		{
			_info.RotationIndex = value;
			if (_info.RotationIndex < 0) _info.RotationIndex = 7;

			switch (_info.Type)
			{
				case ShapeType.sqr:
					_info.RotationIndex %= 2;
					break;
				case ShapeType.rct:
				case ShapeType.pll:
				case ShapeType.plr:
					_info.RotationIndex %= 4;
					break;
				default:
					_info.RotationIndex %= 8;
					break;
			}			
			if (LevelManager.instance) LevelManager.instance.CheckAnswer();
		}
	}

	// Use this for initialization
	private void Start ()
	{
		_info.Type = type;
		if (LevelManager.instance) LevelManager.instance.AddShape(this);
		Position = transform.position;
	}

	private void OnDestroy()
	{		
		if (LevelManager.instance) LevelManager.instance.RemoveShape(this);
	}


	
		
}
