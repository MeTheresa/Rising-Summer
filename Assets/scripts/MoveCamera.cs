using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private float fixedX = 0f;
    [SerializeField] private float fixedZ = -10f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        float highestY = Mathf.Max(player1.position.y, player2.position.y);

        float targetY = Mathf.Max(highestY, transform.position.y);

        Vector3 targetPos = new Vector3(fixedX, targetY, fixedZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
