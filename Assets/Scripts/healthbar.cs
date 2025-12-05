using UnityEngine;
using UnityEngine.UI;

//Developer: Ivan Ching

public class healthbar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    //change healthbar of both player and enemy. (percentage based)
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
}
