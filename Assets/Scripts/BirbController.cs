using System.Collections.Generic;
using UnityEngine;

public class BirbController : MonoBehaviour
{
    public enum birb
    {
        Robin,
        Blackbirb,
        Crow,
        Seagull,
        Eagle,
        Magpie
    }

    [SerializeField] private AudioSource windNoise;
    [SerializeField] private AudioSource rainNoise;
    [SerializeField] private AudioSource birbNoise;
   // [SerializeField] private AudioSource waterNoise;

    private Dictionary<birb, float> birbTimers = new Dictionary<birb, float>();
    [SerializeField] private List<BirbWrapper> birbNoises;

    public void StartAmbienceSounds(float wind, List<birb> birbs)
    {
        MakeWindBlow(wind);
        MakeBirbs(birbs);    
    }

    private void MakeWindBlow(float wind)
    {
        if (wind > 0.24f)
        {
            var adjustedWind = Mathf.InverseLerp(0.24f, 1f, wind);
            windNoise.volume = Mathf.Lerp(0f, 0.5f, adjustedWind);
            windNoise.pitch = Mathf.Lerp(0.8f, 1.5f, adjustedWind);
            windNoise.Play();
        }
    }

    private void MakeBirbs(List<birb> birbs)
    {
        foreach (birb birb in birbs)
        {
            birbTimers.Add(birb, UnityEngine.Random.Range(0, 5));
        }
        InvokeRepeating("MakeBirbNoise", 1f, 1f);
    }

    private void MakeBirbNoise()
    {
        List<birb> allKeys = new List<birb>(birbTimers.Keys);

        foreach (var key in allKeys)
        {
            birbTimers[key] -= birbNoises[(int)key].frequency;
                        
            if (birbTimers[key] < 0.1)
            {                 
                // sing birb
                PlayaDeSound(key);

                // sshhh
                birbTimers[key] =  UnityEngine.Random.Range(1, 10); ;
              //  Debug.Log("played " + key.ToString() + " at " + Time.timeSinceLevelLoad.ToString());
            }
        }
    }

    private void PlayaDeSound(birb key)
    {
        var birb = birbNoises[(int)key];
        var noise = birb.audioClips[UnityEngine.Random.Range(0, birb.audioClips.Count)];
        birbNoise.PlayOneShot(noise, UnityEngine.Random.Range(0.05f, 0.4f));
    }

    internal void StartRainSounds()
    {
        rainNoise.Play();
    }
}

[System.Serializable]
public class BirbWrapper
{
    [SerializeField] private BirbController.birb birdName;
    public float frequency;
    public List<AudioClip> audioClips;
}
