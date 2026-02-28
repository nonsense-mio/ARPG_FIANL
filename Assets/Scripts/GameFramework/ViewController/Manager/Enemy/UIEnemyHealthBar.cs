using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 敌人血条（Shader 广告牌版本）
    /// 挂载在 Quad 网格上，通过 HealthBarShader 的 UV 区间实现红条+黄条效果。
    /// 广告牌朝向由顶点着色器完成，无需 C# LookAt。
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class UIEnemyHealthBar : MonoBehaviour
    {
        private static readonly int PropFill   = Shader.PropertyToID("_FillAmount");
        private static readonly int PropYellow = Shader.PropertyToID("_YellowAmount");

        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;

        private float _maxHealth;
        private float _currentFill;
        private float _yellowFill;
        private float _yellowTimer;
        private float _hideTimer;

        private const float YELLOW_HOLD_TIME  = 1f;
        private const float YELLOW_DECAY_RATE = 0.8f;
        private const float HIDE_DELAY        = 5f;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _mpb      = new MaterialPropertyBlock();
            gameObject.SetActive(false);
        }

        public void SetMaxHealth(int maxHealth)
        {
            _maxHealth   = maxHealth;
            _currentFill = 1f;
            _yellowFill  = 1f;
            Apply();
        }

        public void SetHealth(int health)
        {
            float newFill = Mathf.Clamp01(health / _maxHealth);

            if (newFill < _currentFill)
            {
                // 黄条只在比红条低时才往上拉（连击中途不重置，保持连击前的高水位）
                if (_yellowFill < _currentFill)
                    _yellowFill = _currentFill;
                _yellowTimer = YELLOW_HOLD_TIME;
            }

            _currentFill = newFill;
            _hideTimer   = HIDE_DELAY;
            gameObject.SetActive(true);
            Apply();
        }

        private void Update()
        {
            // 黄条：停留结束后向红条收缩
            if (_yellowTimer > 0)
                _yellowTimer -= Time.deltaTime;
            else if (_yellowFill > _currentFill)
            {
                _yellowFill = Mathf.Max(_currentFill, _yellowFill - YELLOW_DECAY_RATE * Time.deltaTime);
                Apply();
            }

            // 自动隐藏
            _hideTimer -= Time.deltaTime;
            if (_hideTimer <= 0)
                gameObject.SetActive(false);
        }

        private void Apply()
        {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(PropFill,   _currentFill);
            _mpb.SetFloat(PropYellow, _yellowFill);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}
