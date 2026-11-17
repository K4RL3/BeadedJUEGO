using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string nombrePrimeraEscena = "EstaciónPedidos1";

    public void BotonJugar()
    {
        if (ManejadorAudio.Instance != null)
            ManejadorAudio.Instance.PlayBoton();

        SceneManager.LoadScene(nombrePrimeraEscena);
    }

    public void BotonSalir()
    {
        if (ManejadorAudio.Instance != null)
            ManejadorAudio.Instance.PlayBoton();

        Application.Quit();
    }
}