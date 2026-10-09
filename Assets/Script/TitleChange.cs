using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleChange : MonoBehaviour
{

    [SerializeField] private string SceneName = "StageScene";
    [SerializeField] private string SceneName1 = "TutorialScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneName);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneName1);
        }
    }
}
