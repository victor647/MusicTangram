using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlPanel : MonoBehaviour
{

	public GameObject funtionBoardPrefab;
	public static GameObject currentBoard;
	public static GameObject currentShape;
	public bool toggle;

	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(1))
		{
			toggle = !toggle;	
			//reset the toggle of other shapes		
			if (currentShape)
			{
				if (currentShape != gameObject)
				{
					currentShape.GetComponent<ControlPanel>().toggle = false;
					Destroy(currentBoard);
				}
			}		
			if (!toggle)
				Destroy(currentBoard);
			//links the selection shape
			currentShape = gameObject;
			//if the player clicks again after dragging
			
		}

		if (Input.GetMouseButtonUp(1))
		{
			//generates the board if toggle is on
			if (toggle)
			{
				currentBoard = Instantiate(funtionBoardPrefab, transform.position + Vector3.back, Quaternion.identity);			
			}	
		}	
	}
	
	private void Update()
	{
		//if player clicks on empty place, the board disappears
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (currentBoard)
			{
				if (!hit.collider)
				{
					Destroy(currentBoard);
					toggle = false;
				}
				else if (!hit.collider.CompareTag("FunctionBoard"))
				{
					Destroy(currentBoard);
					toggle = false;
					Debug.Log(hit.collider.gameObject);
				}
			}
		}
	}
}
