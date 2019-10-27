using UnityEngine;

namespace Assets.Scripts
{
    public class UILine : MonoBehaviour
    {
        void Start()
        {
            // Declare points in GUI space
            Vector2 p0 = new Vector2(50f, Screen.height - 50f);
            Vector2 p1 = new Vector2(Screen.width - 50f, Screen.height - 50f);            

            DrawTriangleInGUISpace(p0, p1);
        }

        // Accepts GUI points, NOT screen space
        void DrawTriangleInGUISpace(
            Vector2 p0,
            Vector2 p1
            )
        {
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            //lr.SetVertexCount(4);

            // Since there isn't a GUIToWorldPoint available at runtime, subtract y values by screen height to convert
            // from screen to GUI space.  ScreenToWorldPoint accepts a Vector3 arg, where Z is distance from camera. 
            Vector3 worldPoint0 = Camera.main.ScreenToWorldPoint(new Vector3(p0.x, Screen.height - p0.y, 10f));
            Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(new Vector3(p1.x, Screen.height - p1.y, 10f));          

            lr.SetPosition(0, worldPoint0);
            lr.SetPosition(1, worldPoint1);         
        }
    }
}