using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteColor : MonoBehaviour
{
	private GameObject cursorColor;	
	
	void OnMouseDrag()
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward;
		cursorColor.transform.position = position;		
	}

	void OnMouseDown()
	{
		cursorColor = Instantiate(gameObject, transform.position, Quaternion.identity);
		cursorColor.transform.localScale *= 0.2f;
	}

	private void OnMouseUp()
	{
		StartCoroutine(Shrink());	
	}

	IEnumerator Shrink()
	{		
		while (cursorColor.transform.localScale.x > 0.01f)
		{
			cursorColor.transform.localScale -= new Vector3(0.02f, 0.02f, 0f);
			yield return new WaitForFixedUpdate();
		}
		Destroy(cursorColor);
	}
}
