using UnityEngine;
using System.Collections;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private float delay = 3.0f;

    [SerializeField] private GameObject ClearUI;

    private void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if(ExplosionPrefab!=null)
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        }

        if(ClearUI!=null)
        {
            ClearUI.SetActive(true);
        }

        Destroy(gameObject);
    }
}
