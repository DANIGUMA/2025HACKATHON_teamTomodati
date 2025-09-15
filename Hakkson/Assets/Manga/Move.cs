using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    public List<Sprite> list;
    public GameObject Pre, Now, Next;
    public Vector3 PrePos, NowPos, NextPos;
    public float Time;
    public Ease ease;

    public int _currentIndex = 0;
    private bool _isMoving = false; // 移動中はボタン入力を無視するフラグ

    private void Start()
    {
        // 初期状態を設定
        if (list.Count > 0)
        {
            if (Pre != null) Pre.GetComponent<Image>().sprite = null;
            if (Now != null) Now.GetComponent<Image>().sprite = list[0];
            if (Next != null && list.Count > 1)
            {
                Next.GetComponent<Image>().sprite = list[1];
            }
        }
    }

    [Button]
    public void NextPage()
    {
        if (_isMoving || _currentIndex >= list.Count - 1) return;
        _isMoving = true;

        // 次のページを非表示の位置に配置
        Next.transform.localPosition = NextPos;

        // NowをPreの位置へ移動
        Now.transform.DOLocalMove(PrePos, Time).SetEase(ease);

        // NextをNowの位置へ移動
        Next.transform.DOLocalMove(NowPos, Time).SetEase(ease).OnComplete(() =>
        {
            // オブジェクトと参照の入れ替え
            GameObject temp = Pre;
            Pre = Now;
            Now = Next;
            Next = temp;

            // インデックスを更新
            _currentIndex++;

            // 新しいNextに次の画像を読み込む
            if (_currentIndex + 1 < list.Count)
            {
                Next.GetComponent<Image>().sprite = list[_currentIndex + 1];
            }
            else
            {
                Next.GetComponent<Image>().sprite = null;
            }

            _isMoving = false;
        });
    }

    [Button]
    public void BackPage()
    {
        if (_isMoving || _currentIndex <= 0) return;
        _isMoving = true;

        // 戻るページを非表示の位置に配置
        Pre.transform.localPosition = PrePos;

        // NowをNextの位置へ移動
        Now.transform.DOLocalMove(NextPos, Time).SetEase(ease);

        // PreをNowの位置へ移動
        Pre.transform.DOLocalMove(NowPos, Time).SetEase(ease).OnComplete(() =>
        {
            // オブジェクトと参照の入れ替え
            GameObject temp = Next;
            Next = Now;
            Now = Pre;
            Pre = temp;

            // インデックスを更新
            _currentIndex--;

            // 新しいPreに前の画像を読み込む
            if (_currentIndex > 0)
            {
                Pre.GetComponent<Image>().sprite = list[_currentIndex - 1];
            }
            else
            {
                Pre.GetComponent<Image>().sprite = null;
            }

            _isMoving = false;
        });
    }
}