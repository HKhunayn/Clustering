
using System.Collections;
using UnityEngine;


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

        while (true) // check if there is notification to show it
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