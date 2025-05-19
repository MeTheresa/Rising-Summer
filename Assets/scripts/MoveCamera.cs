using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private float minOrthographicSize = 5f;
    [SerializeField] private float zoomOutFactor = 0.5f;
    [SerializeField] private float verticalBuffer = 4f;
    [SerializeField] private float fixedX = 0f;      
    [SerializeField] private float fixedZ = -10f;       

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        float minY = Mathf.Min(player1.position.y, player2.position.y);
        float maxY = Mathf.Max(player1.position.y, player2.position.y);
        float midpointY = (minY + maxY) / 2f;

        Vector3 targetPos = new Vector3(fixedX, midpointY, fixedZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        float verticalDistance = (maxY - minY) + (verticalBuffer * 2f);
        float targetSize = Mathf.Max(minOrthographicSize, verticalDistance * zoomOutFactor);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime / smoothTime);
    }
}
