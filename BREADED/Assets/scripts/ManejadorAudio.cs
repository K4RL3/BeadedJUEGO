using UnityEngine;

public class ManejadorAudio : MonoBehaviour
{
    public static ManejadorAudio Instance;

    public AudioSource musicaSource;
    public AudioSource sfxSource;

    public AudioClip musicaFondo;
    public AudioClip sonidoHover;
    public AudioClip sonidoSeleccion;
    public AudioClip sonidoCrecimiento;
    public AudioClip sonidoCaidaVaso;
    public AudioClip sonidoBoton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicaFondo != null)
        {
            musicaSource.clip = musicaFondo;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    public void PlayHover()
    {
        if (sonidoHover != null) sfxSource.PlayOneShot(sonidoHover);
    }

    public void PlaySeleccion()
    {
        if (sonidoSeleccion != null) sfxSource.PlayOneShot(sonidoSeleccion);
    }

    public void PlayCrecimiento()
    {
        if (sonidoCrecimiento != null) sfxSource.PlayOneShot(sonidoCrecimiento);
    }

    public void PlayCaidaVaso()
    {
        if (sonidoCaidaVaso != null) sfxSource.PlayOneShot(sonidoCaidaVaso);
    }

    public void PlayBoton()
    {
        if (sonidoBoton != null) sfxSource.PlayOneShot(sonidoBoton);
    }
}