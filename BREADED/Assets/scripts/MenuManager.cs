using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Al quitarle "public", ya no aparece en el Inspector.
    // Los nombres de las escenas ahora están fijos aquí en el código.
    private string nombrePrimeraEscena = "Cinematicas";
    private string escenaJuego = "EstaciónPedidos1";

    void Start()
    {
        // Si estamos en la escena de Cinematicas, iniciamos el temporizador automático
        if (SceneManager.GetActiveScene().name == "Cinematicas")
        {
            StartCoroutine(TemporizadorCinematica());
        }
    }

    IEnumerator TemporizadorCinematica()
    {
        yield return new WaitForSeconds(5f); // Espera los 5 segundos
        IrAlJuego(); // Luego pasa al juego
    }

    public void BotonJugar()
    {
        Debug.Log("Cargando: " + nombrePrimeraEscena);
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        SceneManager.LoadScene(nombrePrimeraEscena);
    }

    public void BotonSaltarCinematica()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        StopAllCoroutines(); // Detiene el temporizador de 5 segundos
        IrAlJuego();         // Va al juego de inmediato
    }

    void IrAlJuego()
    {
        SceneManager.LoadScene(escenaJuego);
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