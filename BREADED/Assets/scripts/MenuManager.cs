using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private int indiceEscenaCinematicas = 1; // Según tu Build Settings
    private int indiceEscenaJuego = 2;       // EstaciónPedidos1[cite: 28]

    [Header("Canvas de Pantalla de Inicio")]
    public GameObject canvasInicio;
    public GameObject canvasCreditos;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Cinematicas")
        {
            StartCoroutine(TemporizadorCinematica());
        }
        else
        {
            // Asegura que al iniciar siempre se vea el menú principal y no los créditos
            if (canvasInicio != null) canvasInicio.SetActive(true);
            if (canvasCreditos != null) canvasCreditos.SetActive(false);
        }
    }

    IEnumerator TemporizadorCinematica()
    {
        yield return new WaitForSeconds(17f);
        IrAlJuego();
    }

    public void BotonJugar()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        // Usa la transición hacia Cinemáticas con el efecto configurado[cite: 28]
        if (TransicionEscenasUI.Instance != null)
        {
            TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaCinematicas);
        }
        else
        {
            SceneManager.LoadScene(indiceEscenaCinematicas);
        }
    }

    public void BotonCreditos()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (canvasInicio != null) canvasInicio.SetActive(false);
        if (canvasCreditos != null) canvasCreditos.SetActive(true);
    }

    public void BotonVolverDeCreditos()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (canvasCreditos != null) canvasCreditos.SetActive(false);
        if (canvasInicio != null) canvasInicio.SetActive(true);
    }

    public void BotonSaltarCinematica()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        StopAllCoroutines();
        IrAlJuego();
    }

    void IrAlJuego()
    {
        // Usa la transición hacia el Juego[cite: 28]
        if (TransicionEscenasUI.Instance != null)
        {
            TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaJuego);
        }
        else
        {
            SceneManager.LoadScene(indiceEscenaJuego);
        }
    }
}