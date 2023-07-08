
using UnityEngine;
using UnityEngine.UI;

public class AdjustSlider : MonoBehaviour
{
    public Slider slider;

    public void SetValue()
    {
        slider.value += 10;
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Resume()
    {
        Time.timeScale = 1;
    }
}
