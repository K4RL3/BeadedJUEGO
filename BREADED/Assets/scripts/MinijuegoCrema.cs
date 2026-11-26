using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MinijuegoCrema : MonoBehaviour
{
    public enum TipoRelleno { Amarillo, Blanca, Fresa }

    [Header("Avance a Toppings")]
    public NavegadorEstacion navegador;
    public int indiceEstacionToppings = 3;
    public Transform destinoToppings;
    public bool moverVasoADestinoToppings = true;

    [Header("Colores de Crema")]
    public Color colorAmarillo = new Color(1f, 0.85f, 0.2f);
    public Color colorBlanca = new Color(0.98f, 0.96f, 0.92f);
    public Color colorFresa = new Color(1f, 0.45f, 0.65f);

    private const float VelocidadMoverBoquilla = 3f;
    private const float VelocidadMoverVaso = 3f;
    private const float DuracionParticulas = 3f;
    private const float EsperaTrasParticulas = 1f;

    private const float VelocidadBarra = 0.7f;
    private const float RadioVerde = 0.12f;
    private const float RadioAmarillo = 0.24f;
    private const float RadioNaranja = 0.38f;

    private const float GrosorVerde = 1f;
    private const float GrosorAmarillo = 0.8f;
    private const float GrosorNaranja = 0.5f;
    private const float GrosorRojo = 0.2f;

    private const float ToleranciaCentro2 = 0.25f;
    private const string NombreAmarillo = "Relleno_a";
    private const string NombreBlanca = "Relleno_b";
    private const string NombreFresa = "Relleno_f";

    private Transform boquillaAmarilla;
    private Transform boquillaBlanca;
    private Transform boquillaFresa;
    private Transform puntoSalidaBoquilla;
    private Transform centro2;
    private Transform posicionCentroVaso;

    private ParticleSystem particulasCrema;
    private GameObject canvasMinijuego;
    private Button stopButton;
    private RectTransform flecha;
    private RectTransform puntoIzquierdo;
    private RectTransform puntoDerecho;

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
    private Coroutine secuenciaTransicionCoroutine;
    private readonly List<Behaviour> scriptsHoverDesactivados = new List<Behaviour>();
    private readonly HashSet<EntityId> vasosConCremaAplicada = new HashSet<EntityId>();

    private Vector3 posInicialAmarilla, posInicialBlanca, posInicialFresa;
    private Quaternion rotInicialAmarilla, rotInicialBlanca, rotInicialFresa;

    void Awake()
    {
        if (navegador == null)
            navegador = FindAnyObjectByType<NavegadorEstacion>();

        BuscarReferenciasEnEscena();

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

        if (particulasCrema != null)
            particulasCrema.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void BuscarReferenciasEnEscena()
    {
        GameObject objAmarilla = GameObject.Find("Crema_Amarilla");
        GameObject objBlanca = GameObject.Find("Crema_blanca");
        GameObject objFresa = GameObject.Find("Crema_fresa");
        GameObject objSalida = GameObject.Find("pos crema");
        GameObject objCentro2 = GameObject.Find("centro (2)");
        GameObject objCentro4 = GameObject.Find("centro (4)");
        GameObject objParticulas = GameObject.Find("Particle_crema");
        GameObject objCanvas = GameObject.Find("Canvas_MinijuegoCrema");
        GameObject objStop = GameObject.Find("stopButton");
        GameObject objFlecha = GameObject.Find("flecha");
        GameObject objStart = GameObject.Find("start");
        GameObject objFin = GameObject.Find("fin");

        if (objAmarilla != null) boquillaAmarilla = objAmarilla.transform;
        if (objBlanca != null) boquillaBlanca = objBlanca.transform;
        if (objFresa != null) boquillaFresa = objFresa.transform;
        if (objSalida != null) puntoSalidaBoquilla = objSalida.transform;
        if (objCentro2 != null) centro2 = objCentro2.transform;
        if (objCentro4 != null) posicionCentroVaso = objCentro4.transform;

        if (objParticulas != null) particulasCrema = objParticulas.GetComponent<ParticleSystem>();
        if (objCanvas != null) canvasMinijuego = objCanvas;
        if (objStop != null) stopButton = objStop.GetComponent<Button>();
        if (objFlecha != null) flecha = objFlecha.GetComponent<RectTransform>();
        if (objStart != null) puntoIzquierdo = objStart.GetComponent<RectTransform>();
        if (objFin != null) puntoDerecho = objFin.GetComponent<RectTransform>();
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
                VelocidadMoverBoquilla * Time.deltaTime
            );
            boquillaActiva.rotation = Quaternion.Slerp(
                boquillaActiva.rotation,
                puntoSalidaBoquilla.rotation,
                VelocidadMoverBoquilla * Time.deltaTime
            );
        }

        if (regresandoBoquilla && boquillaActiva != null)
        {
            Vector3 posDestino = ObtenerPosInicial(saborActual);
            Quaternion rotDestino = ObtenerRotInicial(saborActual);

            boquillaActiva.position = Vector3.MoveTowards(
                boquillaActiva.position,
                posDestino,
                VelocidadMoverBoquilla * Time.deltaTime
            );
            boquillaActiva.rotation = Quaternion.Slerp(
                boquillaActiva.rotation,
                rotDestino,
                VelocidadMoverBoquilla * Time.deltaTime
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
            progreso += direccion * VelocidadBarra * Time.deltaTime;
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
            VelocidadMoverVaso * Time.deltaTime
        );

        if (regresandoVaso)
        {
            vasoObjetivoActual.rotation = Quaternion.Slerp(
                vasoObjetivoActual.rotation,
                rotOriginalVaso,
                VelocidadMoverVaso * Time.deltaTime
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

            ArrastrarVaso scriptVaso = vasoObjetivoActual.GetComponent<ArrastrarVaso>();
            if (scriptVaso != null)
            {
                scriptVaso.ActualizarPosicionBase();
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

    private bool VasoListoEnCentro2(out Transform vasoObjetivo)
    {
        vasoObjetivo = ObtenerVasoObjetivo();
        if (vasoObjetivo == null || !vasoObjetivo.gameObject.activeInHierarchy)
            return false;

        if (vasosConCremaAplicada.Contains(vasoObjetivo.GetEntityId()))
        {
            Debug.Log("[MinijuegoCrema] Este vaso ya tiene crema aplicada; solo se permite una vez por vaso.");
            return false;
        }

        if (centro2 == null)
        {
            Debug.LogWarning("[MinijuegoCrema] Falta encontrar o asignar 'centro (2)'.");
            return false;
        }

        float distancia = Vector3.Distance(vasoObjetivo.position, centro2.position);
        return distancia <= ToleranciaCentro2;
    }

    private void IniciarConBoquilla(TipoRelleno sabor, Transform elegida)
    {
        if (!VasoListoEnCentro2(out Transform vasoObjetivo))
        {
            Debug.LogWarning("[MinijuegoCrema] El vaso no está presente, no está en 'centro (2)' o ya tiene crema.");
            return;
        }

        if (secuenciaTransicionCoroutine != null)
        {
            StopCoroutine(secuenciaTransicionCoroutine);
            secuenciaTransicionCoroutine = null;
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

        OcultarTodosLosRellenos(vasoObjetivo);
        rellenoActual = ObtenerRellenoPorSabor(vasoObjetivo, saborActual);

        progreso = 0f;
        direccion = 1;
        minijuegoActivo = true;
        frameInicioMinijuego = Time.frameCount;

        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(true);
    }

    public void DetenerMinijuego()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();
        if (!minijuegoActivo) return;
        minijuegoActivo = false;

        if (vasoObjetivoActual != null)
        {
            vasosConCremaAplicada.Add(vasoObjetivoActual.GetEntityId());
        }

        if (rellenoActual != null)
        {
            float distanciaCentro = Mathf.Abs(progreso - 0.5f);

            float escalaY;
            string zona;

            if (distanciaCentro <= RadioVerde)
            {
                escalaY = GrosorVerde;
                zona = "VERDE";
            }
            else if (distanciaCentro <= RadioAmarillo)
            {
                escalaY = GrosorAmarillo;
                zona = "AMARILLO";
            }
            else if (distanciaCentro <= RadioNaranja)
            {
                escalaY = GrosorNaranja;
                zona = "NARANJA";
            }
            else
            {
                escalaY = GrosorRojo;
                zona = "ROJO";
            }

            rellenoActual.gameObject.SetActive(true);
            rellenoActual.localScale = new Vector3(1f, escalaY, 1f);

            Collider[] colliders = rellenoActual.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;
            }
            Physics.SyncTransforms();

            Debug.Log($"[MinijuegoCrema] Detenido en {progreso:F2} -> {zona} (Scale: {rellenoActual.localScale})");
        }

        if (secuenciaTransicionCoroutine != null)
            StopCoroutine(secuenciaTransicionCoroutine);

        secuenciaTransicionCoroutine = StartCoroutine(SecuenciaParticulasYAvance());
    }

    private IEnumerator SecuenciaParticulasYAvance()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayCrema();
        if (particulasCrema != null)
        {
            var main = particulasCrema.main;
            main.startColor = saborActual switch
            {
                TipoRelleno.Amarillo => colorAmarillo,
                TipoRelleno.Blanca => colorBlanca,
                TipoRelleno.Fresa => colorFresa,
                _ => colorAmarillo
            };

            particulasCrema.gameObject.SetActive(true);
            particulasCrema.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particulasCrema.Play();
        }

        if (DuracionParticulas > 0f)
            yield return new WaitForSeconds(DuracionParticulas);

        if (particulasCrema != null)
            particulasCrema.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (EsperaTrasParticulas > 0f)
            yield return new WaitForSeconds(EsperaTrasParticulas);

        if (vasoObjetivoActual != null)
        {
            if (moverVasoADestinoToppings && destinoToppings != null)
            {
                destinoVasoActual = destinoToppings.position;
                regresandoVaso = false;
                moviendoVaso = true;
            }
            else
            {
                destinoVasoActual = posOriginalVaso;
                regresandoVaso = true;
                moviendoVaso = true;
            }
        }

        regresandoBoquilla = true;

        if (navegador != null)
            navegador.MoverAEstacionEspecifica(indiceEstacionToppings);

        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);

        secuenciaTransicionCoroutine = null;
    }

    public void ResetearCremaEnVaso(Transform vaso)
    {
        if (secuenciaTransicionCoroutine != null)
        {
            StopCoroutine(secuenciaTransicionCoroutine);
            secuenciaTransicionCoroutine = null;
        }

        if (particulasCrema != null)
            particulasCrema.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        minijuegoActivo = false;
        regresandoBoquilla = false;
        regresandoVaso = false;
        moviendoVaso = false;
        boquillaActiva = null;
        rellenoActual = null;
        seleccionandoBoquilla = true;

        RestaurarHoverBoquilla();
        MostrarTodasLasBoquillas();

        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);

        if (vaso != null)
        {
            vasosConCremaAplicada.Remove(vaso.GetEntityId());
            OcultarTodosLosRellenos(vaso);
        }
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

    private void OcultarTodosLosRellenos(Transform vaso)
    {
        Transform rA = vaso.Find(NombreAmarillo);
        Transform rB = vaso.Find(NombreBlanca);
        Transform rF = vaso.Find(NombreFresa);

        ResetearRellenoOculto(rA);
        ResetearRellenoOculto(rB);
        ResetearRellenoOculto(rF);
    }

    private void ResetearRellenoOculto(Transform relleno)
    {
        if (relleno == null) return;
        relleno.localScale = Vector3.one;
        relleno.gameObject.SetActive(false);
    }

    private Transform ObtenerRellenoPorSabor(Transform vaso, TipoRelleno elegido)
    {
        return elegido switch
        {
            TipoRelleno.Amarillo => vaso.Find(NombreAmarillo),
            TipoRelleno.Blanca => vaso.Find(NombreBlanca),
            TipoRelleno.Fresa => vaso.Find(NombreFresa),
            _ => vaso.Find(NombreAmarillo)
        };
    }
}