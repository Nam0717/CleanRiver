using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 4f;
    private Vector3 moveDirection;
    private CarSmokeController smokeController;

    void Start()
    {
        smokeController = GetComponent<CarSmokeController>();
    }
    public void Init(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            if (smokeController != null && !smokeController.isCleaned)
            {
                CarGameManager.Instance.OnCarMissed();
            }

            Destroy(gameObject);
        }
    }
}
