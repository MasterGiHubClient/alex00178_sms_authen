using UnityEngine.UI;
using System.Collections;
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

    public void Show(float delay, string msg)
    {
        StartCoroutine(Show_Process(delay, msg));
    }

    IEnumerator Show_Process(float sec, string msg)
    {
        yield return new WaitForSeconds(sec);
        logText.text = msg;

        if (anim.isPlaying)
        {
            anim.Stop();
        }
        anim.Play();

        if (source.isPlaying)
        {
            source.Stop();
        }
        source.Play();
    }
}
