using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearChangeScene : MonoBehaviour
{
    private bool bossSpawned = false;
    private bool clearStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
     if(clearStarted)
        {
            return;
        }

        Boss boss = FindFirstObjectByType<Boss>();

        if(boss!=null)
        {
            bossSpawned = true;
        }

     if(bossSpawned&&boss==null)
        {
            clearStarted = true;
            StartCoroutine(GoToClearScene());
        }
    }

    System.Collections.IEnumerator GoToClearScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("ClearScene");
    }
}
