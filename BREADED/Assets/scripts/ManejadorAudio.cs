using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManejadorAudio : MonoBehaviour
{
    public static ManejadorAudio Instance;

    public AudioSource musicaSource;
    public AudioSource sfxSource;

    [Header("Música")]
    public AudioClip musicaFondo;
    public AudioClip musicaVictoria;
    public string nombreEscenaResultados = "Resultados";

    [Header("Fade Música de Fondo en Victoria")]
    [Range(0f, 1f)] public float volumenMusicaAlGanar = 0f;
    public float duracionFadeRegresoMusica = 2.5f;

    [Header("Efectos de Interfaz")]
    public AudioClip sonidoHover;
    public AudioClip sonidoSeleccion;
    public AudioClip sonidoBoton;

    [Header("Efectos de Juego")]
    public AudioClip sonidoCrecimiento;
    public AudioClip sonidoCaidaVaso;

    public AudioClip vasoBasura;
    public AudioClip GAMEOVER;
    public AudioClip Victoria;
    public AudioClip sonidoConfeti;

    [Header("Efectos de Ingredientes")]
    public AudioClip sonidoHoverJarabes;
    public AudioClip sonidoCrema;
    public AudioClip sonidoIngredienteVolador;

    private bool estabaEnEscenaResultados = false;
    private float volumenOriginalMusica = 1f;
    private Coroutine rutinaFadeMusica;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (musicaSource != null)
            {
                volumenOriginalMusica = musicaSource.volume;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Start()
    {
        EvaluarMusicaParaEscena(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        EvaluarMusicaParaEscena(escena.name);
    }

    public void EvaluarMusicaParaEscena(string nombreEscena)
    {
        bool esResultados = nombreEscena == nombreEscenaResultados;

        if (esResultados)
        {
            estabaEnEscenaResultados = true;
            if (ReceptorCalificacionParfait.UltimoPedidoAprobado)
            {
                IniciarVictoriaConFadeMusica();
            }
        }
        else if (estabaEnEscenaResultados || musicaSource == null || musicaSource.clip == null)
        {
            estabaEnEscenaResultados = false;
            DetenerFadeMusica();
            RestaurarVolumenMusica();
            ReproducirMusicaFondo();
        }
    }

    public void IniciarVictoriaConFadeMusica()
    {
        if (musicaSource != null)
        {
            if (musicaSource.clip != musicaFondo && musicaFondo != null)
            {
                musicaSource.clip = musicaFondo;
                musicaSource.loop = true;
                if (!musicaSource.isPlaying)
                {
                    musicaSource.Play();
                }
            }

            musicaSource.volume = volumenOriginalMusica * volumenMusicaAlGanar;
        }

        if (musicaVictoria != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(musicaVictoria);
        }

        float esperaAntesDeSubir = 0f;
        if (musicaVictoria != null)
        {
            esperaAntesDeSubir = musicaVictoria.length;
        }
        else if (Victoria != null)
        {
            esperaAntesDeSubir = Victoria.length;
        }

        IniciarFadeRegresoMusica(esperaAntesDeSubir);
    }

    public void IniciarFadeRegresoMusica(float retrasoInicial = 0f)
    {
        if (musicaSource == null) return;

        DetenerFadeMusica();
        rutinaFadeMusica = StartCoroutine(RutinaSubirMusicaGradualmente(retrasoInicial));
    }

    private IEnumerator RutinaSubirMusicaGradualmente(float retrasoInicial)
    {
        if (retrasoInicial > 0f)
        {
            yield return new WaitForSeconds(retrasoInicial);
        }

        if (musicaSource != null && !musicaSource.isPlaying && musicaFondo != null)
        {
            musicaSource.clip = musicaFondo;
            musicaSource.loop = true;
            musicaSource.Play();
        }

        float volumenInicio = musicaSource != null ? musicaSource.volume : 0f;
        float tiempo = 0f;

        while (tiempo < duracionFadeRegresoMusica)
        {
            tiempo += Time.unscaledDeltaTime;
            float t = duracionFadeRegresoMusica > 0f ? Mathf.Clamp01(tiempo / duracionFadeRegresoMusica) : 1f;
            if (musicaSource != null)
            {
                musicaSource.volume = Mathf.Lerp(volumenInicio, volumenOriginalMusica, t);
            }
            yield return null;
        }

        RestaurarVolumenMusica();
        rutinaFadeMusica = null;
    }

    private void DetenerFadeMusica()
    {
        if (rutinaFadeMusica != null)
        {
            StopCoroutine(rutinaFadeMusica);
            rutinaFadeMusica = null;
        }
    }

    private void RestaurarVolumenMusica()
    {
        if (musicaSource != null)
        {
            musicaSource.volume = volumenOriginalMusica;
        }
    }

    public void ReproducirMusica(AudioClip nuevoClip, bool loop = true)
    {
        if (musicaSource == null || nuevoClip == null) return;

        if (musicaSource.clip == nuevoClip && musicaSource.isPlaying)
        {
            musicaSource.loop = loop;
            return;
        }

        musicaSource.Stop();
        musicaSource.clip = nuevoClip;
        musicaSource.loop = loop;
        musicaSource.Play();
    }

    public void ReproducirMusicaFondo()
    {
        ReproducirMusica(musicaFondo, true);
    }

    public void PlayHover() { if (sonidoHover != null && sfxSource != null) sfxSource.PlayOneShot(sonidoHover); }
    public void PlaySeleccion() { if (sonidoSeleccion != null && sfxSource != null) sfxSource.PlayOneShot(sonidoSeleccion); }
    public void PlayCrecimiento() { if (sonidoCrecimiento != null && sfxSource != null) sfxSource.PlayOneShot(sonidoCrecimiento); }
    public void PlayCaidaVaso() { if (sonidoCaidaVaso != null && sfxSource != null) sfxSource.PlayOneShot(sonidoCaidaVaso); }
    public void PlayBoton() { if (sonidoBoton != null && sfxSource != null) sfxSource.PlayOneShot(sonidoBoton); }
    public void PlayTrash() { if (vasoBasura != null && sfxSource != null) sfxSource.PlayOneShot(vasoBasura); }
    public void PlayGO() { if (GAMEOVER != null && sfxSource != null) sfxSource.PlayOneShot(GAMEOVER); }
    public void PlayVic() { if (Victoria != null && sfxSource != null) sfxSource.PlayOneShot(Victoria); }
    public void PlayConfeti() { if (sonidoConfeti != null && sfxSource != null) sfxSource.PlayOneShot(sonidoConfeti); }

    public void PlayHoverJarabes() { if (sonidoHoverJarabes != null && sfxSource != null) sfxSource.PlayOneShot(sonidoHoverJarabes); }
    public void PlayCrema() { if (sonidoCrema != null && sfxSource != null) sfxSource.PlayOneShot(sonidoCrema); }
    public void PlayIngredienteVolador() { if (sonidoIngredienteVolador != null && sfxSource != null) sfxSource.PlayOneShot(sonidoIngredienteVolador); }
}