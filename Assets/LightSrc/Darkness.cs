using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{
    public static Darkness settings;
    SpriteRenderer darkness;
    private void Awake()
    {
        settings = this;
    }

    public void SetDarkness(float alpha)
    {
        if (darkness == null)
        {
            darkness = GetComponent<SpriteRenderer>();
        }
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        darkness.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(0, 0, 0, alpha));
        darkness.SetPropertyBlock(mpb);
    }
    public void StartBrightening(float sec)
    {
        StartCoroutine(BrightenForSeconds(sec));

    }
    IEnumerator BrightenForSeconds(float sec)
    {
        float waitSec = sec / 20;
        float darknessVal = 0;
        settings.SetDarkness(darknessVal);

        while (darknessVal <= 0.95f)
        {
            yield return new WaitForSeconds(waitSec);
            darknessVal += 0.05f;
            settings.SetDarkness(darknessVal);
        }
        settings.SetDarkness(0.99f);
    }
}