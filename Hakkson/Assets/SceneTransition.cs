using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public List<Sprite> Kowai = new List<Sprite>();
    public void LoadNextScene(string Name)
    {
        // ���̃V�[����ǂݍ���
        // �V�[�����܂��̓r���h�ݒ�̃C���f�b�N�X���w�肵�܂�
        
        SceneManager.LoadScene(Name);
    }
    public void ShowManaga(string Name)
    {
        SetManaga();
        SceneManager.LoadScene("SampleScene");

    }
    public void SetManaga()
    {
        Datas.MangaImages = Kowai;
    }
}
