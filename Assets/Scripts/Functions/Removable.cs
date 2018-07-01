using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Removable : MonoBehaviour
{	
	public bool isDead;

	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.name == "Trash" && !Input.GetMouseButton(0) && !isDead)
		{
			GetComponent<ShapeMusic>().Removed();
			StartCoroutine(Shrink());			
		}
	}

	IEnumerator Shrink()
	{
		isDead = true;
		while (transform.localScale.x > 0.1f)
		{
			transform.localScale -= new Vector3(0.1f, 0.1f, 0f);
			yield return new WaitForFixedUpdate();
		}
		Destroy(gameObject);
	}
		
}
