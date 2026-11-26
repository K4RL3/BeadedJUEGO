using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NavegadorEstacion : MonoBehaviour
{
    public enum NombreEstacion
    {
        Vasos = 0,
        Fruta = 1,
        Crema = 2,
        Toppings = 3,
        Extra = 4
    }

    private const float EscalaCalificacion = 2.31f;

    [SerializeField] private Transform[] estaciones;
    public static int estacionInicial = 0;

    private int indiceEscenaPedidos1 = 2;
    public int indiceEscenaCalificacion = 5;
    public string nombreDestinoCalificacion = "VasoCalificacion";

    [Header("Botones y Textos de Navegación")]
    public GameObject botonAdelante;
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    [Header("Botones Especiales")]
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
            if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();
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
    }

    public void IrACalificacionConVaso()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        PrepararVasoParaCalificacion();

        if (TransicionEscenasUI.Instance != null)
            TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaCalificacion);
        else
            SceneManager.LoadScene(indiceEscenaCalificacion);
    }

    private void PrepararVasoParaCalificacion()
    {
        ArrastrarVaso vaso = ObtenerVasoActivo();
        if (vaso == null)
        {
            Debug.LogWarning("[NavegadorEstacion] No se encontró ningún ArrastrarVaso activo para llevar a calificación.");
            return;
        }

        ParfaitMemoria.GuardarEstadoFinal(vaso);

        vaso.esVasoSeleccionado = false;
        vaso.transform.SetParent(null);
        DontDestroyOnLoad(vaso.gameObject);

        Debug.Log($"[NavegadorEstacion] Vaso '{vaso.gameObject.name}' marcado con DontDestroyOnLoad para escena índice {indiceEscenaCalificacion}.");

        SceneManager.sceneLoaded -= OnEscenaCalificacionCargada;
        SceneManager.sceneLoaded += OnEscenaCalificacionCargada;
    }

    private void OnEscenaCalificacionCargada(Scene escena, LoadSceneMode modo)
    {
        if (escena.buildIndex != indiceEscenaCalificacion)
        {
            Debug.Log($"[NavegadorEstacion] Escena cargada '{escena.name}' (índice {escena.buildIndex}) no coincide con indiceEscenaCalificacion ({indiceEscenaCalificacion}).");
            return;
        }

        SceneManager.sceneLoaded -= OnEscenaCalificacionCargada;

        ArrastrarVaso vaso = ParfaitMemoria.VasoFinal != null
            ? ParfaitMemoria.VasoFinal
            : FindAnyObjectByType<ArrastrarVaso>();

        GameObject destino = GameObject.Find(nombreDestinoCalificacion);
        if (destino == null && nombreDestinoCalificacion != "VasoCalificacion")
        {
            destino = GameObject.Find("VasoCalificacion");
        }

        if (vaso == null)
        {
            Debug.LogWarning("[NavegadorEstacion] En la escena de calificación no se encontró el vaso.");
            return;
        }

        vaso.gameObject.SetActive(true);
        vaso.transform.SetParent(null);

        if (destino != null)
        {
            vaso.transform.position = destino.transform.position;
            vaso.transform.rotation = destino.transform.rotation;
            vaso.transform.localScale = Vector3.one * EscalaCalificacion;
            vaso.MoverAEstacion(destino.transform, true);
            vaso.transform.localScale = Vector3.one * EscalaCalificacion;
            Debug.Log($"[NavegadorEstacion] Vaso '{vaso.gameObject.name}' colocado en '{destino.name}' con escala {EscalaCalificacion}.");
        }
        else
        {
            vaso.transform.localScale = Vector3.one * EscalaCalificacion;
            Debug.LogWarning($"[NavegadorEstacion] No se encontró el objeto destino '{nombreDestinoCalificacion}' en la escena de calificación.");
        }
    }

    private ArrastrarVaso ObtenerVasoActivo()
    {
        ArrastrarVaso[] vasos = FindObjectsByType<ArrastrarVaso>();
        for (int i = 0; i < vasos.Length; i++)
        {
            if (vasos[i] != null && vasos[i].esVasoSeleccionado)
                return vasos[i];
        }

        for (int i = 0; i < vasos.Length; i++)
        {
            if (vasos[i] != null)
                return vasos[i];
        }

        return null;
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

        bool haySiguiente = estacionActual < ultimoIndice;

        if (botonAdelante != null)
            botonAdelante.SetActive(haySiguiente);

        if (textoBotonAdelante != null)
        {
            textoBotonAdelante.gameObject.SetActive(haySiguiente);
            if (haySiguiente)
            {
                textoBotonAdelante.text = ObtenerEtiquetaEstacion(estacionActual + 1);
            }
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