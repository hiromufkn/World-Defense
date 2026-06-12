using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 3f;
    public float moveRange = 3f;

    private int direction=1;
    private Vector3 StartPos;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = transform.position;
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Speed * direction * Time.deltaTime);

        if (transform.position.x > StartPos.x + moveRange)
        {
            direction = -1;
        }

        if(transform.position.x<StartPos.x-moveRange)
        {
            direction = 1;
        }

    }
}
