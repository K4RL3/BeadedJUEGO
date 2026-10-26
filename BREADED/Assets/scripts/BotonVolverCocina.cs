using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonVolverCocina : MonoBehaviour
{
    public string nombreEscenaCocina = "SampleScene";

    public void VolverALaCocina()
    {
        if (ManejadorAudio.Instance != null)
            ManejadorAudio.Instance.PlayBoton();

        SceneManager.LoadScene(nombreEscenaCocina);
    }
}