using UnityEngine;
using System.Collections;
using static Unity.VisualScripting.Member;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private float delay = 3.0f;

    [SerializeField] private GameObject ClearUI;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ExplosionSound;
    [SerializeField] private AudioClip ClearSound;

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

        PlayExplosionSound();

        if(ClearUI!=null)
        {
            if(audioSource!=null&&ClearSound!=null)
            {
                audioSource.PlayOneShot(ClearSound);
            }

            ClearUI.SetActive(true);
        }

        Destroy(gameObject);
    }

    void PlayExplosionSound()
    {
        if (ExplosionSound==null)
        {
            return;
        }
        GameObject soundObject = new GameObject("ExplosionSound");
        AudioSource source = soundObject.AddComponent<AudioSource>();

        source.clip = ExplosionSound;
        source.volume = 1.0f;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, ExplosionSound.length + 1.0f);
    }
}
