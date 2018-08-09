using UnityEngine;
using UnityEngine.SceneManagement;

public class SnapToGrid : MonoBehaviour {

    private ShapeInfo _shapeInfo;
    private bool _inShape;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("SB"))
            Destroy(this);        
        _shapeInfo = GetComponent<ShapeInfo>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Answer"))
            _inShape = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Answer"))
            _inShape = false;
    }

    private void OnMouseUp()
    {   
        if (_inShape)
            SnapToPosition();
        else
            _shapeInfo.Position = transform.position;
    }

    private void SnapToPosition()
    {
        float modifier = 10f;//2f / transform.localScale.x;
        float x = Mathf.RoundToInt(transform.position.x * modifier) / modifier;
        float y = Mathf.RoundToInt(transform.position.y * modifier) / modifier;
        _shapeInfo.Position = transform.position = new Vector3(x, y, 0);        
    }
}
