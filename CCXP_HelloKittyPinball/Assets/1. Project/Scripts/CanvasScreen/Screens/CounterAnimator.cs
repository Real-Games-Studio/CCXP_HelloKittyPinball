using System.Collections;
using TMPro;
using UnityEngine;

public class CounterAnimator : MonoBehaviour
{
    public TMP_Text texto;

    public void StartCount(int targetcount, int timer)
    {
        StartCoroutine(CountRoutine(targetcount,timer));
    }
    IEnumerator CountRoutine(int targetCount, int timers)
    {
        if (targetCount ==-1)
        {
            texto.text = "";
            yield return new WaitForEndOfFrame();
            yield break;
        }
        else
        {
            int currentCount = 0;
            float timer = 0.0f;
            while (timer<=timers)
            {
                currentCount = (int) (Mathf.Min(1.0f, timer / timers) * targetCount);
                texto.text = currentCount.ToString();
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
            }
            texto.text = targetCount.ToString();
        }
    }

}
