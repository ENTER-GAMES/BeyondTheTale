using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioPrefab;

    public void PlaySound(AudioClip clip)
    {
        AudioSource audioSource = Instantiate(audioPrefab);
        audioSource.name = $"SFX {clip.name}";
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        Destroy(audioSource, clip.length);
    }
}
