using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NavegadorEstacion : MonoBehaviour
{
    [SerializeField] private Transform[] estaciones;
    public static int estacionInicial = 0;

    [Header("Textos de los Botones")]
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    MoverCamara cameraMover;
    int estacionActual = 0;

    void Start()
    {
        cameraMover = GetComponent<MoverCamara>();
        estacionActual = estacionInicial;
        cameraMover.MoverAEstacion(estaciones[estacionActual]);
        ActualizarTextos();
    }

    public void BotonAtras()
    {
        // 🎵 SONIDO DEL BOTÓN
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual > 0)
        {
            estacionActual--;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
        }
        else
        {
            SceneManager.LoadScene("EstaciónPedidos1");
        }
    }

    public void BotonAdelante()
    {
        // 🎵 SONIDO DEL BOTÓN
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual < estaciones.Length - 1)
        {
            estacionActual++;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
        }
    }

    void ActualizarTextos()
    {
        if (estacionActual == 0)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Clientes";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "Toppings";
        }
        else if (estacionActual == 1)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Armado";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "";
        }
    }
}