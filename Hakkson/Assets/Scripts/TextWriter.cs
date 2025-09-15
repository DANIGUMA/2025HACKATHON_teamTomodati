using UnityEngine;
using TMPro;
using System.Collections;

public static class TextWriter
{
    // 静的メソッド：タイピング効果付きテキスト表示
    public static IEnumerator WriteText(TMP_Text textUI, string textToWrite, float speed = 0.05f)
    {
        textUI.text = ""; // テキストをクリア

        // 1文字ずつ表示
        foreach (char character in textToWrite)
        {
            textUI.text += character;
            yield return new WaitForSeconds(speed);
        }
    }

    // 即座にテキストを表示（タイピング効果なし）
    public static void WriteTextInstant(TMP_Text textUI, string textToWrite)
    {
        textUI.text = textToWrite;
    }
}
