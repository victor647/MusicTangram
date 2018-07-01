using UnityEngine;

public class SampleMusic : MonoBehaviour
{    		
    private MeshRenderer mesh;
    public static SampleMusic highlightedObject;
    private Material originalMaterial;
    private Material tempMaterial;
    private Color32 tempColor;
    private bool toggleOn;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        originalMaterial = mesh.material;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (!toggleOn)
            {
                if (highlightedObject)
                {
                    highlightedObject.Reset();
                }
                highlightedObject = this;
                Focus();
            }
            else            
                Reset();                            
        }		
    }

    void Focus()
    {
        MixerManager.instance.SetMixerSnapshot(MixerManager.instance.sampleOnly, 1f);
        toggleOn = true;        
        tempMaterial = new Material(originalMaterial);
        tempColor = tempMaterial.color;
        tempColor.a = 255;
        tempMaterial.color = tempColor;
        mesh.material = tempMaterial;
        MixerManager.instance.sampleTrack.SetSwitch(gameObject.name);        
    }

    public void Reset()
    {
        if (MixerManager.instance.currentMixer == MixerManager.instance.sampleOnly)
            MixerManager.instance.SetMixerSnapshot(MixerManager.instance.allOn, 1f);
        toggleOn = false;
        mesh.material = originalMaterial;          
        MixerManager.instance.sampleTrack.SetSwitch("");	   
    }        
    
    private void Update()
    {
        //if player clicks on empty place, the board disappears
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (highlightedObject)
            {
                if (!hit.collider)                
                    Reset();                              
                else if (hit.collider.gameObject != highlightedObject.gameObject)                
                    Reset();                                    
            }
        }
    }
}