using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class ClearSceneController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField]private AudioClip 

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Title Scene");
        }
    }
}
