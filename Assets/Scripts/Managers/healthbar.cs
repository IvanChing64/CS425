using UnityEngine;
using UnityEngine.UI;

//Developer: Ivan Ching

public class healthbar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider guardSlider;

    //change healthbar of both player and enemy. (percentage based)
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthSlider.value = currentValue / maxValue;
    }

    public void UpdateGuardBar(float currentValue, float maxValue)
    {
        guardSlider.value = currentValue / maxValue;
        guardSlider.gameObject.SetActive(guardSlider.value > 0);
    }
}
