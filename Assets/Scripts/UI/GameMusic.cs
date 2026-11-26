using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip[] tracks = new AudioClip[4];
    public bool musicEnabled, byAct = true;
    public int act;
    private int currentTrack = -1, neededTrack;
    private float trackTime;

    private void Update()
    {
        if (musicEnabled)
        {
            musicSource.loop = byAct;
            if (byAct)
            {
                neededTrack = Mathf.Clamp(act - 1, 0, tracks.Length - 1);
                if (currentTrack != neededTrack)
                {
                    trackTime = 0;
                    if (currentTrack >= 0)
                    {
                        trackTime = musicSource.time;
                    }
                    currentTrack = neededTrack;
                    musicSource.clip = tracks[currentTrack];
                    musicSource.time = trackTime % musicSource.clip.length;
                    musicSource.Play();
                }
            }
            else if (!musicSource.isPlaying)
            {
                currentTrack = (currentTrack + 1) % tracks.Length;
                musicSource.clip = tracks[currentTrack];
                musicSource.Play();
            }
        }
        else if (musicSource.isPlaying)
        {
            musicSource.Stop();
            currentTrack = -1;
        }
    }
}
