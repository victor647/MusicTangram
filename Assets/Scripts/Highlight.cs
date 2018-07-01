using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {

	// Use this for initialization
	private void OnMouseEnter()
	{
		transform.localScale = Vector3.one;
	}

	private void OnMouseExit()
	{
		transform.localScale = Vector3.one * 0.5f;
	}
}
