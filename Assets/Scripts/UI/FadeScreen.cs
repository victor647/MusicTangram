using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class FadeScreen : MonoBehaviour {
 
	public Texture2D fadeTexture;
 
	[Range(0.1f,1f)]
	public float fadespeed = 0.02f;
	public int drawDepth = -1000;
 
	private float alpha = 1f;
	public bool fadeInOnStart = true;
	public static int fadeDirection;

	private void Awake()
	{
		fadeDirection = -1;
		alpha = fadeInOnStart ? 1 : 0;
	}

	void OnGUI() {		
		alpha += fadeDirection * fadespeed * Time.deltaTime;
		alpha = Mathf.Clamp01(alpha);		 		
		
		Color newColor = GUI.color; 
		newColor.a = alpha;
 
		GUI.color = newColor;
 
		GUI.depth = drawDepth;
 
		GUI.DrawTexture( new Rect(0,0, Screen.width, Screen.height), fadeTexture);
 
	}
}
