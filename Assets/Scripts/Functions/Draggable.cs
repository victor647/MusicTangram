using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Draggable : MonoBehaviour {

    private Vector2 _offset;
    private PolygonCollider2D _collider;    
    private Rigidbody2D _rigidBody;    

    private void Start()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();        
    }

    void OnMouseDown()
    {
        //mouse follows whereever player clicks on the shape
        _offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //enables overlapping with other shapes when dragging
        _collider.isTrigger = true;
        //repels when landing on another shape
        _rigidBody.bodyType = RigidbodyType2D.Dynamic;
        
    }

    void OnMouseDrag()
    {		
        //updates position with mouse position
        Vector2 position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + _offset;
        transform.position = new Vector3(position.x, position.y, -1f);		
    }

    private void OnMouseUp()
    {
        //makes the shape repel with overlapping shapes
        _collider.isTrigger = false;
        //resets the sorting layer
        transform.Translate(Vector3.forward); 
        Invoke("LockPosition", 0.3f);        
        
    }


    void LockPosition()
    {
        //lock the position of shape after moving in a short time
        _rigidBody.bodyType = RigidbodyType2D.Kinematic;
    }

}