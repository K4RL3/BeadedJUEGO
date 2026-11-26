using UnityEngine;

public class ControladorResultados : MonoBehaviour
{
    private const string NOMBRE_CANVAS_VICTORIA = "VICTORIA";
    private const string NOMBRE_CANVAS_GAMEOVER = "GAMEOVER";

    [Header("Canvas de Resultado (Opcional si se llaman VICTORIA y GAMEOVER)")]
    public GameObject canvasVictoria;
    public GameObject canvasGameOver;

    [Header("Audio de Victoria")]
    public bool reproducirSonidoVictoriaAlEntrar = true;

    private void Start()
    {
        ResolverCanvasSiFaltan();
        AplicarResultado();
    }

    private void ResolverCanvasSiFaltan()
    {
        if (canvasVictoria == null)
        {
            GameObject encontradoVictoria = GameObject.Find(NOMBRE_CANVAS_VICTORIA);
            if (encontradoVictoria != null)
            {
                canvasVictoria = encontradoVictoria;
            }
        }

        if (canvasGameOver == null)
        {
            GameObject encontradoGameOver = GameObject.Find(NOMBRE_CANVAS_GAMEOVER);
            if (encontradoGameOver != null)
            {
                canvasGameOver = encontradoGameOver;
            }
        }
    }

    public void AplicarResultado()
    {
        bool aprobado = ReceptorCalificacionParfait.UltimoPedidoAprobado;

        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(aprobado);
        }
        else
        {
            Debug.LogWarning($"[Resultados] No se encontró el Canvas '{NOMBRE_CANVAS_VICTORIA}'.");
        }

        if (canvasGameOver != null)
        {
            canvasGameOver.SetActive(!aprobado);
        }
        else
        {
            Debug.LogWarning($"[Resultados] No se encontró el Canvas '{NOMBRE_CANVAS_GAMEOVER}'.");
        }

        if (aprobado && reproducirSonidoVictoriaAlEntrar && ManejadorAudio.Instance != null)
        {
            ManejadorAudio.Instance.PlayVic();
        }
    }
}