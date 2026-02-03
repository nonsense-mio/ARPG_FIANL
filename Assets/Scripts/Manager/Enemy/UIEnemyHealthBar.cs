using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT
{
    /// <summary>
    /// 敌人的血条
    /// </summary>
    public class UIEnemyHealthBar : MonoBehaviour
    {
        public Slider slider;
        float timeUntilBarIsHidden = 0;
        private UIYellowBar yellowBar;
        [SerializeField] Text txtHealthNum;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            yellowBar = GetComponentInChildren<UIYellowBar>();
        }


        /// <summary>
        /// 设置UI显示
        /// </summary>
        /// <param name="health"></param>
        public void SetHealth(int health)
        {
            if (yellowBar != null)
            {
                yellowBar.gameObject.SetActive(true);
                if (health > slider.value)
                {
                    yellowBar.slider.value = health;
                    yellowBar.timer = 3f;
                }
            }
            if (txtHealthNum != null)
                txtHealthNum.text = health.ToString() + " / " + slider.maxValue.ToString();
            slider.value = health;
            timeUntilBarIsHidden = 5;

        }
        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            if (yellowBar != null)
            {
                yellowBar.SetMaxStat(maxHealth);
            }
        }

        private void Update()
        {
            if (Camera.main != null)
                transform.LookAt(transform.position + Camera.main.transform.forward);
            timeUntilBarIsHidden -= Time.deltaTime;

            if (slider != null)
            {
                if (timeUntilBarIsHidden <= 0)
                {
                    timeUntilBarIsHidden = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    if (!slider.gameObject.activeInHierarchy)
                    {
                        slider.gameObject.SetActive(true);
                    }
                }
                if (slider.value <= 0)
                {
                    //销毁血条
                    Destroy(slider.gameObject);
                }
            }

        }
    }
}

