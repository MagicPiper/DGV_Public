//
// Attach this script to your camera in order to use depth nodes in forward rendering
//

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EnableCameraDepthInForward : MonoBehaviour
{
    [SerializeField] private bool autoSet = false;
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Set();
    }
#endif

    private void Start()
    {
        if (autoSet)
        {
            Set();
        }
    }
    public void Set()
    {
        if (GetComponent<Camera>().depthTextureMode == DepthTextureMode.None)
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        }
    }
}
