using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    public float duration = 1f;
    bool isPause = false;
    float pendingPauseDuration = 0f;
    // Update is called once per frame
    void Update()
    {
        if (pendingPauseDuration > 0 && !isPause)
        {
            StartCoroutine(DoPause());
        }
    }

    public void Pause()
    {
        pendingPauseDuration = duration;
    }

    IEnumerator DoPause()
    {
        isPause = true;

        var original = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = original;
        pendingPauseDuration = 0;
        isPause = false;
    }
}
