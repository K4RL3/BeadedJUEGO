using UnityEngine;
using TMPro;

public class NavegadorEstacion : MonoBehaviour
{
    public enum NombreEstacion
    {
        Vasos = 0,
        Fruta = 1,
        Crema = 2,
        Toppings = 3,
        Extra = 4 // Renombra esta según la estación nueva que agregaste
    }

    [Tooltip("Orden de estaciones en escena (debe coincidir con NombreEstacion)")]
    [SerializeField] private Transform[] estaciones;
    public static int estacionInicial = 0;

    private int indiceEscenaPedidos1 = 2;

    [Header("Textos de los Botones")]
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    [Header("Botones Especiales")]
    [Tooltip("Índices de estaciones donde aparecen los botones especiales")]
    public int[] estacionesConBotonesEspeciales = { (int)NombreEstacion.Fruta, (int)NombreEstacion.Crema };
    public GameObject botonEliminar;
    public GameObject botonIrACremas;

    MoverCamara cameraMover;
    int estacionActual = 0;

    void Start()
    {
        cameraMover = GetComponent<MoverCamara>();
        estacionActual = Mathf.Clamp(estacionInicial, 0, Mathf.Max(0, estaciones.Length - 1));
        if (estaciones.Length > 0 && cameraMover != null)
        {
            cameraMover.MoverAEstacion(estaciones[estacionActual]);
        }
        ActualizarTextos();
        ActualizarBotonesEspeciales();
    }

    public void BotonAtras()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        if (estacionActual > 0)
        {
            estacionActual--;
            if (cameraMover != null) cameraMover.MoverAEstacion(estaciones[estacionActual]);
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
            if (cameraMover != null) cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
            ActualizarBotonesEspeciales();
        }
        else
        {
            // En la última estación, regresa a los clientes
            if (TransicionEscenasUI.Instance != null)
                TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaPedidos1);
        }
    }

    private string ObtenerEtiquetaEstacion(int indice)
    {
        if (System.Enum.IsDefined(typeof(NombreEstacion), indice))
        {
            NombreEstacion est = (NombreEstacion)indice;
            return est == NombreEstacion.Extra ? "Extra" : est.ToString();
        }
        return "Estación " + indice;
    }

    void ActualizarTextos()
    {
        int ultimoIndice = estaciones.Length - 1;

        if (textoBotonAtras != null)
        {
            textoBotonAtras.text = estacionActual > 0
                ? ObtenerEtiquetaEstacion(estacionActual - 1)
                : "Clientes";
        }

        if (textoBotonAdelante != null)
        {
            textoBotonAdelante.text = estacionActual < ultimoIndice
                ? ObtenerEtiquetaEstacion(estacionActual + 1)
                : "Clientes";
        }
    }

    void ActualizarBotonesEspeciales()
    {
        bool mostrarBotones = false;
        if (estacionesConBotonesEspeciales != null)
        {
            for (int i = 0; i < estacionesConBotonesEspeciales.Length; i++)
            {
                if (estacionesConBotonesEspeciales[i] == estacionActual)
                {
                    mostrarBotones = true;
                    break;
                }
            }
        }

        if (botonEliminar != null) botonEliminar.SetActive(mostrarBotones);
        if (botonIrACremas != null) botonIrACremas.SetActive(mostrarBotones);
    }

    public int ObtenerEstacionActual()
    {
        return estacionActual;
    }

    public NombreEstacion ObtenerNombreEstacionActual()
    {
        return (NombreEstacion)estacionActual;
    }

    public void MoverAEstacionEspecifica(int indice)
    {
        if (indice >= 0 && indice < estaciones.Length)
        {
            estacionActual = indice;
            if (cameraMover != null) cameraMover.MoverAEstacion(estaciones[estacionActual]);
            ActualizarTextos();
            ActualizarBotonesEspeciales();
        }
    }
}