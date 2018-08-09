using UnityEngine;
using UnityEngine.SceneManagement;

public class SnapToShape : MonoBehaviour {
    
    private PolygonCollider2D _polygonCollider;
    private Rigidbody2D _rigidbody;
    
    public float snapDistance = 0.3f;
    private bool _inShape;

    private void Start()
    {
        if (!SceneManager.GetActiveScene().name.Contains("SB"))
            Destroy(this);
        _polygonCollider = GetComponent<PolygonCollider2D>();       
    }

    private void OnMouseDown()
    {        
        //prevents snapping raycast to collide with itself
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

    }

    private void OnMouseUp()
    {
        SnapToEdge();    
    }

    private void SnapToEdge()
    {
        //gets the rotation of shape in radian
        float rotationAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        for (int i = 0; i < _polygonCollider.points.Length; i++)
        {
            //needs an additional point for the last side
            int nextPoint = i + 1;
            if (i == _polygonCollider.points.Length - 1)
                nextPoint = 0;            
            //calculates vertice coordinates after rotation
            Vector2 thisVertice = new Vector2(_polygonCollider.points[i].x * Mathf.Cos(rotationAngle) - _polygonCollider.points[i].y * Mathf.Sin(rotationAngle),
                _polygonCollider.points[i].y * Mathf.Cos(rotationAngle) + _polygonCollider.points[i].x * Mathf.Sin(rotationAngle));
            Vector2 nextVertice = new Vector2(_polygonCollider.points[nextPoint].x * Mathf.Cos(rotationAngle) - _polygonCollider.points[nextPoint].y * Mathf.Sin(rotationAngle),
                _polygonCollider.points[nextPoint].y * Mathf.Cos(rotationAngle) + _polygonCollider.points[nextPoint].x * Mathf.Sin(rotationAngle));
            Vector2 rayStart = (thisVertice + nextVertice) / 2f + (Vector2)transform.position;        
            //gets the midpoint of each side
            Vector2 side = nextVertice - thisVertice;
            //gets the perpendicular vector
            Vector2 rayDirection = new Vector2(-side.y, side.x);
            //raycast from the side
            Debug.DrawRay(rayStart, rayDirection, Color.cyan, 0.5f); 
            RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, snapDistance); 	                        
            //make sure it hits something
            if (hit.collider)
            {
                if (hit.collider.CompareTag("Shape"))
                {
                    //snaps the shape to its closest edge
                    Vector2 moveDirection = hit.point - rayStart;
                    //the angle needs to be negative because we need to rotate the shape back
                    transform.Translate(new Vector2(moveDirection.x * Mathf.Cos(-rotationAngle) - moveDirection.y * Mathf.Sin(-rotationAngle), 
                        moveDirection.y * Mathf.Cos(-rotationAngle) + moveDirection.x * Mathf.Sin(-rotationAngle)) * 0.92f);
               }                
            }			
        }
        //reset the layer back to raycast detectable
        gameObject.layer = LayerMask.NameToLayer("Default");        
    }
}
