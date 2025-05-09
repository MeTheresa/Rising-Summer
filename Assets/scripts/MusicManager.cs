using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            audioSource.mute = !audioSource.mute;
        }
    }
}
