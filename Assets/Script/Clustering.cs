﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Clustering : MonoBehaviour
{
    [SerializeField]private int k=3;
    [SerializeField]private int maxIterations =10;
    [SerializeField]private float delay =0.5f;
    [SerializeField]private int maxPoints=64;
    [SerializeField]private TMP_Text textK;
    [SerializeField]private TMP_Text textIterations;
    [SerializeField]private TMP_Text textDelay;
    [SerializeField]private Camera cam;
    [SerializeField]private GameObject point;
    [SerializeField]private GameObject center;
    [SerializeField]private List<UnityEngine.Color> color;
    [SerializeField]private float maxTimeToAddPoint = 0.3f;
    [SerializeField]private GameObject startButton;
    [SerializeField]private GameObject stopButton;
    [SerializeField]private animationController animi;
    private float lastPressTime = 0;
    private static List<Transform> points = new List<Transform>();
    private static List<Cluster> clusters = new List<Cluster>();

    static public bool inMoving = false;
    private void Start()
    {
        updateText();
    }
    void LateUpdate()
    {

        if (Input.GetMouseButtonDown(0)) { 
            lastPressTime = Time.time;
        
        }
        if (Input.GetMouseButtonUp(0)) { 
            if (!inMoving && (Time.time- lastPressTime <= maxTimeToAddPoint))
                addPoint();
            lastPressTime = Time.time;
        
        }

    }

    private void addPoint() {
        if (camManger.isMovingAndLeftClick || Time.time -  camManger.lastTimeLeftClick > 0.1f) // make it frindly with camera Movement (click to add hold to move camera)
            return;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || points.Count >= maxPoints) // to not be addable with over GUI
            return;
        points.Add(Instantiate(point, cam.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0,0,10), Quaternion.identity).transform);
    }
    public static bool deletePoint(GameObject g) {
        if (isCalculating)
        {
            notification.add("you cant delete right now!");
            return false;
        }
        points.Remove(g.transform);
        return true;
    }

    public static float distance(Vector3 t1, Vector3 t2) {
        return Mathf.Pow(Mathf.Pow(t1.x - t2.x, 2) + Mathf.Pow(t1.y - t2.y, 2), 0.5f);
    }

    bool isEqual(List<Cluster> c1, List<Cluster> c2) {
        if (c1.Count != c2.Count)
            return false;
        for (int i = 0; i < c1.Count; i++) {
            bool b = isEqual(c1[i].getPoints(), c2[i].getPoints());
            if (!b)
                return b;
        }
        return true;
    
    }

    bool isEqual(List<Transform> t1, List<Transform> t2) {
        if (t1.Count != t2.Count)
            return false;
        for (int i = 0; i < t1.Count; i++) {
            if (t1[i].position != t2[i].position)
                return false;
        }
        return true;
        
    }

    bool isEqual(List<Vector3> c1, List<Vector3> c2) {
        if (c1.Count != c2.Count)
            return false;
        for (int i = 0; i < c1.Count; i++)
        {
            if (!c2.Contains(c1[i]))
                return false;
        }
        return true;

    }
    void destroyTransform(List<Cluster> c) {
        foreach (Cluster cl in c) {
            Destroy(cl.getCenter().gameObject);
            cl.getPoints().Clear();
        }
            

    }


    void changeColor(UnityEngine.Color c) {
        foreach (Transform t in points) {
            t.GetComponent<SpriteRenderer>().color = c;
        }
    }

    public void destroyAllPoints() {
        if (isCalculating) {
            notification.add("you cant delete right now!");
            return;
        }
           
        foreach (Transform t in points) { 
            Destroy (t.gameObject);
        }
        points.Clear();
        GameObject[] gg = GameObject.FindGameObjectsWithTag("center");
        foreach (GameObject g in gg)
            g.SetActive(false);

    }


    // -----------------------( for buttons controling )-----------------------
    public void increaseK()
    {
        if (k > 98)
            return;
        k++;
        updateText();
    }

    public void decreaseK()
    {
        if (k == 2)
            return;
        k--;
        updateText();
    }
    
    public void increaseIterations()
    {
        if (maxIterations > 98)
            return;
        maxIterations++;
        updateText();
    }

    public void decreaseIterations()
    {
        if (maxIterations == 2)
            return;
        maxIterations--;
        updateText();
    }
      
    public void increaseDelay()
    {
        if (delay > 9.8)
            return;
        delay+=0.1f;
        updateText();
    }

    public void decreaseDelay()
    {
        if (delay > 0.1f)
            delay -= 0.1f;
        updateText();
    }

    void updateText() {
        textK.text = k + "";
        textIterations.text = maxIterations + "";
        textDelay.text =  delay.ToString("F1");
    }

    public void github() { 
        
        Application.OpenURL("https://github.com/HKhunayn/Clustering");
    
    }

    // ------------------------------------------------------------------------

    bool isStop = false;

    public void stop() {
        isStop = true;
    }

    static bool isCalculating = false;
    public void KMean() { // if its follows the rules then call KMean and start visualizing
        if (isCalculating) {
            notification.add("already started!");
            return;
        }
        if (points.Count <= k)
        {
            notification.add("add more points");
            return;
        }
        StartCoroutine(kMean(k));
    }
    
    IEnumerator kMean(int k) {
            isCalculating = true;
            startButton.SetActive(false);
            stopButton.SetActive(true);

            if (points.Count <= k) {

                isCalculating = true;
                yield return null;
            }

            animi.Close(); // hide the leftside menu
            destroyTransform(clusters);
            clusters = new List<Cluster>();
            List<Vector3> newCenters = new List<Vector3>();
            List<int> usedIndex = new List<int>();
            for (int i = 0; i < k; i++)
            { // chooseing K points to create the clusters from
                
                int x = UnityEngine.Random.Range(0, points.Count-1);
                
                while (usedIndex.Contains(x)) // make sure that there is no duplicated point chosen as started point
                    x = UnityEngine.Random.Range(0, points.Count - 1);
                usedIndex.Add(x);
                newCenters.Add(points[x].transform.position);
                clusters.Add(new Cluster(Instantiate(center, newCenters[i], Quaternion.identity).transform, new List<Transform>(), color[(i % color.Count)]));

            }
            changeColor(UnityEngine.Color.white);
            List<Vector3> centers = new List<Vector3>();
            int iteration = 0;
            List<Transform> usedPoints = new List<Transform>(); // a list to store nearest points of each cluster
            while (!isEqual(centers, newCenters) && iteration < maxIterations && !isStop)
            {
                iteration++;
                yield return new WaitForSecondsRealtime(delay);

                foreach (Transform g in points) // determine the points to the nearest cluster
                {
                    if (usedPoints.Contains(g)) // to not add the nearest points twice
                        continue; 
                    Cluster nearest = clusters[0];
                    for (int i = 1; i < k; i++)
                    {
                        if (distance(nearest.getCenter().position, g.transform.position) > distance(clusters[i].getCenter().position, g.transform.position))
                            nearest = clusters[i];
                    }
                    nearest.addPoint(g);
                }

                centers = newCenters;
                newCenters = new List<Vector3>();
                usedPoints = new List<Transform>(); // reset the points that was the nearest to the cluster
                for (int i=0;i<clusters.Count; i++) // calculate the new centers
                {
                    Vector3 v = clusters[i].calculateCenter();

                    newCenters.Add(v);
                    Transform nearest = clusters[i].getNearestPoint();// to make sure each clusters has at least one point
                    usedPoints.Add(nearest);


                    clusters[i].clearPoints();
                    clusters[i].addPoint(nearest);
                    clusters[i].setCenterPos(v);
                ;
                }

            }
        animi.Open(); // showing the leftside menu
        isCalculating = isStop = false;
        startButton.SetActive(true);
        stopButton.SetActive(false);

    }

}

class Cluster
{
    Transform center;
    List<Transform> points;
    UnityEngine.Color color;
    Transform nearestPoint;
    public Cluster(Transform center, List<Transform> points, UnityEngine.Color color)
    {
        this.center = center;
        this.points = points;
        this.color = color;
        this.center.GetComponent<SpriteRenderer>().color = color;
    }

    public Transform getCenter() { return center; }
    public void setCenter(Transform center) { this.center = center; }
    public void setCenterPos(Vector3 v) { try { this.center.position = v; } catch (Exception e) { this.center.position = calculateCenter(); } }


    public List<Transform> getPoints() { return points; }
    public void clearPoints() { points.Clear(); nearestPoint = null; }
    public void setPoints(List<Transform> points) { this.points = points; }
    public void addPoint(Transform point) {
        if (nearestPoint == null)
            nearestPoint = point;
        else
            nearestPoint = Clustering.distance(center.position, nearestPoint.position) > Clustering.distance(center.position, point.position) ? point : nearestPoint;
        points.Add(point);
        point.GetComponent<SpriteRenderer>().color = color;
    }

    public Transform getNearestPoint() { return nearestPoint; }
    public UnityEngine.Color getColor() { 
        return color;
    }

    public Vector3 calculateCenter() {
        Vector3 total = Vector3.zero;
        foreach (Transform t in points)
        {
            total += t.position;
        }
        return total/ points.Count;
    }


    override public string ToString() {

        return "center:" + center + "\tpoints:" + points.Count + "\tcolor:" + color;
    }
}
