using UnityEngine;
using UnityEngine.UI;

public class TicketVisual : MonoBehaviour
{
    [Header("Contenedor")]
    public Transform contenedorIconos;
    float tamañoIcono = 50f;

    void Start()
    {
        // Limpiamos cualquier icono previo
        foreach (Transform child in contenedorIconos)
        {
            Destroy(child.gameObject);
        }

        // Si venimos de otra escena y hay un pedido guardado, lo dibujamos todo de golpe
        if (PedidoMemoria.IngredientesActuales != null)
        {
            foreach (Sprite sprite in PedidoMemoria.IngredientesActuales)
            {
                AgregarIcono(sprite);
            }
        }
    }

    // Esta función la llamará el NPC de la Estación 2 para ir llenando el ticket en vivo
    public void AgregarIcono(Sprite sprite)
    {
        GameObject nuevoIcono = new GameObject("Icono_" + sprite.name);
        nuevoIcono.transform.SetParent(contenedorIconos, false);

        Image img = nuevoIcono.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
    }
}