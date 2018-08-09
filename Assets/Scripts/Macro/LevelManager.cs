using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

using InteractiveMusicPlayer;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;
	[ReadOnly] public int totalShapesInGame;	
	
	private List<ShapeInfo> _shapesNotMatched = new List<ShapeInfo>();
	private List<ShapeInfo> _shapesMatched = new List<ShapeInfo>();
	public UIFadeEffect panel;
	public List<ShapeInfo.ShapeInformation> correctShapes = new List<ShapeInfo.ShapeInformation>();

	public int attempts;	

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
		_shapesNotMatched.Add(shape);	
	}

	public void RemoveShape(ShapeInfo shape)
	{
		totalShapesInGame--;
		_shapesNotMatched.Remove(shape);
		if (totalShapesInGame == 0)					
			ReferenceMix.instance.Toggle.isOn = true;		
		CheckAnswer();		
	}

	private void Start()
	{
		totalShapesInGame = attempts = 0;
		MusicManager.SetParameterValue("WinProgress", SceneManager.GetActiveScene().name.Contains("SB")? 1 : 0);		
	}

	public void CheckAnswer()
	{
		ShapeInfo newCorrectShape = null;
		foreach (var shape in _shapesNotMatched) //iterating shapes that are not matched
		{
			foreach (var correctShape in correctShapes) //compare with each correct shapes
			{
				if (shape.Info.MatchExceptColor(correctShape) && !shape.isInCorrectPosition) //locks the position
				{
					shape.isInCorrectPosition = true;
					Destroy(shape.GetComponent<Draggable>());					
					Destroy(shape.GetComponent<Removable>());
					Destroy(shape.GetComponent<SnapToGrid>());
					Destroy(shape.GetComponent<ControlPanel>());
					SoundEffectManager.instance.LockShape(); //plays the sound of locking shape
				}
				if (shape.Info.Equals(correctShape)) //if the shape is completely correct
				{																				
					foreach (var s in _shapesMatched) //check repeating shapes
					{
						if (s != shape && s.Info.MatchColor(shape.Info))
						{														
							goto UpdateMusic;						
						}
					}
					shape.gameObject.layer = 1; //transparentFX
					_shapesMatched.Add(shape);
					newCorrectShape = shape; //can't remove within foreach
					Destroy(shape.GetComponent<Paintable>());
					SoundEffectManager.instance.CorrectShape();															
				}
			}
		}

		if (newCorrectShape) _shapesNotMatched.Remove(newCorrectShape); //removal outside of iteration
		
		UpdateMusic:
		WinProgress.UpdateProgress(_shapesMatched.Count);
		if (_shapesMatched.Count == correctShapes.Count && _shapesMatched.Count == totalShapesInGame)
		{
			SoundEffectManager.instance.StingerWin();
			panel.Fade(true);
		}
		else		
			MusicManager.SetParameterValue("WinProgress", _shapesMatched.Count);		
	}

}
