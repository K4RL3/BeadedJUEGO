using UnityEngine;

public class NavegadorEstacion : MonoBehaviour
{
    public MoverCamara cameraMover;
    public Transform[] estaciones;

    private int estacionActual = 0;

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
