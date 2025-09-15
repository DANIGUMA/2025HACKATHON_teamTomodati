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
    private bool _isMoving = false; // �ړ����̓{�^�����͂𖳎�����t���O

    private void Start()
    {
        // ������Ԃ�ݒ�
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

        // ���̃y�[�W���\���̈ʒu�ɔz�u
        Next.transform.localPosition = NextPos;

        // Now��Pre�̈ʒu�ֈړ�
        Now.transform.DOLocalMove(PrePos, Time).SetEase(ease);

        // Next��Now�̈ʒu�ֈړ�
        Next.transform.DOLocalMove(NowPos, Time).SetEase(ease).OnComplete(() =>
        {
            // �I�u�W�F�N�g�ƎQ�Ƃ̓���ւ�
            GameObject temp = Pre;
            Pre = Now;
            Now = Next;
            Next = temp;

            // �C���f�b�N�X���X�V
            _currentIndex++;

            // �V����Next�Ɏ��̉摜��ǂݍ���
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

        // �߂�y�[�W���\���̈ʒu�ɔz�u
        Pre.transform.localPosition = PrePos;

        // Now��Next�̈ʒu�ֈړ�
        Now.transform.DOLocalMove(NextPos, Time).SetEase(ease);

        // Pre��Now�̈ʒu�ֈړ�
        Pre.transform.DOLocalMove(NowPos, Time).SetEase(ease).OnComplete(() =>
        {
            // �I�u�W�F�N�g�ƎQ�Ƃ̓���ւ�
            GameObject temp = Next;
            Next = Now;
            Now = Pre;
            Pre = temp;

            // �C���f�b�N�X���X�V
            _currentIndex--;

            // �V����Pre�ɑO�̉摜��ǂݍ���
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