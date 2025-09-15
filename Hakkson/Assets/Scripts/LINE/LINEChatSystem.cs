using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BasicLINEChat : MonoBehaviour
{
    // UI要素
    private Canvas canvas;
    private GameObject mainPanel;
    private ScrollRect scrollView;
    private Transform messageContainer;
    private InputField inputField;
    private Button sendButton;

    // 色設定
    private Color myMessageColor = new Color(0.2f, 0.6f, 1f, 1f); // 青色
    private Color otherMessageColor = Color.white; // 白色
    private Color backgroundColor = new Color(0.95f, 0.95f, 0.97f, 1f); // 薄いグレー

    void Start()
    {
        CreateUI();
        AddSampleMessages();
    }

    void CreateUI()
    {
        // Canvas作成
        CreateCanvas();

        // メインパネル作成
        CreateMainPanel();

        // メッセージエリア作成
        CreateMessageArea();

        // 入力エリア作成
        CreateInputArea();
    }

    void CreateCanvas()
    {
        // 既存のCanvasがあるかチェック
        canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);

            canvasObj.AddComponent<GraphicRaycaster>();
        }
    }

    void CreateMainPanel()
    {
        mainPanel = new GameObject("MainPanel");
        mainPanel.transform.SetParent(canvas.transform, false);

        Image panelImage = mainPanel.AddComponent<Image>();
        panelImage.color = backgroundColor;

        RectTransform panelRect = mainPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
    }

    void CreateMessageArea()
    {
        // ScrollView作成
        GameObject scrollViewObj = new GameObject("ScrollView");
        scrollViewObj.transform.SetParent(mainPanel.transform, false);

        scrollView = scrollViewObj.AddComponent<ScrollRect>();
        scrollViewObj.AddComponent<Image>().color = Color.clear;

        RectTransform scrollRect = scrollViewObj.GetComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(0, 80); // 下部に入力エリア用スペース
        scrollRect.offsetMax = Vector2.zero;

        // Content作成
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollViewObj.transform, false);

        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);

        // Layout Group追加
        VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;

        // Content Size Fitter追加
        ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollView.content = contentRect;
        scrollView.horizontal = false;
        scrollView.vertical = true;

        messageContainer = contentObj.transform;
    }

    void CreateInputArea()
    {
        // 入力エリア背景
        GameObject inputArea = new GameObject("InputArea");
        inputArea.transform.SetParent(mainPanel.transform, false);

        Image inputAreaBG = inputArea.AddComponent<Image>();
        inputAreaBG.color = Color.white;

        RectTransform inputAreaRect = inputArea.GetComponent<RectTransform>();
        inputAreaRect.anchorMin = new Vector2(0, 0);
        inputAreaRect.anchorMax = new Vector2(1, 0);
        inputAreaRect.pivot = new Vector2(0.5f, 0);
        inputAreaRect.sizeDelta = new Vector2(0, 80);

        // 入力フィールド作成
        GameObject inputFieldObj = new GameObject("InputField");
        inputFieldObj.transform.SetParent(inputArea.transform, false);

        inputField = inputFieldObj.AddComponent<InputField>();
        Image inputFieldBG = inputFieldObj.AddComponent<Image>();
        inputFieldBG.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        RectTransform inputFieldRect = inputFieldObj.GetComponent<RectTransform>();
        inputFieldRect.anchorMin = new Vector2(0, 0.5f);
        inputFieldRect.anchorMax = new Vector2(1, 0.5f);
        inputFieldRect.pivot = new Vector2(0.5f, 0.5f);
        inputFieldRect.sizeDelta = new Vector2(-100, 50);
        inputFieldRect.anchoredPosition = new Vector2(-25, 0);

        // 入力フィールドのテキスト
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputFieldObj.transform, false);
        Text inputText = textObj.AddComponent<Text>();
        inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputText.fontSize = 16;
        inputText.color = Color.black;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);

        inputField.textComponent = inputText;

        // 送信ボタン作成
        GameObject sendButtonObj = new GameObject("SendButton");
        sendButtonObj.transform.SetParent(inputArea.transform, false);

        sendButton = sendButtonObj.AddComponent<Button>();
        Image buttonBG = sendButtonObj.AddComponent<Image>();
        buttonBG.color = myMessageColor;

        RectTransform buttonRect = sendButtonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 0.5f);
        buttonRect.anchorMax = new Vector2(1, 0.5f);
        buttonRect.pivot = new Vector2(1, 0.5f);
        buttonRect.sizeDelta = new Vector2(70, 50);
        buttonRect.anchoredPosition = new Vector2(-10, 0);

        // ボタンのテキスト
        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(sendButtonObj.transform, false);
        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.text = "送信";
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        // イベント設定
        sendButton.onClick.AddListener(SendMessage);
        inputField.onEndEdit.AddListener(delegate {
            if (Input.GetKeyDown(KeyCode.Return)) SendMessage();
        });
    }

    void SendMessage()
    {
        string message = inputField.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            CreateMessageBubble(message, true);
            inputField.text = "";
            inputField.ActivateInputField();
            ScrollToBottom();
        }
    }

    void CreateMessageBubble(string messageText, bool isMyMessage)
    {
        GameObject messageObj = new GameObject("MessageBubble");
        messageObj.transform.SetParent(messageContainer, false);

        Image messageBG = messageObj.AddComponent<Image>();
        messageBG.color = isMyMessage ? myMessageColor : otherMessageColor;

        // メッセージテキスト
        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(messageObj.transform, false);
        Text messageTextComp = textObj.AddComponent<Text>();
        messageTextComp.text = messageText;
        messageTextComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        messageTextComp.fontSize = 16;
        messageTextComp.color = isMyMessage ? Color.white : Color.black;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(15, 10);
        textRect.offsetMax = new Vector2(-15, -10);

        // サイズ調整
        ContentSizeFitter msgFitter = messageObj.AddComponent<ContentSizeFitter>();
        msgFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        msgFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ContentSizeFitter textFitter = textObj.AddComponent<ContentSizeFitter>();
        textFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 位置設定（自分のメッセージは右寄せ）
        RectTransform messageRect = messageObj.GetComponent<RectTransform>();
        if (isMyMessage)
        {
            messageRect.anchorMin = new Vector2(1, 0);
            messageRect.anchorMax = new Vector2(1, 0);
            messageRect.pivot = new Vector2(1, 0);
        }
        else
        {
            messageRect.anchorMin = new Vector2(0, 0);
            messageRect.anchorMax = new Vector2(0, 0);
            messageRect.pivot = new Vector2(0, 0);
        }
    }

    void AddSampleMessages()
    {
        CreateMessageBubble("こんにちは！", false);
        CreateMessageBubble("元気にしてる？", false);
        CreateMessageBubble("こんにちは！元気だよ", true);
        CreateMessageBubble("それは良かった", false);

        // 少し遅れてスクロール
        Invoke("ScrollToBottom", 0.2f);
    }

    void ScrollToBottom()
    {
        if (scrollView != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }
    }
}
