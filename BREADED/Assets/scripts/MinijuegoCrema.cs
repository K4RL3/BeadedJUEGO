using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MinijuegoCrema : MonoBehaviour
{
    public enum TipoRelleno { Amarillo, Blanca, Fresa }

    [Header("Boquillas en la Escena (3D)")]
    public Transform boquillaAmarilla;
    public Transform boquillaBlanca;
    public Transform boquillaFresa;
    public Transform puntoSalidaBoquilla;
    public float velocidadMoverBoquilla = 8f;

    [Header("Posición del Vaso durante el Minijuego")]
    public Transform posicionCentroVaso;
    public float velocidadMoverVaso = 7f;

    [Header("UI del Minijuego")]
    public GameObject canvasMinijuego;
    public Button stopButton;
    public RectTransform flecha;
    public RectTransform puntoIzquierdo;
    public RectTransform puntoDerecho;

    [Header("Velocidad y Zonas (0 = Izquierda, 1 = Derecha)")]
    public float velocidadBarra = 2.2f;
    [Range(0f, 1f)] public float inicioVerde = 0.38f;
    [Range(0f, 1f)] public float finVerde = 0.62f;
    [Range(0f, 1f)] public float inicioNaranja = 0.20f;
    [Range(0f, 1f)] public float finNaranja = 0.80f;

    [Header("Deformación de Relleno (Base 1,1,1)")]
    public float grosorVerde = 1.0f;
    public float grosorNaranja = 1.35f;
    public float grosorRojo = 1.75f;
    public float expansionAnchoRojo = 1.15f;

    [Header("Nombres de Rellenos en el Vaso")]
    public string nombreAmarillo = "Relleno_a";
    public string nombreBlanca = "Relleno_b";
    public string nombreFresa = "Relleno_f";

    private bool seleccionandoBoquilla = true;
    private bool minijuegoActivo = false;
    private bool regresandoBoquilla = false;

    private Transform vasoObjetivoActual;
    private Vector3 destinoVasoActual;
    private bool moviendoVaso;
    private bool regresandoVaso;
    private Vector3 posOriginalVaso;
    private Quaternion rotOriginalVaso;

    private float progreso;
    private int direccion = 1;
    private int frameInicioMinijuego = -1;

    private TipoRelleno saborActual;
    private Transform boquillaActiva;
    private Transform rellenoActual;
    private readonly List<Behaviour> scriptsHoverDesactivados = new List<Behaviour>();

    private Vector3 posInicialAmarilla, posInicialBlanca, posInicialFresa;
    private Quaternion rotInicialAmarilla, rotInicialBlanca, rotInicialFresa;

    void Awake()
    {
        if (boquillaAmarilla != null)
        {
            posInicialAmarilla = boquillaAmarilla.position;
            rotInicialAmarilla = boquillaAmarilla.rotation;
        }
        if (boquillaBlanca != null)
        {
            posInicialBlanca = boquillaBlanca.position;
            rotInicialBlanca = boquillaBlanca.rotation;
        }
        if (boquillaFresa != null)
        {
            posInicialFresa = boquillaFresa.position;
            rotInicialFresa = boquillaFresa.rotation;
        }

        if (stopButton != null)
            stopButton.onClick.AddListener(DetenerMinijuego);

        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);
    }

    void Update()
    {
        if (seleccionandoBoquilla)
        {
            DetectarClickBoquilla();
        }

        ActualizarMovimientoVaso();

        if (boquillaActiva != null && puntoSalidaBoquilla != null && !regresandoBoquilla)
        {
            boquillaActiva.position = Vector3.MoveTowards(
                boquillaActiva.position,
                puntoSalidaBoquilla.position,
                velocidadMoverBoquilla * Time.deltaTime
            );
            boquillaActiva.rotation = Quaternion.Slerp(
                boquillaActiva.rotation,
                puntoSalidaBoquilla.rotation,
                velocidadMoverBoquilla * Time.deltaTime
            );
        }

        if (regresandoBoquilla && boquillaActiva != null)
        {
            Vector3 posDestino = ObtenerPosInicial(saborActual);
            Quaternion rotDestino = ObtenerRotInicial(saborActual);

            boquillaActiva.position = Vector3.MoveTowards(
                boquillaActiva.position,
                posDestino,
                velocidadMoverBoquilla * Time.deltaTime
            );
            boquillaActiva.rotation = Quaternion.Slerp(
                boquillaActiva.rotation,
                rotDestino,
                velocidadMoverBoquilla * Time.deltaTime
            );

            if (Vector3.Distance(boquillaActiva.position, posDestino) < 0.005f)
            {
                boquillaActiva.position = posDestino;
                boquillaActiva.rotation = rotDestino;
                RestaurarHoverBoquilla();
                regresandoBoquilla = false;
                boquillaActiva = null;
                MostrarTodasLasBoquillas();
                seleccionandoBoquilla = true;
            }
        }

        if (minijuegoActivo)
        {
            progreso += direccion * velocidadBarra * Time.deltaTime;
            if (progreso >= 1f)
            {
                progreso = 1f;
                direccion = -1;
            }
            else if (progreso <= 0f)
            {
                progreso = 0f;
                direccion = 1;
            }

            ActualizarPosicionFlecha();

            if (Time.frameCount > frameInicioMinijuego &&
                Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                DetenerMinijuego();
            }
        }
    }

    private void ActualizarMovimientoVaso()
    {
        if (!moviendoVaso || vasoObjetivoActual == null)
            return;

        vasoObjetivoActual.position = Vector3.MoveTowards(
            vasoObjetivoActual.position,
            destinoVasoActual,
            velocidadMoverVaso * Time.deltaTime
        );

        if (regresandoVaso)
        {
            vasoObjetivoActual.rotation = Quaternion.Slerp(
                vasoObjetivoActual.rotation,
                rotOriginalVaso,
                velocidadMoverVaso * Time.deltaTime
            );
        }

        if (Vector3.Distance(vasoObjetivoActual.position, destinoVasoActual) < 0.005f)
        {
            vasoObjetivoActual.position = destinoVasoActual;
            if (regresandoVaso)
            {
                vasoObjetivoActual.rotation = rotOriginalVaso;
                regresandoVaso = false;
            }
            moviendoVaso = false;
        }
    }

    private void DetectarClickBoquilla()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || Camera.main == null)
            return;

        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(rayo, out RaycastHit golpe, 100f))
            return;

        if (boquillaAmarilla != null && (golpe.transform == boquillaAmarilla || golpe.transform.IsChildOf(boquillaAmarilla)))
        {
            IniciarConBoquilla(TipoRelleno.Amarillo, boquillaAmarilla);
        }
        else if (boquillaBlanca != null && (golpe.transform == boquillaBlanca || golpe.transform.IsChildOf(boquillaBlanca)))
        {
            IniciarConBoquilla(TipoRelleno.Blanca, boquillaBlanca);
        }
        else if (boquillaFresa != null && (golpe.transform == boquillaFresa || golpe.transform.IsChildOf(boquillaFresa)))
        {
            IniciarConBoquilla(TipoRelleno.Fresa, boquillaFresa);
        }
    }

    private bool HayVasoPresente(out Transform vasoObjetivo)
    {
        vasoObjetivo = ObtenerVasoObjetivo();
        return vasoObjetivo != null && vasoObjetivo.gameObject.activeInHierarchy;
    }

    private void IniciarConBoquilla(TipoRelleno sabor, Transform elegida)
    {
        if (!HayVasoPresente(out Transform vasoObjetivo))
        {
            Debug.LogWarning("[MinijuegoCrema] No hay un vaso presente en la estación de cremas.");
            return;
        }

        vasoObjetivoActual = vasoObjetivo;
        posOriginalVaso = vasoObjetivo.position;
        rotOriginalVaso = vasoObjetivo.rotation;

        saborActual = sabor;
        boquillaActiva = elegida;
        regresandoBoquilla = false;
        regresandoVaso = false;
        seleccionandoBoquilla = false;

        if (posicionCentroVaso != null)
        {
            destinoVasoActual = posicionCentroVaso.position;
            moviendoVaso = true;
        }

        DesactivarHoverBoquilla(elegida);

        if (boquillaAmarilla != null) boquillaAmarilla.gameObject.SetActive(boquillaAmarilla == elegida);
        if (boquillaBlanca != null) boquillaBlanca.gameObject.SetActive(boquillaBlanca == elegida);
        if (boquillaFresa != null) boquillaFresa.gameObject.SetActive(boquillaFresa == elegida);

        PrepararRellenosEnVaso(vasoObjetivo, saborActual);

        progreso = 0f;
        direccion = 1;
        minijuegoActivo = true;
        frameInicioMinijuego = Time.frameCount;

        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(true);
    }

    public void DetenerMinijuego()
    {
        if (!minijuegoActivo) return;
        minijuegoActivo = false;

        if (rellenoActual != null)
        {
            bool enVerde = progreso >= inicioVerde && progreso <= finVerde;
            bool enNaranja = !enVerde && progreso >= inicioNaranja && progreso <= finNaranja;

            float escalaY = grosorRojo;
            float escalaXZ = expansionAnchoRojo;
            string zona = "ROJO";

            if (enVerde)
            {
                escalaY = grosorVerde;
                escalaXZ = 1f;
                zona = "VERDE";
            }
            else if (enNaranja)
            {
                escalaY = grosorNaranja;
                escalaXZ = 1.05f;
                zona = "NARANJA";
            }

            rellenoActual.localScale = new Vector3(escalaXZ, escalaY, escalaXZ);
            Debug.Log($"[MinijuegoCrema] Detenido en {progreso:F2} -> {zona} (Scale: {rellenoActual.localScale})");
        }

        if (vasoObjetivoActual != null)
        {
            destinoVasoActual = posOriginalVaso;
            regresandoVaso = true;
            moviendoVaso = true;
        }

        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);

        regresandoBoquilla = true;
    }

    private void DesactivarHoverBoquilla(Transform boquilla)
    {
        RestaurarHoverBoquilla();
        if (boquilla == null) return;

        MonoBehaviour[] componentes = boquilla.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < componentes.Length; i++)
        {
            MonoBehaviour comp = componentes[i];
            if (comp == null || comp == this) continue;

            if (comp.GetType().Name.ToLower().Contains("hover") && comp.enabled)
            {
                comp.enabled = false;
                scriptsHoverDesactivados.Add(comp);
            }
        }
    }

    private void RestaurarHoverBoquilla()
    {
        for (int i = 0; i < scriptsHoverDesactivados.Count; i++)
        {
            if (scriptsHoverDesactivados[i] != null)
                scriptsHoverDesactivados[i].enabled = true;
        }
        scriptsHoverDesactivados.Clear();
    }

    private void MostrarTodasLasBoquillas()
    {
        if (boquillaAmarilla != null) boquillaAmarilla.gameObject.SetActive(true);
        if (boquillaBlanca != null) boquillaBlanca.gameObject.SetActive(true);
        if (boquillaFresa != null) boquillaFresa.gameObject.SetActive(true);
    }

    private Vector3 ObtenerPosInicial(TipoRelleno sabor) => sabor switch
    {
        TipoRelleno.Amarillo => posInicialAmarilla,
        TipoRelleno.Blanca => posInicialBlanca,
        TipoRelleno.Fresa => posInicialFresa,
        _ => posInicialAmarilla
    };

    private Quaternion ObtenerRotInicial(TipoRelleno sabor) => sabor switch
    {
        TipoRelleno.Amarillo => rotInicialAmarilla,
        TipoRelleno.Blanca => rotInicialBlanca,
        TipoRelleno.Fresa => rotInicialFresa,
        _ => rotInicialAmarilla
    };

    private void ActualizarPosicionFlecha()
    {
        if (flecha == null || puntoIzquierdo == null || puntoDerecho == null) return;
        flecha.position = Vector3.Lerp(puntoIzquierdo.position, puntoDerecho.position, progreso);
    }

    private Transform ObtenerVasoObjetivo()
    {
        ArrastrarVaso[] vasos = FindObjectsByType<ArrastrarVaso>();
        for (int i = 0; i < vasos.Length; i++)
        {
            if (vasos[i] != null && vasos[i].gameObject.activeInHierarchy && vasos[i].esVasoSeleccionado)
                return vasos[i].transform;
        }

        for (int i = 0; i < vasos.Length; i++)
        {
            if (vasos[i] != null && vasos[i].gameObject.activeInHierarchy)
                return vasos[i].transform;
        }

        return null;
    }

    private void PrepararRellenosEnVaso(Transform vaso, TipoRelleno elegido)
    {
        Transform rA = vaso.Find(nombreAmarillo);
        Transform rB = vaso.Find(nombreBlanca);
        Transform rF = vaso.Find(nombreFresa);

        ConfigurarEstadoInicial(rA, elegido == TipoRelleno.Amarillo);
        ConfigurarEstadoInicial(rB, elegido == TipoRelleno.Blanca);
        ConfigurarEstadoInicial(rF, elegido == TipoRelleno.Fresa);

        rellenoActual = elegido switch
        {
            TipoRelleno.Amarillo => rA,
            TipoRelleno.Blanca => rB,
            TipoRelleno.Fresa => rF,
            _ => rA
        };
    }

    private void ConfigurarEstadoInicial(Transform relleno, bool activar)
    {
        if (relleno == null) return;
        relleno.localScale = Vector3.one;
        relleno.gameObject.SetActive(activar);
    }
}