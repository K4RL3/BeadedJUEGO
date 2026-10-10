using UnityEngine;
using UnityEngine.SceneManagement;

public class NavegadorEstacion : MonoBehaviour
{
    public Transform[] estaciones;
    public string nombreEscenaAnotar = "AtendiendoClientes";

    MoverCamara cameraMover;
    int estacionActual = 0;

    void Start()
    {
        cameraMover = GetComponent<MoverCamara>();
    }

    public void BotonAtras()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual > 0)
        {
            estacionActual--;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            Debug.Log("Atrás → Estación " + estacionActual);
        }
        else
        {
            SceneManager.LoadScene(nombreEscenaAnotar);
            Debug.Log("Atrás → Cargando escena de anotar pedido");
        }
    }

    public void BotonAdelante()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual < estaciones.Length - 1)
        {
            estacionActual++;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            Debug.Log("Adelante → Estación " + estacionActual);
        }
    }
}