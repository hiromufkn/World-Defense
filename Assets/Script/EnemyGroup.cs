using UnityEngine;

public class EnemyGroup : MonoBehaviour
{

    public GameObject nextGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount==0)
        {
            if (nextGroup != null)
            {
                Instantiate(
                    nextGroup,
                    transform.position,
                    Quaternion.identity

                    );

                Destroy(gameObject);
            }
        }
    }
}
