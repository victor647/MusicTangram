using UnityEngine;

public class ScreenPosition : MonoBehaviour {

	public enum screenPosition
	{
		UpperLeft, UpperRight, LowerLeft, LowerRight, Center, Left, Right, Up, Down
	}

	public screenPosition position;
	public Vector2 offset;

	private void Start () {
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		switch (position)
		{
			case screenPosition.Center:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0));
				break;
			case screenPosition.LowerLeft:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
				break;
			case screenPosition.LowerRight:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(1,0,0));
				break;
			case screenPosition.UpperLeft:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
				break;
			case screenPosition.UpperRight:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(1,1,0));
				break;		
			case screenPosition.Up:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f,1,0));
				break;
			case screenPosition.Down:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0,0));
				break;
			case screenPosition.Left:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0,0.5f,0));
				break;
			case screenPosition.Right:
				transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(1,0.5f,0));
				break;
		}		
		transform.Translate(offset);
	}
}
