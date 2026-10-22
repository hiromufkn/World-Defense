using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;

public class TitleChange : MonoBehaviour
{

    [SerializeField] private string SceneName = "StageScene";
    [SerializeField] private string SceneName1 = "TutorialScene";

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectSE;

    private bool isLoading = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        if(isLoading)
        {
            return;
        }

        if(Keyboard.current!=null&&Keyboard.current.enterKey.wasPressedThisFrame)
        {
            StartCoroutine(LoadSceneWithSE(SceneName));
        }

        if (Keyboard.current!=null&&Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(LoadSceneWithSE(SceneName1));
        }
    }

    private IEnumerator LoadSceneWithSE(string SceneName)
    {
        isLoading = true;

        if(audioSource!=null&&selectSE!=null)
        {
            audioSource.PlayOneShot(selectSE);
            yield return new WaitForSeconds(Mathf.Min(selectSE.length,1f));
        }
        SceneManager.LoadScene(SceneName);
    }
}
