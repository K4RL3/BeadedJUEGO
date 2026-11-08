using UnityEngine;

public class TicketRail : MonoBehaviour
{
    [Header("UI")]
    public GameObject marcoDelTicket; // <-- CAMBIO: Ahora pedimos el GameObject completo del marco
    public Transform lugarTicketGrande; // El contenedor (puede ser el mismo marco)

    [Header("Prefabs")]
    public GameObject prefabTicketGrande;

    void Start()
    {
        // 1. Limpiamos cualquier ticket que haya quedado
        foreach (Transform child in lugarTicketGrande.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. Revisamos si hay pedidos guardados
        if (PedidoMemoria.TicketsActivos.Count == 0)
        {
            // SI NO HAY PEDIDOS, APAGAMOS EL MARCO POR COMPLETO
            if (marcoDelTicket != null) marcoDelTicket.SetActive(false);
            return;
        }

        // 3. SI HAY PEDIDOS, ENCENDEMOS EL MARCO
        if (marcoDelTicket != null) marcoDelTicket.SetActive(true);

        // 4. Tomamos el último pedido y lo dibujamos
        int ultimoIndice = PedidoMemoria.TicketsActivos.Count - 1;
        GameObject ticketCreado = Instantiate(PedidoMemoria.TicketsActivos[ultimoIndice], lugarTicketGrande);

        RectTransform rect = ticketCreado.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}