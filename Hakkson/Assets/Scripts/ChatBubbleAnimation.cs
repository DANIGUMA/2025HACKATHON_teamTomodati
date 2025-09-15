using UnityEngine;
using System.Collections;

public class ChatBubbleAnimation : MonoBehaviour
{
    [Header("アニメーション設定")]
    public float animationDuration = 0.3f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        // 必要なコンポーネントを取得/追加
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初期状態を設定（非表示）
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;
    }

    void Start()
    {
        // アニメーション開始
        StartCoroutine(PlayEntranceAnimation());
    }

    // 登場アニメーション
    IEnumerator PlayEntranceAnimation()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // フェードイン
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            // スケールアニメーション
            float scaleValue = scaleCurve.Evaluate(progress);
            rectTransform.localScale = Vector3.one * scaleValue;

            yield return null;
        }

        // 最終状態を確実に設定
        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
    }

    // 退場アニメーション（将来使用）
    public IEnumerator PlayExitAnimation()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);

            yield return null;
        }
    }
}
