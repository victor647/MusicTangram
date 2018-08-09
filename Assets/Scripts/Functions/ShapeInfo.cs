using UnityEngine;

public class ShapeInfo : MonoBehaviour
{
	public enum ShapeType
	{
		sqr, tri, rct, pll, plr, tpz
	}

	public ShapeType type;
	[ReadOnly] public bool isInCorrectPosition;	
	
	[System.Serializable]
	public struct ShapeInformation
	{
		public ShapeType Type;
		public int RotationIndex;
		public int SizeIndex;
		public string Color;
		public Vector2 Position;
		public Vector2 SecondaryPosition;

		public bool MatchExceptColor(ShapeInformation s)
		{
			return  MatchPosition(s) && s.Type == Type && s.RotationIndex == RotationIndex && s.SizeIndex == SizeIndex;
		}

		public bool MatchColor(ShapeInformation s)
		{
			return Color == s.Color && Type == s.Type;
		}
		
		private bool MatchPosition(ShapeInformation s)
		{
			if (s.SecondaryPosition == Vector2.zero)
				return (s.Position - Position).magnitude < 0.01f;
			return (s.Position - Position).magnitude < 0.01f || (s.SecondaryPosition - Position).magnitude < 0.01f;
		}
		
		public bool Equals(ShapeInformation s)
		{
			return Type == s.Type && RotationIndex == s.RotationIndex
				&& SizeIndex == s.SizeIndex && Color == s.Color && MatchPosition(s);
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
			if (LevelManager.instance)
			{
				LevelManager.instance.attempts++;
				LevelManager.instance.CheckAnswer();				
			}
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
