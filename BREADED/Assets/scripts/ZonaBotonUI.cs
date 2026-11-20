using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ZonaBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum AccionBoton { Eliminar, IrAAcabados }
    public AccionBoton accionDelBoton;

    public NavegadorEstacion navegador;
    public Transform workSpotAcabados;

    private bool mouseEncima = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
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

            ArrastrarVaso vasoAEliminar = ObtenerVasoActivo();
            if (vasoAEliminar != null)
            {
                Destroy(vasoAEliminar.gameObject);
            }
            return;
        }

        if (accionDelBoton == AccionBoton.IrAAcabados)
        {
            ArrastrarVaso vaso = ObtenerVasoActivo();
            if (vaso == null)
            {
                Debug.LogWarning("ZonaBotonUI: No se encontró un vaso para mover a acabados.");
                return;
            }

            if (workSpotAcabados != null)
            {
                vaso.MoverAEstacion(workSpotAcabados);
            }
            else
            {
                Debug.LogWarning("ZonaBotonUI: Falta asignar workSpotAcabados.");
            }

            if (navegador != null)
            {
                navegador.MoverAEstacionEspecifica(2);
            }
            else
            {
                Debug.LogWarning("ZonaBotonUI: Falta asignar navegador.");
            }
        }
    }
}