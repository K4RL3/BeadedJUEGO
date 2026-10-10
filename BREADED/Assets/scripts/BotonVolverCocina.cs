using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonVolverCocina : MonoBehaviour
{
    public string nombreEscenaCocina = "SampleScene";

    public void VolverALaCocina()
    {
        SceneManager.LoadScene(nombreEscenaCocina);
    }
}