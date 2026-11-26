using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReceptorCalificacionParfait : MonoBehaviour
{
    private const string NOMBRE_ESCENA_RESULTADOS = "Resultados";
    private const int INDICE_ESCENA_RESULTADOS = 6;
    private const float ESCALA_POR_DEFECTO = 2.31f;
    private const int ESTADO_NEUTRAL_POR_DEFECTO = 0;
    private const int ESTADO_FELIZ_POR_DEFECTO = 2;
    private const int ESTADO_ENOJADO_POR_DEFECTO = 3;

    [Header("Exhibición del Vaso")]
    public Transform puntoExhibicion;
    public float escalaVaso = ESCALA_POR_DEFECTO;

    [Header("Ticket Ordenado (Opcional)")]
    public GameObject ticketPrefabRespaldo;

    [Header("Animator con parámetro 'Estados'")]
    public Animator animatorReaccion;
    public string nombreParametroEstados = "Estados";
    public int estadoNeutral = ESTADO_NEUTRAL_POR_DEFECTO;
    public int estadoFeliz = ESTADO_FELIZ_POR_DEFECTO;
    public int estadoEnojado = ESTADO_ENOJADO_POR_DEFECTO;

    [Header("Efecto de Victoria")]
    public ParticleSystem cortinaConfeti;
    public float duracionRespaldoAnimacionVictoria = 2f;
    public float margenPostAnimacionVictoria = 0f;
    public float esperaConfetiAntesDeCambiarEscena = 2.5f;

    [Header("Flujo y Transición a Resultados")]
    public float esperaAntesDeEvaluar = 1f;
    public float esperaAntesDeCambiarEscena = 2.5f;
    public bool evaluarAutomaticamente = true;
    public bool irAResultadosAutomaticamente = true;
    public bool limpiarParfaitAlCambiarEscena = true;
    public bool usarIndiceEscena = false;

    [Header("Opciones de Comparación")]
    public bool ignorarMayusculas = true;
    public bool permitirCoincidenciaParcial = true;

    public static bool UltimoPedidoAprobado { get; private set; }
    public bool UltimoResultadoAprobado => UltimoPedidoAprobado;

    private Coroutine rutinaEvaluacion;
    private Coroutine rutinaSecuenciaVictoria;
    private Coroutine rutinaCambioEscena;

    private void Start()
    {
        DetenerConfeti();
        ColocarVaso();
        SetEstadoAnimator(estadoNeutral);

        if (evaluarAutomaticamente)
        {
            IniciarEvaluacion();
        }
    }

    public void IniciarEvaluacion()
    {
        if (rutinaEvaluacion != null)
        {
            StopCoroutine(rutinaEvaluacion);
        }

        rutinaEvaluacion = StartCoroutine(RutinaEvaluarPedido());
    }

    private IEnumerator RutinaEvaluarPedido()
    {
        if (esperaAntesDeEvaluar > 0f)
        {
            yield return new WaitForSeconds(esperaAntesDeEvaluar);
        }

        EvaluarPedido();
        rutinaEvaluacion = null;
    }

    private void ColocarVaso()
    {
        if (!ParfaitMemoria.ParfaitListoParaCalificar || ParfaitMemoria.VasoFinal == null)
        {
            Debug.LogWarning("[Calificacion] No hay vaso del parfait listo para mostrar.");
            return;
        }

        Transform vasoTransform = ParfaitMemoria.VasoFinal.transform;
        vasoTransform.SetParent(null);
        vasoTransform.gameObject.SetActive(true);

        if (puntoExhibicion != null)
        {
            vasoTransform.position = puntoExhibicion.position;
            vasoTransform.rotation = puntoExhibicion.rotation;
        }

        vasoTransform.localScale = Vector3.one * escalaVaso;
    }

    public void EvaluarPedido()
    {
        GameObject ticketRef = ObtenerTicketActual();
        List<string> ingredientesPedidos = ExtraerNombresDesdeTicket(ticketRef);
        List<string> elementosEnVaso = ObtenerElementosDelVaso();

        List<string> faltantes = new List<string>();
        foreach (string pedido in ingredientesPedidos)
        {
            if (!ExisteEnCapas(pedido, elementosEnVaso))
            {
                faltantes.Add(pedido);
            }
        }

        bool todoCorrecto = ingredientesPedidos.Count > 0 && faltantes.Count == 0;
        UltimoPedidoAprobado = todoCorrecto;

        SetEstadoAnimator(todoCorrecto ? estadoFeliz : estadoEnojado);

        if (todoCorrecto)
        {
            if (rutinaSecuenciaVictoria != null)
            {
                StopCoroutine(rutinaSecuenciaVictoria);
            }
            rutinaSecuenciaVictoria = StartCoroutine(RutinaVictoriaConConfeti());
        }
        else
        {
            DetenerConfeti();
            if (irAResultadosAutomaticamente)
            {
                ProgramarCambioAResultados(esperaAntesDeCambiarEscena);
            }
        }
    }

    private IEnumerator RutinaVictoriaConConfeti()
    {
        yield return null;

        float esperaAnimacion = ObtenerDuracionAnimacionActual();
        if (esperaAnimacion > 0f)
        {
            yield return new WaitForSeconds(esperaAnimacion);
        }

        if (margenPostAnimacionVictoria > 0f)
        {
            yield return new WaitForSeconds(margenPostAnimacionVictoria);
        }

        ActivarConfeti();
        rutinaSecuenciaVictoria = null;

        if (irAResultadosAutomaticamente)
        {
            ProgramarCambioAResultados(esperaConfetiAntesDeCambiarEscena);
        }
    }

    private float ObtenerDuracionAnimacionActual()
    {
        if (animatorReaccion == null || animatorReaccion.runtimeAnimatorController == null)
        {
            return duracionRespaldoAnimacionVictoria;
        }

        AnimatorStateInfo info = animatorReaccion.GetCurrentAnimatorStateInfo(0);
        if (info.length > 0.01f && !float.IsInfinity(info.length))
        {
            float restante = info.length * Mathf.Max(0f, 1f - (info.normalizedTime % 1f));
            return restante > 0.05f ? restante : info.length;
        }

        return duracionRespaldoAnimacionVictoria;
    }

    public void ActivarConfeti()
    {
        if (cortinaConfeti == null) return;

        cortinaConfeti.gameObject.SetActive(true);
        cortinaConfeti.Clear();
        cortinaConfeti.Play();
    }

    public void DetenerConfeti()
    {
        if (cortinaConfeti == null) return;

        cortinaConfeti.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void ProgramarCambioAResultados(float demora)
    {
        if (rutinaCambioEscena != null)
        {
            StopCoroutine(rutinaCambioEscena);
        }

        rutinaCambioEscena = StartCoroutine(RutinaIrAResultados(demora));
    }

    private IEnumerator RutinaIrAResultados(float demora)
    {
        if (demora > 0f)
        {
            yield return new WaitForSeconds(demora);
        }

        DetenerConfeti();

        if (limpiarParfaitAlCambiarEscena)
        {
            FinalizarYLimpiarParfait();
        }

        rutinaCambioEscena = null;

        if (usarIndiceEscena)
        {
            SceneManager.LoadScene(INDICE_ESCENA_RESULTADOS);
        }
        else
        {
            SceneManager.LoadScene(NOMBRE_ESCENA_RESULTADOS);
        }
    }

    private List<string> ObtenerElementosDelVaso()
    {
        List<string> elementos = new List<string>();

        if (ParfaitMemoria.CapasAgregadas != null)
        {
            elementos.AddRange(ParfaitMemoria.CapasAgregadas);
        }

        if (ParfaitMemoria.VasoFinal != null)
        {
            Transform[] hijos = ParfaitMemoria.VasoFinal.GetComponentsInChildren<Transform>(false);
            foreach (Transform t in hijos)
            {
                if (t == null || !t.gameObject.activeInHierarchy)
                    continue;

                string nombreLimpio = t.gameObject.name.Replace("(Clone)", "").Trim();
                if (!string.IsNullOrEmpty(nombreLimpio) && !elementos.Contains(nombreLimpio))
                {
                    elementos.Add(nombreLimpio);
                }
            }
        }

        return elementos;
    }

    private void SetEstadoAnimator(int valorEstado)
    {
        if (animatorReaccion == null || string.IsNullOrEmpty(nombreParametroEstados))
            return;

        if (animatorReaccion.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"[Calificacion] El Animator en '{animatorReaccion.gameObject.name}' no tiene asignado un AnimatorController.");
            return;
        }

        foreach (AnimatorControllerParameter param in animatorReaccion.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Int && param.name == nombreParametroEstados)
            {
                animatorReaccion.SetInteger(nombreParametroEstados, valorEstado);
                return;
            }
        }
    }

    private GameObject ObtenerTicketActual()
    {
        if (PedidoMemoria.TicketsActivos != null && PedidoMemoria.TicketsActivos.Count > 0)
        {
            return PedidoMemoria.TicketsActivos[PedidoMemoria.TicketsActivos.Count - 1];
        }

        return ticketPrefabRespaldo;
    }

    private List<string> ExtraerNombresDesdeTicket(GameObject ticket)
    {
        List<string> nombres = new List<string>();
        if (ticket == null) return nombres;

        Image[] imagenes = ticket.GetComponentsInChildren<Image>(true);
        foreach (Image img in imagenes)
        {
            if (img == null || img.sprite == null || img.gameObject == ticket)
                continue;

            string nombreSprite = img.sprite.name.Replace("_Burbuja", "").Trim();
            if (!string.IsNullOrEmpty(nombreSprite) && !nombres.Contains(nombreSprite))
            {
                nombres.Add(nombreSprite);
            }
        }

        return nombres;
    }

    private bool ExisteEnCapas(string ingredientePedido, List<string> capasPuestas)
    {
        string esperado = Normalizar(ingredientePedido);

        foreach (string capa in capasPuestas)
        {
            if (string.IsNullOrEmpty(capa)) continue;
            string actual = Normalizar(capa);

            if (actual == esperado)
                return true;

            if (permitirCoincidenciaParcial && (actual.Contains(esperado) || esperado.Contains(actual)))
                return true;
        }

        return false;
    }

    private string Normalizar(string texto)
    {
        string limpio = texto.Replace("_", " ").Trim();
        return ignorarMayusculas ? limpio.ToLowerInvariant() : limpio;
    }

    public void FinalizarYLimpiarParfait()
    {
        if (ParfaitMemoria.VasoFinal != null)
        {
            Destroy(ParfaitMemoria.VasoFinal.gameObject);
        }

        ParfaitMemoria.Limpiar();

        if (PedidoMemoria.TicketsActivos != null && PedidoMemoria.TicketsActivos.Count > 0)
        {
            PedidoMemoria.TicketsActivos.RemoveAt(PedidoMemoria.TicketsActivos.Count - 1);
        }
    }

    [ContextMenu("Test / Imprimir Diagnostico")]
    public void ImprimirDiagnostico()
    {
        GameObject ticketRef = ObtenerTicketActual();
        List<string> pedidos = ExtraerNombresDesdeTicket(ticketRef);
        List<string> elementos = ObtenerElementosDelVaso();

        Debug.Log($"[Test Calificacion] Ticket usado: {(ticketRef != null ? ticketRef.name : "NINGUNO")}");
        Debug.Log($"[Test Calificacion] Ingredientes en ticket ({pedidos.Count}): {string.Join(", ", pedidos)}");
        Debug.Log($"[Test Calificacion] Elementos detectados en vaso ({elementos.Count}): {string.Join(", ", elementos)}");

        foreach (string p in pedidos)
        {
            bool ok = ExisteEnCapas(p, elementos);
            Debug.Log($"   - '{p}' -> {(ok ? "OK" : "FALTA")}");
        }
    }

    [ContextMenu("Test / Simular Todos Correctos")]
    public void SimularTodosCorrectos()
    {
        GameObject ticketRef = ObtenerTicketActual();
        List<string> pedidos = ExtraerNombresDesdeTicket(ticketRef);

        ParfaitMemoria.CapasAgregadas = new List<string>(pedidos);
        ParfaitMemoria.ParfaitListoParaCalificar = true;

        EvaluarPedido();
        ImprimirDiagnostico();
    }

    [ContextMenu("Test / Simular Falta Ingrediente")]
    public void SimularFaltaIngrediente()
    {
        GameObject ticketRef = ObtenerTicketActual();
        List<string> pedidos = ExtraerNombresDesdeTicket(ticketRef);

        ParfaitMemoria.CapasAgregadas = new List<string>();
        if (pedidos.Count > 1)
        {
            for (int i = 0; i < pedidos.Count - 1; i++)
            {
                ParfaitMemoria.CapasAgregadas.Add(pedidos[i]);
            }
        }

        ParfaitMemoria.ParfaitListoParaCalificar = true;
        EvaluarPedido();
        ImprimirDiagnostico();
    }
}