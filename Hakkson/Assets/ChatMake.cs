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

            // Width��V�����l�i��: 200�j�ɕύX
            sizeDelta.x = MainSize.x;
            sizeDelta.y = MainSize.y * (int)(messege.Length / LineWords);
            // �ύX�����T�C�Y��Rect Transform�ɓK�p
            rect.sizeDelta = sizeDelta;
        }
    }
}
