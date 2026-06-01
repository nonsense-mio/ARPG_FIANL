using System;
using System.Collections;
using Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace ARPG
{
    /// <summary>
    /// 场景系统实现 - 替代 SceneMgr (BaseManager 单例)
    /// 通过 ITickSystem 协程托管实现异步场景加载
    /// 支持黑屏淡入淡出过渡效果
    /// </summary>
    public class SceneSystem : AbstractSystem, ISceneSystem
    {
        private const float FadeOutDuration = 0.3f;
        private const float FadeInDuration = 0.5f;
        private const string PackageName = "DefaultPackage";

        private ITickSystem tickSystem;
        private CanvasGroup fadeCanvasGroup;
        private SceneHandle currentSceneHandle;

        protected override void OnInit()
        {
            tickSystem = this.GetSystem<ITickSystem>();
            CreateFadeCanvas();
        }

        private void CreateFadeCanvas()
        {
            // Canvas
            var canvasGo = new GameObject("FadeCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            GameObject.DontDestroyOnLoad(canvasGo);

            // 全屏黑色 Image + CanvasGroup
            var imageGo = new GameObject("FadeImage");
            imageGo.transform.SetParent(canvasGo.transform, false);
            var image = imageGo.AddComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = true;

            var rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            fadeCanvasGroup = imageGo.AddComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        public void LoadScene(string sceneName, Action callback = null)
        {
            SceneManager.LoadScene(sceneName);
            callback?.Invoke();
        }

        public void LoadSceneAsync(string sceneName, Action callback)
        {
            tickSystem.StartCoroutine(LoadSceneCoroutine(sceneName, callback));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, Action callback)
        {
            // 淡出 (透明→黑屏)
            yield return FadeCoroutine(0f, 1f, FadeOutDuration);

            // 释放上一个场景的 Bundle 引用
            currentSceneHandle?.Release();

            // 通过 YooAsset 加载场景
            currentSceneHandle = YooAssets.GetPackage(PackageName)
                .LoadSceneAsync(sceneName, LoadSceneMode.Single);
            yield return currentSceneHandle;

            // 黑屏期间执行回调（初始化场景：实例化玩家、配置摄像机等）
            callback?.Invoke();

            // 等待场景初始化和渲染稳定
            for (int i = 0; i < 3; i++)
                yield return null;

            // 淡入 (黑屏→透明) — 此时画面已就绪
            yield return FadeCoroutine(1f, 0f, FadeInDuration);
        }

        private IEnumerator FadeCoroutine(float from, float to, float duration)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            fadeCanvasGroup.alpha = from;
            bool fadingIn = to < from; // 1→0 为淡入

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = fadingIn ? t * t * (3f - 2f * t) : 1 - Mathf.Pow(1 - t, 3);
                fadeCanvasGroup.alpha = Mathf.Lerp(from, to, easedT);
                yield return null;
            }

            fadeCanvasGroup.alpha = to;
            if (to == 0f)
                fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}
