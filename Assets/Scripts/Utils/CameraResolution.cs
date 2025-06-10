using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    void Start()
    {
         Camera.main.aspect = (float)Screen.width / Screen.height; 
    }
}
