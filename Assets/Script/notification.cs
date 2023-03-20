using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class notification : MonoBehaviour
{
    private static Transform notificationObject;
    private static TMP_Text notiText;
    public static HashSet<string> remainingString = new HashSet<string>();
    private static bool isActive = false;

    void Awake() {
        notificationObject = GameObject.Find("Notification Canvas").transform.GetChild(0);
        notiText = notificationObject.GetChild(0).GetComponent<TMP_Text>();
        disable();
        
    }
    public static void add(string text) { // method to recive notifications as string
        remainingString.Add(text);
        notiText.text = text;
        notificationObject.gameObject.SetActive(true);
        Debug.Log(notificationObject.gameObject);
    }

    public static void active() {
        if (!isActive) {
            isActive = true;
            if (notification.remainingString.Count > 0) {
                notiText.text = remainingString.ElementAt(remainingString.Count - 1);
                remainingString.Remove(remainingString.ElementAt(remainingString.Count - 1));
                notificationObject.gameObject.SetActive(true);
                
            }
            isActive = false;
        }

    }

    public static void disable() { notificationObject.gameObject.SetActive(false); }

}

