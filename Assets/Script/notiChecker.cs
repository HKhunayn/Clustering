
using System.Collections;
using UnityEngine;
using TMPro;


class notiChecker : MonoBehaviour
{
    float lastTime = -10;
    private void Start()
    {
        notification.disable();
        StartCoroutine(check());

    }

    IEnumerator check()
    {

        while (true)
        {

            yield return new WaitForSeconds(0.5f);
            if (notification.remainingString.Count > 0 && (Time.time - lastTime) > 3f)
            {
                notification.active();
                lastTime = Time.time;
            }
            if (notification.remainingString.Count == 0 && (Time.time - lastTime) > 3f)
                notification.disable();
        }

    }


}