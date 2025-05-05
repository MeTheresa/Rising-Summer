using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.25f;
    [SerializeField] private float delayBeforeStart = 5f;

    [SerializeField]private float timer = 0f;
    private bool startMoving = false;

    void Update()
    {
        // Wait for delay time
        if (!startMoving)
        {
            timer += Time.deltaTime;
            if (timer >= delayBeforeStart)
            {
                startMoving = true;
            }
        }
        else
        {
            // Move the camera up
            transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
        }
    }
}
