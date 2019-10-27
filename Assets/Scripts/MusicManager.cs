using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private AudioSource introMusic;
    [SerializeField] private AudioSource postRoundMusic;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void DisableMusic()
    {
        introMusic.volume = 0f;
        postRoundMusic.volume = 0f;
    }

    public void EnableMusic()
    {
        introMusic.volume = 0.7f;
        postRoundMusic.volume = 0.7f;
    }

    public void PlayIntroMusic()
    {
        introMusic.Play();
        introMusic.loop = true;
    }

    public void PlayPostRoundMusic()
    {
        postRoundMusic.Play();
    }

    public void FadeOutMusic()
    {
        postRoundMusic.FadeOut(8f);
        introMusic.FadeOut(8f);
    }
}
