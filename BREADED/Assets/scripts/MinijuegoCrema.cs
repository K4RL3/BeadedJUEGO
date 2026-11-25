using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MinijuegoCrema : MonoBehaviour
{
    [Header("Interfaz (UI)")]
    public GameObject panelMinijuego;
    public RectTransform barraFondo;      // El rectángulo completo de la barra
    public RectTransform zonaVerde;       // El área ideal de disparo
    public RectTransform indicador;       // La flecha o línea que se mueve
    public TextMeshProUGUI textoSabor;    // Opcional: muestra el sabor seleccionado
    public TextMeshProUGUI textoResultado;// Opcional: muestra "¡Perfecto!", "Bien", etc.

    [Header("Configuración del Minijuego")]
    public float velocidadIndicador = 650f;
    [Range(0.1f, 0.5f)]
    public float anchoZonaVerde = 0.22f;  // 22% del ancho de la barra

    private bool jugando = false;
    private float direccion = 1f;
    private float posX = 0f;
    private float limiteX = 0f;
    private string saborActual = "";

    void Start()
    {
        if (panelMinijuego != null)
            panelMinijuego.SetActive(false);
    }

    public void IniciarMinijuego(string sabor)
    {
        if (jugando) return;

        // Verificamos que haya un vaso activo seleccionado antes de abrir el minijuego
        CapaCremaVaso vasoActivo = ObtenerCapaCremaVasoActivo();
        if (vasoActivo == null)
        {
            Debug.LogWarning("No hay un vaso seleccionado con CapaCremaVaso.");
            return;
        }

        saborActual = sabor;
        jugando = true;

        if (textoSabor != null) textoSabor.text = "Crema: " + saborActual;
        if (textoResultado != null) textoResultado.text = "";

        if (panelMinijuego != null) panelMinijuego.SetActive(true);

        // Calculamos los límites según el ancho actual de la barra en pantalla
        float anchoBarra = barraFondo.rect.width;
        limiteX = anchoBarra * 0.5f;

        // Ajustamos el tamaño de la zona verde en el centro
        zonaVerde.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, anchoBarra * anchoZonaVerde);
        zonaVerde.anchoredPosition = new Vector2(0f, zonaVerde.anchoredPosition.y);

        // Arrancamos el indicador desde el extremo izquierdo
        posX = -limiteX;
        direccion = 1f;
        indicador.anchoredPosition = new Vector2(posX, indicador.anchoredPosition.y);

        if (ManejadorAudio.Instance != null)
            ManejadorAudio.Instance.PlayBoton();
    }

    void Update()
    {
        if (!jugando) return;

        // Movimiento de ida y vuelta del indicador
        posX += direccion * velocidadIndicador * Time.deltaTime;

        if (posX >= limiteX)
        {
            posX = limiteX;
            direccion = -1f;
        }
        else if (posX <= -limiteX)
        {
            posX = -limiteX;
            direccion = 1f;
        }

        indicador.anchoredPosition = new Vector2(posX, indicador.anchoredPosition.y);

        // Detener con clic izquierdo o barra espaciadora
        bool disparo = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                       (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        if (disparo)
        {
            EvaluarTiro();
        }
    }

    void EvaluarTiro()
    {
        jugando = false;

        float distanciaAlCentro = Mathf.Abs(posX - zonaVerde.anchoredPosition.x);
        float radioZonaVerde = zonaVerde.rect.width * 0.5f;

        float precision;     // 0 a 100
        float factorAltura;  // Escala vertical de la crema en el vaso
        string mensaje;

        if (distanciaAlCentro <= radioZonaVerde)
        {
            // ¡Dentro de la zona verde!
            precision = 100f;
            factorAltura = 1f;
            mensaje = "¡Perfecto!";
        }
        else
        {
            // Fuera de la zona verde: calculamos qué tan lejos quedó
            float exceso = (distanciaAlCentro - radioZonaVerde) / Mathf.Max(1f, (limiteX - radioZonaVerde));
            precision = Mathf.Clamp(100f - (exceso * 70f), 30f, 85f);

            // Si cayó a la izquierda queda chaparrita; si cayó a la derecha queda desbordada
            factorAltura = (posX < 0f) ? 0.65f : 1.35f;
            mensaje = (posX < 0f) ? "¡Muy poca crema!" : "¡Demasiada crema!";
        }

        if (textoResultado != null)
            textoResultado.text = mensaje;

        // Aplicamos la crema en el vaso activo
        CapaCremaVaso vasoActivo = ObtenerCapaCremaVasoActivo();
        if (vasoActivo != null)
        {
            vasoActivo.AplicarCapaPorMinijuego(precision, factorAltura, saborActual);
        }

        // Cerramos el panel después de una breve pausa para que se vea el resultado
        Invoke(nameof(CerrarPanel), 0.65f);
    }

    void CerrarPanel()
    {
        if (panelMinijuego != null)
            panelMinijuego.SetActive(false);
    }

    CapaCremaVaso ObtenerCapaCremaVasoActivo()
    {
        ArrastrarVaso[] vasos = FindObjectsByType<ArrastrarVaso>(FindObjectsInactive.Exclude);
        for (int i = 0; i < vasos.Length; i++)
        {
            if (vasos[i].esVasoSeleccionado)
            {
                return vasos[i].GetComponent<CapaCremaVaso>();
            }
        }
        return null;
    }
}