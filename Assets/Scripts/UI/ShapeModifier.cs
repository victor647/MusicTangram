using System.Collections;
using System.Collections.Generic;
using InteractiveMusicPlayer;
using UnityEngine;

public class ShapeModifier : MonoBehaviour {
    public enum ModifierType
    {
        RotateLeft, RotateRight, IncreaseSize, DecreaseSize
    }

    public ModifierType type;
    private GameObject shape;
    private MusicSwitch music;
    private bool isChangingSize;

    private void OnMouseUp()
    {
        shape = ControlPanel.currentShape;
        music = shape.GetComponent<ShapeMusic>().music;
        var info = shape.GetComponent<ShapeInfo>();
        switch (type)
        {
            case ModifierType.DecreaseSize:
                if (isChangingSize) return;
                //shrink 3 times at most
                if (shape.transform.localScale.x > 0.41f)
                {
                    info.SizeIndex--;
                    StartCoroutine(Size(-0.02f));
                }							
                break;			
            case ModifierType.IncreaseSize:
                if (isChangingSize) return;
                //expand 3 times at most
                if (shape.transform.localScale.x < 1.99f)
                {
                    info.SizeIndex++;
                    StartCoroutine(Size(0.02f));
                }
				
                break;
            case ModifierType.RotateLeft:
                info.RotationIndex--;
                StartCoroutine(Rotation(5));
                break;
            case ModifierType.RotateRight:
                info.RotationIndex++;
                StartCoroutine(Rotation(-5));
                break;
        }
    }

    private IEnumerator Size(float increment)
    {
        isChangingSize = true;
        int progress = 0;
        while (progress < 10)
        {
            progress ++;
            shape.transform.localScale += new Vector3(increment, increment, 0f);
            music.SetVolume(Mathf.Sqrt(shape.transform.localScale.x) / 1.414f);
            yield return new WaitForFixedUpdate();
        }
        isChangingSize = false;
    }

    private IEnumerator Rotation(int increment)
    {        
        int progress = 0;
        while (progress < 9)
        {
            progress ++;
            shape.transform.Rotate(0, 0, increment);
            music.SetPan(Mathf.Sin(shape.transform.eulerAngles.z * Mathf.Deg2Rad) * -0.6f);
            yield return new WaitForFixedUpdate();
        }			
    }
}