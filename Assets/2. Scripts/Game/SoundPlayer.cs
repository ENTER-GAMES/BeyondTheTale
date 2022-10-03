using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public void PlaySound(AudioClip clip)
    {
        GameObject go = new GameObject($"SFX {clip.name}");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        Destroy(go, clip.length);
    }
}
