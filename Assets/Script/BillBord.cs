using UnityEngine;

public class BillBord : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void LateUpdate()
    {
        if (Camera.main == null)
            return;

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}
