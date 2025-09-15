using Sirenix.OdinInspector;
using UnityEngine;

public class SizeEdit : MonoBehaviour
{
    public RectTransform Text;
    RectTransform RectTransform;
    public Vector2 Size = new();
    public Vector2 Space = new();
    public void Start()
    {
        RectTransform = GetComponent<RectTransform>();
    }
    public void Update()
    {
        RectTransform.sizeDelta = (Text.sizeDelta * Size) + Space;
    }
    [Button]
    public void Up()
    {
        RectTransform = GetComponent<RectTransform>();
        RectTransform.sizeDelta = (Text.sizeDelta * Size) + Space;
    }
}
