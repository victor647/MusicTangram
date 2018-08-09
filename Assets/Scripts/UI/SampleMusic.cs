using InteractiveMusicPlayer;
using UnityEngine;

public class SampleMusic : MonoBehaviour
{    		
    private MeshRenderer _mesh;
    public static SampleMusic HighlightedObject;
    private Material _originalMaterial;
    private Material _tempMaterial;
    private Color32 _tempColor;
    private bool _toggleOn;    

    private void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _originalMaterial = _mesh.material;        
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (!_toggleOn)
            {
                if (HighlightedObject)
                {
                    HighlightedObject.Reset();
                }
                HighlightedObject = this;
                Focus();
            }
            else            
                Reset();                            
        }		
    }

    private void Focus()
    {
        MixerManager.instance.SetMixerSnapshot(MixerManager.instance.sampleOnly, 1f);
        _toggleOn = true;        
        _tempMaterial = new Material(_originalMaterial);
        _tempColor = _tempMaterial.color;
        _tempColor.a = 255;
        _tempMaterial.color = _tempColor;
        _mesh.material = _tempMaterial;
        MixerManager.instance.sampleTrack.SetSwitch(gameObject.name, 0.5f);        
    }

    public void Reset()
    {
        if (MixerManager.instance.currentMixer == MixerManager.instance.sampleOnly)
            MixerManager.instance.SetMixerSnapshot(MixerManager.instance.allOn, 1f);
        _toggleOn = false;
        _mesh.material = _originalMaterial;          
        MixerManager.instance.sampleTrack.SetSwitch("", 0.5f);        
    }        
    
    private void Update()
    {
        //if player clicks on empty place, the board disappears
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (HighlightedObject)
            {
                if (!hit.collider)                
                    Reset();                              
                else if (hit.collider.gameObject != HighlightedObject.gameObject)                
                    Reset();                                    
            }
        }
    }
}