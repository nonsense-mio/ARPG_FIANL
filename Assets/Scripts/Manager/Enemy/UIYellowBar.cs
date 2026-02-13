using System.Collections;
using System.Collections.Generic;
using ARPG;
using UnityEngine;
using UnityEngine.UI;

public class UIYellowBar : MonoBehaviour
{
    public Slider slider;
    UIEnemyHealthBar parentHealthBar;
    public float timer;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        parentHealthBar = GetComponentInParent<UIEnemyHealthBar>();
    }
    private void OEnable()
    {
        if(timer <= 0)
        {
            timer = 2f;
        }
    }
    private void Update()
    {
        if(timer <= 0)
        {
            if(slider.value > parentHealthBar.slider.value)
            {
                slider.value -= Time.deltaTime * 30;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void SetMaxStat(int maxStat)
    {
        slider.maxValue = maxStat;
        slider.value = maxStat;
    }

}
