using System.Collections;
using InteractiveMusicPlayer;
using UnityEngine;

public class PaletteColor : MonoBehaviour
{
	private GameObject _instantiateCcursor;

	private void OnMouseDrag()
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward;
		_instantiateCcursor.transform.position = position;		
	}

	private void OnMouseDown()
	{				
		_instantiateCcursor = Instantiate(gameObject, transform.position, Quaternion.identity);
		_instantiateCcursor.transform.localScale *= 0.2f;		
	}

	private void OnMouseUp()
	{
		StartCoroutine(Shrink());	
	}

	private IEnumerator Shrink()
	{		
		while (_instantiateCcursor.transform.localScale.x > 0.01f)
		{
			_instantiateCcursor.transform.localScale -= new Vector3(0.02f, 0.02f, 0f);
			yield return new WaitForFixedUpdate();
		}
		Destroy(_instantiateCcursor);
	}
}
