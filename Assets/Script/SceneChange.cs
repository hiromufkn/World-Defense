using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private float WaitTime = 3.0f;
    [SerializeField] private string SceneName = "Title Scene";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        StartCoroutine(ReturnToTitle());
    }

    private IEnumerator ReturnToTitle()
    {
        yield return new WaitForSeconds(WaitTime);

        SceneManager.LoadScene(SceneName);
    }
}
