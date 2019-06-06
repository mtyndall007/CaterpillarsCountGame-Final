using UnityEngine;
using System.Collections;

public class MagnifyGlass : MonoBehaviour
{
    private Camera magnifyCamera;
    private GameObject magnifyBorders;
    private LineRenderer LeftBorder, RightBorder, TopBorder, BottomBorder; // Reference for lines of magnify glass borders
    private float MGOX, MG0Y; // Magnify Glass Origin X and Y position
    private float MGWidth = Screen.width / 5f, MGHeight = Screen.width / 5f; // Magnify glass width and height
    private Vector3 mousePos;
    private static int rightClickCounter;

    private static bool zoomable;

    public Material lineMaterial;

    void Start()
    {

        createMagnifyGlass();
        DontDestroyOnLoad(magnifyBorders);
        DontDestroyOnLoad(magnifyCamera);
        zoomable = true;

    }

    void Update()
    {

        if (Bug.isPaused)
        {
            zoomable = false;
        }
        if (Input.GetMouseButtonDown(1) && zoomable)
        {
            rightClickCounter++;
        }
        if (rightClickCounter%2 == 1 && zoomable)
        {
            Debug.Log("t " +zoomable);
            magnifyCamera.enabled = true;
            magnifyBorders.SetActive(true);
            // Following lines set the camera's pixelRect and camera position at mouse position
            magnifyCamera.pixelRect = new Rect(Input.mousePosition.x - MGWidth / 2.0f, Input.mousePosition.y - MGHeight / 2.0f, MGWidth, MGHeight);
            mousePos = getWorldPosition(Input.mousePosition);
            magnifyCamera.transform.position = mousePos;
            mousePos.z = 0;
            magnifyBorders.transform.position = mousePos;
        } else
        {
            magnifyCamera.enabled = false;
            magnifyBorders.SetActive(false);

        }

        //clampCamera();
        if (!mouseInBound())
        {
            magnifyCamera.enabled = false;
            magnifyBorders.SetActive(false);
        }


    }

    public bool mouseInBound()
    {
        //Debug.Log(Input.mousePosition.y + " > " + Screen.height);
        //LEFT BOUND
        if(Input.mousePosition.x < 0 +MGWidth/8)
        {
            return false;
        }
        //RIGHT BOUND
        if (Input.mousePosition.x > Screen.width - MGWidth/8)
        {
            return false;
        }
        //BOTTOM BOUND
        if (Input.mousePosition.y < 0 +MGHeight/8)
        {
            return false;
        }
        //TOP BOUND
        if (Input.mousePosition.y > Screen.height - MGHeight/8)
        {
            return false;
        }

        return true;
    }

    //public void clampCamera()
    //{
    //    Vector2 mousePosition = Input.mousePosition;
    //    Debug.Log(getWorldPosition(Input.mousePosition));

    //    if(mousePosition.x < 57.4)
    //    {
    //        magnifyCamera.pixelRect = new Rect(0, Input.mousePosition.y - MGHeight / 2.0f, MGWidth, MGHeight);
    //        mousePos = getWorldPosition(Input.mousePosition);
    //        magnifyCamera.transform.position = mousePos;
    //        mousePos.z = 0;

    //        magnifyBorders.transform.position = new Vector3(-5,mousePos.y);
    //    }

    //    if (mousePosition.y < 57.4)
    //    {
    //        //magnifyCamera.pixelRect = new Rect(0, Input.mousePosition.y - MGHeight / 2.0f, MGWidth, MGHeight);
    //        //mousePos = getWorldPosition(Input.mousePosition);
    //        //magnifyCamera.transform.position = mousePos;
    //        //mousePos.z = 0;

    //        //magnifyBorders.transform.position = new Vector3(-5, mousePos.y);
    //    }

    //}

    public static void EnableZoom()
    {
        zoomable = true;
    }

    public static void DisableZoom()
    {
        zoomable = false;
    }

    public static bool IsZoomable(){
      return zoomable;
    }

    //Useful for removing magnifying effect while the user is returning out of the bug selection UI
    public static void ResetCounter()
    {
        rightClickCounter = 0;
    }

    // Following method creates MagnifyGlass
    private void createMagnifyGlass()
    {
        GameObject camera = new GameObject("MagnifyCamera");
        MGOX = Screen.width / 2f - MGWidth / 2f;
        MG0Y = Screen.height / 2f - MGHeight / 2f;
        magnifyCamera = camera.AddComponent<Camera>();
        magnifyCamera.pixelRect = new Rect(MGOX, MG0Y, MGWidth, MGHeight);
        magnifyCamera.transform.position = new Vector3(0, 0, 0);
        if (Camera.main.orthographic)
        {
            magnifyCamera.orthographic = true;
            magnifyCamera.orthographicSize = Camera.main.orthographicSize / 9.0f;//+ 1.0f;
            createBordersForMagniyGlass();
        }
        else
        {
            magnifyCamera.orthographic = false;
            magnifyCamera.fieldOfView = Camera.main.fieldOfView / 7.0f;//3.0f;
        }

    }

    // Following method sets border of MagnifyGlass
    private void createBordersForMagniyGlass()
    {
        magnifyBorders = new GameObject();
        LeftBorder = getLine();
        LeftBorder.SetVertexCount(2);
        LeftBorder.SetPosition(0, new Vector3(getWorldPosition(new Vector3(MGOX, MG0Y, 0)).x, getWorldPosition(new Vector3(MGOX, MG0Y, 0)).y - 0.1f, -1));
        LeftBorder.SetPosition(1, new Vector3(getWorldPosition(new Vector3(MGOX, MG0Y + MGHeight, 0)).x, getWorldPosition(new Vector3(MGOX, MG0Y + MGHeight, 0)).y + 0.1f, -1));
        LeftBorder.transform.parent = magnifyBorders.transform;
        TopBorder = getLine();
        TopBorder.SetVertexCount(2);
        TopBorder.SetPosition(0, new Vector3(getWorldPosition(new Vector3(MGOX, MG0Y + MGHeight, 0)).x, getWorldPosition(new Vector3(MGOX, MG0Y + MGHeight, 0)).y, -1));
        TopBorder.SetPosition(1, new Vector3(getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y + MGHeight, 0)).x, getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y + MGHeight, 0)).y, -1));
        TopBorder.transform.parent = magnifyBorders.transform;
        RightBorder = getLine();
        RightBorder.SetVertexCount(2);
        RightBorder.SetPosition(0, new Vector3(getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y + MGWidth, 0)).x, getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y + MGWidth, 0)).y + 0.1f, -1));
        RightBorder.SetPosition(1, new Vector3(getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y, 0)).x, getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y, 0)).y - 0.1f, -1));
        RightBorder.transform.parent = magnifyBorders.transform;
        BottomBorder = getLine();
        BottomBorder.SetVertexCount(2);
        BottomBorder.SetPosition(0, new Vector3(getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y, 0)).x, getWorldPosition(new Vector3(MGOX + MGWidth, MG0Y, 0)).y, -1));
        BottomBorder.SetPosition(1, new Vector3(getWorldPosition(new Vector3(MGOX, MG0Y, 0)).x, getWorldPosition(new Vector3(MGOX, MG0Y, 0)).y, -1));
        BottomBorder.transform.parent = magnifyBorders.transform;
    }

    // Following method creates new line for MagnifyGlass's border
    private LineRenderer getLine()
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.SetVertexCount(2);
        line.SetWidth(0.2f, 0.2f);
        line.SetColors(Color.black, Color.black);
        line.useWorldSpace = false;
        return line;
    }
    private void setLine(LineRenderer line)
    {
        line.material = new Material(Shader.Find("Diffuse"));
        line.SetVertexCount(2);
        line.SetWidth(0.2f, 0.2f);
        line.SetColors(Color.black, Color.black);
        line.useWorldSpace = false;
    }

    // Following method calculates world's point from screen point as per camera's projection type
    public Vector3 getWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos;
        if (Camera.main.orthographic)
        {
            worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = Camera.main.transform.position.z;
        }
        else
        {
            worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.transform.position.z));
            worldPos.x *= -1;
            worldPos.y *= -1;
        }
        return worldPos;
    }
}
