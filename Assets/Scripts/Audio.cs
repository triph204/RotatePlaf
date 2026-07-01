using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio instance;
    public AudioSource rotate;
    public AudioSource run;
    public AudioSource fall;    
    public AudioSource jump;
    public AudioSource die;
    public AudioSource pass;
    public AudioSource open;





    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(AudioSource audio)
    {
        if (audio == null) return;
        audio.Stop();
        audio.Play();
    }
}
