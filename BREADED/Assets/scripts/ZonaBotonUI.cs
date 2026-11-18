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
        Debug.Log("Mouse sobre el botón: " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
        Debug.Log("Mouse salió del botón: " + gameObject.name);
    }

    void Update()
    {
        // Detectamos el clic directamente en el Update
        if (mouseEncima && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ArrastrarVaso vaso = FindAnyObjectByType<ArrastrarVaso>();

            if (vaso != null)
            {
                Debug.Log("¡Vaso encontrado! Ejecutando acción: " + accionDelBoton);

                if (accionDelBoton == AccionBoton.Eliminar)
                {
                    Destroy(vaso.gameObject);
                }
                else if (accionDelBoton == AccionBoton.IrAAcabados)
                {
                    vaso.MoverAEstacion(workSpotAcabados);

                    if (navegador != null)
                    {
                        navegador.MoverAEstacionEspecifica(2);
                    }
                }
            }
            else
            {
                Debug.Log("El mouse está sobre el botón, pero no se encontró ningún vaso siendo arrastrado.");
            }
        }
    }
}