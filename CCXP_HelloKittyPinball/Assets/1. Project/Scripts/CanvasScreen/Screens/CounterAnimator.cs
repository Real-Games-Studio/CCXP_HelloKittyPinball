using System.Collections;
using TMPro;
using UnityEngine;

public class CounterAnimator : MonoBehaviour
{
    public TMP_Text texto;
    public float CountTime = 2.0f;

    public void StartCount(int targetcount)
    {
        StartCoroutine(CountRoutine(targetcount));
    }
    IEnumerator CountRoutine(int targetCount)
    {
        int currentCount = 0;
        float timer = 0.0f;
        while (timer<=CountTime)
        {
            currentCount = (int) (Mathf.Min(1.0f, timer / CountTime) * targetCount);
            texto.text = currentCount.ToString();
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }

}
