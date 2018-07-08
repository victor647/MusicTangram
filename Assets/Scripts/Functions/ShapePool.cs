using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveMusicPlayer;

public class ShapePool : MonoBehaviour
{

	public GameObject shapePrefab;
	public GameObject musicPrefab;
	private GameObject tempShape;
	private GameObject generatedShape;
	private Vector2 offset;
	private Vector2 initialPosition;
	private Vector2 mousePosition;
	private bool tempInstantiated;

	private void OnMouseDown()
	{
		initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		offset = (Vector2)transform.position - initialPosition;				
	}

	private void OnMouseDrag()
	{
		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//checks if the player drags for a threshold distance
		if ((mousePosition - initialPosition).magnitude > 1f && !tempInstantiated)
		{
			tempInstantiated = true;
			tempShape = Instantiate(gameObject, transform.position, Quaternion.identity);					
		}
		//make sure the shape is instantiated
		if (tempShape)
		{
			tempShape.transform.position = (Vector3) (mousePosition + offset) + Vector3.back;			
		}

	}
	
	private void OnMouseUp()
	{
		//instantiate the actual shape from prefab
		if ((mousePosition - initialPosition).magnitude > 2f && tempInstantiated)
		{
			generatedShape = Instantiate(shapePrefab, tempShape.transform.position + Vector3.forward, Quaternion.identity);
			generatedShape.GetComponent<ShapeMusic>().musicPrefab = musicPrefab;
			generatedShape.transform.localScale *= 0.5f;
			StartCoroutine(Expand());
			tempInstantiated = false;						
			GetComponent<SampleMusic>().Reset();
		}
		else
		{
			Destroy(tempShape);
			tempInstantiated = false;
		}
	}

	//animation for expanding the shape
	private IEnumerator Expand()
	{
		Removable rmv = generatedShape.GetComponent<Removable>();
		while (generatedShape.transform.localScale.x < 1f)
		{	
			if (rmv.isDead) break;
			generatedShape.transform.localScale += new Vector3(0.1f, 0.1f, 0f);
			yield return new WaitForFixedUpdate();							
		}
		Destroy(tempShape);		
	}
}
