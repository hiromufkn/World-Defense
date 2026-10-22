using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class ClearSceneController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectSE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current!=null&&Keyboard.current.enterKey.wasPressedThisFrame)
        {
            StartCoroutine(LoadSceneWithSE());
        }
    }

    private IEnumerator LoadSceneWithSE()
    {
        if(audioSource!=null&&selectSE!=null)
        {
            audioSource.PlayOneShot(selectSE);
            yield return new WaitForSeconds(Mathf.Min(selectSE.length, 1f));
        }

        SceneManager.LoadScene("Title Scene");
    }
}
