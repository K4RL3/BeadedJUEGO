using UnityEngine;

public class ManejadorAudio : MonoBehaviour
{
    public static ManejadorAudio Instance;

    public AudioSource musicaSource;
    public AudioSource sfxSource;

    [Header("Música")]
    public AudioClip musicaFondo;

    [Header("Efectos de Interfaz")]
    public AudioClip sonidoHover;
    public AudioClip sonidoSeleccion;
    public AudioClip sonidoBoton;

    [Header("Efectos de Juego")]
    public AudioClip sonidoCrecimiento;
    public AudioClip sonidoCaidaVaso;

    // --- NUEVOS SONIDOS ---
    [Header("Efectos de Ingredientes")]
    public AudioClip sonidoHoverJarabes;    // Para botellas/sifones
    public AudioClip sonidoCaidaFruta;      // Cuando cae la fruta al vaso

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

    // --- TUS FUNCIONES EXISTENTES ---
    public void PlayHover() { if (sonidoHover != null) sfxSource.PlayOneShot(sonidoHover); }
    public void PlaySeleccion() { if (sonidoSeleccion != null) sfxSource.PlayOneShot(sonidoSeleccion); }
    public void PlayCrecimiento() { if (sonidoCrecimiento != null) sfxSource.PlayOneShot(sonidoCrecimiento); }
    public void PlayCaidaVaso() { if (sonidoCaidaVaso != null) sfxSource.PlayOneShot(sonidoCaidaVaso); }
    public void PlayBoton() { if (sonidoBoton != null) sfxSource.PlayOneShot(sonidoBoton); }

    // --- NUEVAS FUNCIONES ---
    public void PlayHoverJarabes()
    {
        if (sonidoHoverJarabes != null) sfxSource.PlayOneShot(sonidoHoverJarabes);
    }

    public void PlayCaidaFruta()
    {
        if (sonidoCaidaFruta != null) sfxSource.PlayOneShot(sonidoCaidaFruta);
    }
}