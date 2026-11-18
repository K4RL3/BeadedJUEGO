using UnityEngine;
using TMPro;

public class NavegadorEstacion : MonoBehaviour
{
    [SerializeField] private Transform[] estaciones;
    public static int estacionInicial = 0;

    private int indiceEscenaPedidos1 = 2;

    [Header("Textos de los Botones")]
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    [Header("Botones Especiales (Solo Estación Bowls)")]
    public GameObject botonEliminar;
    public GameObject botonIrACremas;

    MoverCamara cameraMover;
    int estacionActual = 0;

    void Start()
    {
        cameraMover = GetComponent<MoverCamara>();
        estacionActual = estacionInicial;
        cameraMover.MoverAEstacion(estaciones[estacionActual]);
        ActualizarTextos();
        ActualizarBotonesEspeciales();
    }

    public void BotonAtras()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual > 0)
        {
            estacionActual--;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
            ActualizarBotonesEspeciales();
        }
        else
        {
            if (TransicionEscenasUI.Instance != null)
                TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaPedidos1);
        }
    }

    public void BotonAdelante()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual < estaciones.Length - 1)
        {
            estacionActual++;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
            ActualizarBotonesEspeciales();
        }
    }

    void ActualizarTextos()
    {
        if (estacionActual == 0)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Clientes";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "Ingredientes";
        }
        else if (estacionActual == 1)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Armado";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "Acabados";
        }
        else if (estacionActual == 2)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Ingredientes";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "Toppings";
        }
        else if (estacionActual == 3)
        {
            if (textoBotonAtras != null) textoBotonAtras.text = "Acabados";
            if (textoBotonAdelante != null) textoBotonAdelante.text = "";
        }
    }

    void ActualizarBotonesEspeciales()
    {
        bool mostrarBotones = (estacionActual == 1 || estacionActual == 2);

        if (botonEliminar != null) botonEliminar.SetActive(mostrarBotones);
        if (botonIrACremas != null) botonIrACremas.SetActive(mostrarBotones);
    }

    // --- ESTA ES LA FUNCIÓN QUE FALTABA ---
    public int ObtenerEstacionActual()
    {
        return estacionActual;
    }

    // --- FUNCIÓN PARA MOVERSE A UNA ESTACIÓN ESPECÍFICA ---
    public void MoverAEstacionEspecifica(int indice)
    {
        if (indice >= 0 && indice < estaciones.Length)
        {
            estacionActual = indice;
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
            ActualizarBotonesEspeciales();
        }
    }
}