using UnityEngine;
using System.Collections;
public class camManger : MonoBehaviour
{
    Camera cam;
    [SerializeField] float XLimit = 256;
    [SerializeField] float YLimit = 256;
    [SerializeField] float minZoom = 5;
    [SerializeField] float maxZoom = 10;
    [SerializeField] GameObject centeringButton;
    [SerializeField] Clustering clustering;
    Vector3 initialCameraPos;
    private Vector3 lastPos;
    private float lastTimeLeftClick = 1f;
    public static Vector3 firstCamPos;
    public static bool isMovingAndLeftClick = false;
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        initialCameraPos = firstCamPos = cam.transform.position;
        lastPos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        // call changeLoc when any mouse clicked
        if (Input.GetMouseButton(2) || ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !Clustering.inMoving))
            changeLoc();
        lastPos = cam.ScreenToWorldPoint(Input.mousePosition);
        // check if the camera pos changed then showCenteringButton 
        if (initialCameraPos != cam.transform.position)
            showCenteringButton();
    }
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
            changeZoom();
        if (Input.GetMouseButtonDown(0))
            lastTimeLeftClick = Time.time;
    }

    private void LateUpdate()
    {
        if (isMovingAndLeftClick && Input.GetMouseButtonUp(0))
            StartCoroutine(setFalse());
    }
    IEnumerator setFalse() {
        yield return new WaitForFixedUpdate();
        isMovingAndLeftClick = false;
    }
    void showCenteringButton(){
        centeringButton.SetActive(true);
    }
    private void changeLoc() {
        if (Time.time - lastTimeLeftClick < 0.2f)
            return;
        if (!isMovingAndLeftClick)
            firstCamPos = cam.transform.position;
        Vector3 v = cam.transform.position;
        // move the camera pos to mouse diricetion
        cam.transform.position += (lastPos- cam.ScreenToWorldPoint(Input.mousePosition));
        v = cam.transform.position;
        cam.transform.position = new Vector3(Mathf.Clamp(v.x, -XLimit, XLimit) , Mathf.Clamp(v.y, -YLimit, YLimit) , v.z);

        bool t = v != cam.transform.position;
        if (t  && Input.GetMouseButton(0))
            isMovingAndLeftClick = true;
    }

    private void changeZoom()
    {
        cam.orthographicSize -= Input.mouseScrollDelta.y;
        cam.orthographicSize = Mathf.Max(Mathf.Min(cam.orthographicSize, maxZoom), minZoom);
    }



    public void centeringCam()
    {
        cam.transform.position  =  initialCameraPos;
        centeringButton.SetActive(false);
    }


}
