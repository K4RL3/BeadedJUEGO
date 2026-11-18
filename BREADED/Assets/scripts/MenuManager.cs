using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private int indiceEscenaCinematicas = 1; // Según tu Build Settings
    private int indiceEscenaJuego = 2;       // EstaciónPedidos1

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Cinematicas")
        {
            StartCoroutine(TemporizadorCinematica());
        }
    }

    IEnumerator TemporizadorCinematica()
    {
        yield return new WaitForSeconds(5f);
        IrAlJuego();
    }

    public void BotonJugar()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        // Usa la transición hacia Cinemáticas con el efecto configurado
        if (TransicionEscenasUI.Instance != null)
        {
            TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaCinematicas);
        }
        else
        {
            SceneManager.LoadScene(indiceEscenaCinematicas);
        }
    }

    public void BotonSaltarCinematica()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        StopAllCoroutines();
        IrAlJuego();
    }

    void IrAlJuego()
    {
        // Usa la transición hacia el Juego
        if (TransicionEscenasUI.Instance != null)
        {
            TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaJuego);
        }
        else
        {
            SceneManager.LoadScene(indiceEscenaJuego);
        }
    }

    public void BotonSalir()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}