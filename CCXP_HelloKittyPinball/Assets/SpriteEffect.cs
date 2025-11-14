using UnityEngine;

public class SpriteEffect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;


    public void TurnOnEffect()
    {
        spriteRenderer.enabled = true;

        CancelInvoke("TurnOffEffect");

        Invoke("TurnOffEffect", 1f);
    }


    public void TurnOffEffect()
    {
        spriteRenderer.enabled = false;
    }
}
