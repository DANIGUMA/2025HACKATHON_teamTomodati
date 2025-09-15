using Sirenix.OdinInspector; // StringBuilder ���g�����߂ɕK�v
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Unity�G�f�B�^��Ő��E�V�X�e���̌v�Z�e�X�g�����s����MonoBehaviour�B
/// </summary>
public class Test
{
    [Header("�e�X�g�ݒ�")]
    [Tooltip("�e�X�g�Ɏg�p���郉���_���Ȗ���f�[�^�̐�")]
    [SerializeField] private int numberOfManga = 100;

    /// <summary>
    /// �Q�[���J�n���Ɏ����I�ɌĂяo�����Unity�̃��C�t�T�C�N�����\�b�h�B
    /// </summary>
    private async void Start()
    {
        await RunTest(numberOfManga);
    }

    /// <summary>
    /// �w�肳�ꂽ���̃f�[�^�Ńe�X�g�����s���܂��B
    /// </summary>
    [Button]
    private async Task RunTest(int testDataCount)
    {
        Debug.Log($"--- {testDataCount}���̖���f�[�^�Ńe�X�g���J�n ---");

        // 1. �e�X�g�Ώۂ�OnePersonData������
        OnePersonData personData = new();

        // 2. �w�肳�ꂽ�����������_���Ȗ���f�[�^�𐶐����Ēǉ�
        for (uint i = 0; i < (uint)testDataCount; i++)
        {
            // TestGenerator�N���X���v���W�F�N�g���ɑ��݂��邱�Ƃ��O��
            OneMangaData mangaData = TestGenerator.CreateRandomMangaData(i);
            personData.mangaDatas.Add(i, mangaData);
        }
        Debug.Log($"{personData.mangaDatas.Count}���̃����_���f�[�^�𐶐����܂����B");

        // 4. �n�D�x�N�g���̌v�Z���������s
        Debug.Log("\n�n�D�x�N�g��(LData)�̌v�Z�����s���܂�...");
        // OnePersonData.Calculations() ���v���W�F�N�g���ɑ��݂��邱�Ƃ��O��
        Debug.Log("�v�Z���������܂����B");

        // 5. ���ʂ�\��
        Debug.Log("\n--- �v�Z���� (LData) ---");
        PrintMatrix(personData.PreferenceData);
    }
    public static OnePersonData GenerateRandomData()
    {
        OnePersonData personData = new();

        // 2. �w�肳�ꂽ�����������_���Ȗ���f�[�^�𐶐����Ēǉ�
        for (uint i = 0; i < 1; i++)
        {
            // TestGenerator�N���X���v���W�F�N�g���ɑ��݂��邱�Ƃ��O��
            OneMangaData mangaData = TestGenerator.CreateRandomMangaData(i);
            personData.mangaDatas.Add(i, mangaData);
        }
        personData.personID = (uint)Random.Range(0, 10000000000);
        return personData;
    }
    /// <summary>
    /// �s���Unity�̃R���\�[���Ɍ��₷���\�����邽�߂̃w���p�[�֐��B
    /// </summary>
    private void PrintMatrix(double[,] matrix)
    {
        if (matrix == null)
        {
            Debug.LogWarning("�s��NULL�ł��B");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("Matrix Output:");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                sb.Append($"{matrix[i, j]:F4} ");
            }
            sb.AppendLine("]");
        }
        Debug.Log(sb.ToString());
    }
}
