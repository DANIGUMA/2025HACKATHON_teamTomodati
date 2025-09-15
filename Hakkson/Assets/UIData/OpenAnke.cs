using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class OpenAnke : MonoBehaviour
{
    public RectTransform transform;
    public Vector3 Open, Close;
    public float Time;
    public Ease ease;
    [Button]
    public void Show()
    {
        transform.DOLocalMove(Open, Time).SetEase(ease);
    }
    [Button]
    public void Hide()
    {
        transform.DOLocalMove(Close, Time).SetEase(ease);
    }
}
