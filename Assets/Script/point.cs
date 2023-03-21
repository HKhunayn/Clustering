using System.Collections;
using UnityEngine;

public class point : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] private static float holdToDelete = 0.5f;
    private float lastClick = 0f;
    bool isHolding = false;
    private Vector3 lastPos;
    private bool stillNear = true;
    
    private void Start()
    {
        cam = Camera.main;
    }
    private void OnCollisionEnter2D(Collision2D c)
    {
        if ((transform.position - c.transform.position) == Vector3.zero)
            transform.position += new Vector3(0.1f, 0, 0);

    }

    private void OnMouseDrag()
    {
        Clustering.inMoving = true;
        transform.position  = cam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
    }

    private void OnMouseDown()
    {
        isHolding = true;
        lastClick = Time.time;
    }
    private void OnMouseUp()
    {
        isHolding = false;
        lastPos = transform.position;
        StartCoroutine(stoppedMoving());
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) || (Time.time - lastClick > holdToDelete && Time.time - lastClick > 0.05f && isHolding && isNear() && stillNear) ){
            Clustering.deletePoint(gameObject);
            Clustering.inMoving = false;
            Destroy(gameObject);
        }

    }
    bool isNear() {
        float f = Mathf.Pow(Mathf.Pow(Mathf.Abs(transform.position.x - lastPos.x),2) + Mathf.Pow(Mathf.Abs(transform.position.y - lastPos.y),2), 0.5f);
        Debug.Log("dis="+f);
        if (f > 0.5f)
            stillNear = false;
        return f < 0.5f;
    
    }
    IEnumerator stoppedMoving() {

        yield return new WaitForEndOfFrame();
        Clustering.inMoving = false;
        stillNear = true;
    }
}
