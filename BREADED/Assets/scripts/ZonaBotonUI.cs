using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ZonaBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum AccionBoton { Eliminar, SiguienteEstacion }
    public AccionBoton accionDelBoton = AccionBoton.SiguienteEstacion;

    public NavegadorEstacion navegador;

    [Header("Puntos de Trabajo por Estación (Opcional)")]
    [Tooltip("Punto donde se coloca el vaso según el índice de la estación destino")]
    public Transform[] workSpotsPorEstacion;
    [Tooltip("Punto de respaldo si no hay uno asignado para la estación destino")]
    public Transform workSpotPorDefecto;

    private bool mouseEncima = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
    }

    private ArrastrarVaso ObtenerVasoSostenido()
    {
        ArrastrarVaso[] vasos = FindObjectsByType<ArrastrarVaso>();
        for (int i = 0; i < vasos.Length; i++)
        {
            if (vasos[i] != null && vasos[i].esVasoSeleccionado)
                return vasos[i];
        }
        return null;
    }

    private Transform ObtenerWorkSpotParaEstacion(int indiceEstacion)
    {
        if (workSpotsPorEstacion != null && indiceEstacion >= 0 && indiceEstacion < workSpotsPorEstacion.Length)
        {
            if (workSpotsPorEstacion[indiceEstacion] != null)
                return workSpotsPorEstacion[indiceEstacion];
        }
        return workSpotPorDefecto;
    }

    void Update()
    {
        if (!mouseEncima || Mouse.current == null || !Mouse.current.leftButton.wasReleasedThisFrame)
            return;

        if (accionDelBoton == AccionBoton.Eliminar)
        {
            Vasos gestorVasos = FindAnyObjectByType<Vasos>();
            if (gestorVasos != null)
            {
                gestorVasos.EliminarVasoActual();
                return;
            }

            ArrastrarVaso vasoSostenido = ObtenerVasoSostenido();
            if (vasoSostenido != null)
            {
                Destroy(vasoSostenido.gameObject);
            }
            return;
        }

        if (accionDelBoton == AccionBoton.SiguienteEstacion)
        {
            ArrastrarVaso vaso = ObtenerVasoSostenido();
            if (vaso == null)
            {
                // Si no estabas sosteniendo el vaso, no dispara el avance accidentalmente
                return;
            }

            if (navegador == null)
            {
                Debug.LogWarning("ZonaBotonUI: Falta asignar navegador.");
                return;
            }

            int estacionActual = navegador.ObtenerEstacionActual();
            int siguienteEstacion = estacionActual + 1;

            Transform destinoVaso = ObtenerWorkSpotParaEstacion(siguienteEstacion);
            if (destinoVaso != null)
            {
                vaso.MoverAEstacion(destinoVaso);
            }

            navegador.BotonAdelante();
        }
    }
}