using UnityEngine.UI;
using UnityEngine;

public class Logbox : MonoBehaviour
{
    private static Logbox instance;
    public static Logbox Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<Logbox>();
            }

            return instance;
        }
    }
    [SerializeField] Text logText;
    [SerializeField] Animation anim;
    [SerializeField] AudioSource source;

    public void Show(string msg)
    {
        logText.text = msg;

        if(anim.isPlaying)
        {
            anim.Stop();
        }
        anim.Play();

        if(source.isPlaying)
        {
            source.Stop();
        }
        source.Play();
    }
}
