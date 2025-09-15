using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Internal;
using System.Drawing;
using UnityEngine;

public class ChatMake : MonoBehaviour
{
    public GameObject ChatPre;
    public int LineWords;
    public Vector2 MainSize;
    public Transform Content;
    [Button]
    public void ShowChat(string[] messeges)
    {
        foreach (string messege in messeges)
        {
            GameObject chat = Instantiate(ChatPre,Content);
            RectTransform rect = chat.GetComponent<RectTransform>();
            Vector2 sizeDelta = rect.sizeDelta;

            // Widthを新しい値（例: 200）に変更
            sizeDelta.x = MainSize.x;
            sizeDelta.y = MainSize.y * (int)(messege.Length / LineWords);
            // 変更したサイズをRect Transformに適用
            rect.sizeDelta = sizeDelta;
        }
    }
}
