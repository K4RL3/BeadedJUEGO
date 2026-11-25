using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ZonaBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum AccionBoton { Eliminar, IrASiguiente }
    public AccionBoton accionDelBoton;

    public NavegadorEstacion navegador;
    public int indiceEstacionFrutas = 1;
    public int indiceEstacionCremas = 2;
    public int indiceEstacionToppings = 3;
    public bool ocultarYDesactivarEnEstacionCremas = true;

    public Transform workSpotCremas;
    public Transform workSpotToppings;

    private RectTransform rectTransform;
    private bool mouseEncima = false;
    private int ultimoFrameEjecutado = -1;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (navegador == null)
            navegador = FindAnyObjectByType<NavegadorEstacion>();
    }

    private bool EstaOcultoPorEstacionCremas()
    {
        if (!ocultarYDesactivarEnEstacionCremas || navegador == null)
            return false;

        return navegador.ObtenerEstacionActual() == indiceEstacionCremas;
    }

    private void SincronizarVisibilidadEstacion()
    {
        if (!ocultarYDesactivarEnEstacionCremas || navegador == null)
            return;

        bool debeOcultar = navegador.ObtenerEstacionActual() == indiceEstacionCremas;
        if (debeOcultar && gameObject.activeSelf)
        {
            mouseEncima = false;
            gameObject.SetActive(false);
        }
        else if (!debeOcultar && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EstaOcultoPorEstacionCremas()) return;
        mouseEncima = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
    }

    public bool EstaSobreZona(Vector2 posicionPantalla)
    {
        if (!gameObject.activeInHierarchy || EstaOcultoPorEstacionCremas()) return false;
        if (mouseEncima) return true;
        if (rectTransform == null) return false;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera camCanvas = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera
            : null;

        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, posicionPantalla, camCanvas);
    }

    public void EjecutarAccionDesdeArrastre(ArrastrarVaso vaso)
    {
        if (EstaOcultoPorEstacionCremas()) return;
        if (ultimoFrameEjecutado == Time.frameCount) return;
        ultimoFrameEjecutado = Time.frameCount;

        if (vaso != null)
            vaso.SeleccionarEsteVaso();

        if (accionDelBoton == AccionBoton.Eliminar)
        {
            EliminarVaso(vaso);
        }
        else if (accionDelBoton == AccionBoton.IrASiguiente)
        {
            EjecutarSiguiente(vaso);
        }
    }

    private void EliminarVaso(ArrastrarVaso vaso)
    {
        Vasos gestorVasos = FindAnyObjectByType<Vasos>();
        if (gestorVasos != null)
        {
            string nombreRef = vaso != null ? vaso.gameObject.name : "vaso actual";
            Debug.Log($"[ZonaBotonUI] Borrado solicitado via Vasos.EliminarVasoActual() (ref: {nombreRef}).");
            gestorVasos.EliminarVasoActual();
            return;
        }

        ArrastrarVaso vasoAEliminar = vaso != null ? vaso : ObtenerVasoActivo();
        if (vasoAEliminar != null)
        {
            string nombreObjeto = vasoAEliminar.gameObject.name;
            Destroy(vasoAEliminar.gameObject);
            Debug.Log($"[ZonaBotonUI] Borrado objeto '{nombreObjeto}'.");
        }
        else
        {
            Debug.Log("[ZonaBotonUI] No se encontro ningun vaso para borrar.");
        }
    }

    private void EjecutarSiguiente(ArrastrarVaso vaso)
    {
        if (navegador == null)
        {
            Debug.LogWarning("ZonaBotonUI: Falta asignar navegador.");
            return;
        }

        int estacionActual = navegador.ObtenerEstacionActual();

        if (estacionActual == indiceEstacionCremas && ocultarYDesactivarEnEstacionCremas)
            return;

        if (estacionActual == indiceEstacionFrutas)
        {
            if (vaso != null && workSpotCremas != null)
                vaso.MoverAEstacion(workSpotCremas);
            else if (workSpotCremas == null)
                Debug.LogWarning("ZonaBotonUI: Falta asignar workSpotCremas.");

            navegador.MoverAEstacionEspecifica(indiceEstacionCremas);
        }
        else if (estacionActual == indiceEstacionCremas)
        {
            if (vaso != null && workSpotToppings != null)
                vaso.MoverAEstacion(workSpotToppings);
            else if (workSpotToppings == null)
                Debug.LogWarning("ZonaBotonUI: Falta asignar workSpotToppings.");

            navegador.MoverAEstacionEspecifica(indiceEstacionToppings);
        }
        else if (estacionActual == indiceEstacionToppings)
        {
            navegador.IrACalificacionConVaso();
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

    void Update()
    {
        SincronizarVisibilidadEstacion();
        if (!gameObject.activeInHierarchy || EstaOcultoPorEstacionCremas()) return;

        if (!mouseEncima || Mouse.current == null || !Mouse.current.leftButton.wasReleasedThisFrame)
            return;

        if (ultimoFrameEjecutado == Time.frameCount) return;
        ultimoFrameEjecutado = Time.frameCount;

        ArrastrarVaso vaso = ObtenerVasoActivo();

        if (accionDelBoton == AccionBoton.Eliminar)
        {
            EliminarVaso(vaso);
            return;
        }

        if (accionDelBoton == AccionBoton.IrASiguiente)
        {
            if (vaso == null && navegador != null && navegador.ObtenerEstacionActual() != indiceEstacionToppings)
            {
                Debug.LogWarning("ZonaBotonUI: No se encontró un vaso para mover a acabados.");
                return;
            }

            EjecutarSiguiente(vaso);
        }
    }
}