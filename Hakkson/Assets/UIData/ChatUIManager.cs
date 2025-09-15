using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Immutable;

public class ChatUIManager : MonoBehaviour
{
    [Header("UI参照")]
    public TMP_InputField messageInputField;
    public Button sendButton;
    public Transform chatContent;
    public GameObject chatBubblePrefab;

    [Header("スクロール設定")]
    public ScrollRect chatScrollRect;

    [Header("吹き出し画像")]
    public Sprite playerBubbleSprite;
    public Sprite otherBubbleSprite;

    [Header("メッセージスタイル")]
    public Color playerTextColor = Color.white;
    public Color otherTextColor = Color.black;
    public Color playerBackgroundColor = new Color(0.3f, 0.6f, 1f, 1f);
    public Color otherBackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    [Header("テスト用")]
    public Button switchSenderButton;

    [Header("タイピング設定")]
    public float typingSpeed = 0.03f;

    [Header("サイズ設定")]
    public float maxBubbleWidth = 300f;
    public float minBubbleWidth = 100f;
    public float bubblePadding = 20f;

    private bool isPlayerMessage = true;

    public float OpX, MyX;
    void Start()
    {
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);

        if (messageInputField != null)
            messageInputField.onSubmit.AddListener(OnEnterPressed);

        if (switchSenderButton != null)
            switchSenderButton.onClick.AddListener(SwitchSender);

        if (chatScrollRect == null)
            chatScrollRect = GetComponentInChildren<ScrollRect>();

        Debug.Log("チャットシステム初期化完了");
    }

    public void SendMessage()
    {
        if (messageInputField == null) return;

        string message = messageInputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        CreateChatBubble(message, isPlayerMessage);

        messageInputField.text = "";
        messageInputField.ActivateInputField();
    }

    void OnEnterPressed(string message)
    {
        SendMessage();
    }

    public void SwitchSender()
    {
        isPlayerMessage = !isPlayerMessage;

        if (switchSenderButton != null)
        {
            TMP_Text buttonText = switchSenderButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = isPlayerMessage ? "相手に切替" : "自分に切替";
        }

        Debug.Log("送信者を" + (isPlayerMessage ? "自分" : "相手") + "に変更");
    }

    void CreateChatBubble(string message, bool isPlayer)
    {
        if (chatBubblePrefab == null || chatContent == null) return;

        GameObject newBubble = Instantiate(chatBubblePrefab, chatContent);
        // 1フレーム待ってからサイズ調整
        newBubble.GetComponent<ContentText>().Messege.GetComponent<RectTransform>().localPosition = isPlayer ? new Vector3(MyX, 0, 0) : new Vector3(OpX, 0, 0);
        Debug.Log(newBubble.GetComponent<ContentText>().Messege.transform.localPosition);
        if (isPlayer)
        {
            newBubble.GetComponent<ContentText>().Messege.transform.localScale =new Vector3(-1,1,1);
        }
        else
        {
            newBubble.GetComponent<ContentText>().Messege.transform.localScale = new Vector3(1, 1, 1);
        }
        StartCoroutine(SetupBubbleWithDelay(newBubble, message, isPlayer));
    }

    IEnumerator SetupBubbleWithDelay(GameObject bubble, string message, bool isPlayer)
    {
        // まずテキストを設定
        TMP_Text messageText = GetMessageText(bubble);
        if (messageText != null)
        {
            messageText.text = message;

        }

        yield return null; // 1フレーム待機

        // サイズ調整

        // スタイル設定
        SetMessageStyle(bubble, isPlayer);

        // タイピング効果（サイズ設定後）
        if (messageText != null)
        {
            StartCoroutine(TypeMessage(messageText, message));
        }

        // スクロール
        StartCoroutine(ScrollToBottom());
    }


    TMP_Text GetMessageText(GameObject bubble)
    {
        if (bubble != null)
        {
            return bubble.GetComponent<ContentText>().text;
        }
        return null;
    }

    Image GetBackgroundImage(GameObject bubble)
    {
        Transform backgroundTransform = bubble.transform.Find("Background");
        if (backgroundTransform != null)
        {
            return backgroundTransform.GetComponent<ContentText>().image;
        }
        return null;
    }

    void SetMessageStyle(GameObject bubble, bool isPlayer)
    {
        if (bubble == null) return;

        // 背景画像を設定
        Image backgroundImage = GetBackgroundImage(bubble);
        if (backgroundImage != null)
        {
            SetBackgroundImage(backgroundImage, isPlayer);
        }

        // バブル全体の配置を設定

        // テキスト色を設定
        SetTextColor(bubble, isPlayer);
    }

    void SetBackgroundImage(Image backgroundImage, bool isPlayer)
    {
        if (backgroundImage == null) return;

        if (isPlayer)
        {
                backgroundImage.type = Image.Type.Sliced;
                backgroundImage.color = playerBackgroundColor;
        }
        else
        {
                backgroundImage.type = Image.Type.Sliced;
                backgroundImage.color = otherBackgroundColor;
        }
    }

    void SetTextColor(GameObject bubble, bool isPlayer)
    {
        TMP_Text messageText = GetMessageText(bubble);
        if (messageText != null)
        {
            messageText.color = isPlayer ? playerTextColor : otherTextColor;
        }
    }

    IEnumerator TypeMessage(TMP_Text textComponent, string message)
    {
        if (textComponent == null) yield break;

        textComponent.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text += message[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator ScrollToBottom()
    {
        yield return null;

        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
