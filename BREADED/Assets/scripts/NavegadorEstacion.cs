using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Importante para usar TextMeshPro

public class NavegadorEstacion : MonoBehaviour
{
    public Transform[] estaciones;
    public string nombreEscenaAnotar = "AtendiendoClientes";


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

        ActualizarTextos(); // Actualizamos los textos al entrar
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
            ActualizarTextos(); // Actualizamos al cambiar
        }
        else
        {
            SceneManager.LoadScene("EstaciónPedidos1");
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
            ActualizarTextos(); // Actualizamos al cambiar
        }
    }

    void ActualizarTextos()
    {
        if (estacionActual == 0) // En la Mesa (Armado)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Clientes";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "Toppings";
        }
        else if (estacionActual == 1) // En Toppings
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Armado";
            if (textoBotonAdelante != null) textoBotonAdelante.text = ""; // O el texto que quieras
        }
    }
}