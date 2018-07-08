using UnityEngine;
using UnityEngine.UI;

public class WinProgress : MonoBehaviour
{

	private static Text _text;

	private void Start ()
	{
		_text = GetComponent<Text>();
	}

	public static void UpdateProgress(int number)
	{
		_text.text = number + "/" + GameManager.instance.correctShapes.Count;
	}				
}
