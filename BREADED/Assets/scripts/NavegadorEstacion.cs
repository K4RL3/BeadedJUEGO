using UnityEngine;

public class NavegadorEstacion : MonoBehaviour
{
    [SerializeField] private Transform[] estaciones;

    private MoverCamara cameraMover;
    private int estacionActual = 0;

    void Start()
    {
        cameraMover = GetComponent<MoverCamara>();
    }

    public void BotonAtras()
    {
        if (estacionActual > 0)
        {
            estacionActual--;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            Debug.Log("Atrás → Estación " + estacionActual);
        }
    }

    public void BotonAdelante()
    {
        if (estacionActual < estaciones.Length - 1)
        {
            estacionActual++;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            Debug.Log("Adelante → Estación " + estacionActual);
        }
    }
}