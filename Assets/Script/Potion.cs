using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private float floatHeight = 0.15f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotateSpeed = 30f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // è„â∫Ç…ïÇÇ≠
        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position =
            startPosition + new Vector3(0f, y, 0f);

        // Xé≤Ç20ìxåXÇØÇΩèÛë‘Ç≈Yé≤âÒì]
        transform.rotation =
            Quaternion.Euler(20f, Time.time * rotateSpeed, 0f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Player player = other.GetComponent<Player>();

        if (player == null)
            return;

        player.Heal(30f);

        Destroy(gameObject);
    }
}
