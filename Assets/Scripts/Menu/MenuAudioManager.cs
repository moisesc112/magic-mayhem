using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        // Check if the AudioSource is assigned, and use the one on this GameObject if not.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Check if the AudioSource exists and is not already playing.
        if (audioSource != null)
        {
            // Ensure the audio is set to loop
            audioSource.loop = true;

            // Only play the audio if it is not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                Debug.Log("Menu audio started and is set to loop.");
            }
            else
            {
                Debug.Log("Menu audio is already playing.");
            }
        }
        else
        {
            Debug.LogError("AudioSource is missing on the AudioManager GameObject.");
        }
    }
}
