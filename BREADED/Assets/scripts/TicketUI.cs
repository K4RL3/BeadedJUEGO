using UnityEngine;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    public GameObject panelTicketGrande;
    public Image[] slotsIngredientes;

    void Start()
    {
        if (panelTicketGrande != null) panelTicketGrande.SetActive(false);

        // Si estamos en la cocina y ya hay pedidos guardados, lo cargamos
        CargarPedidoDeMemoria();
    }

    public void MostrarIngrediente(int index, Sprite sprite)
    {
        if (panelTicketGrande != null) panelTicketGrande.SetActive(true);

        if (index < slotsIngredientes.Length)
        {
            slotsIngredientes[index].sprite = sprite;
            slotsIngredientes[index].color = new Color(1, 1, 1, 1);
        }
    }

    void CargarPedidoDeMemoria()
    {
        if (OrderManager.Instance != null && OrderManager.Instance.pedidosActivos.Count > 0)
        {
            Pedido pedido = OrderManager.Instance.pedidosActivos[0];
            for (int i = 0; i < slotsIngredientes.Length; i++)
            {
                if (i < pedido.ingredientes.Length)
                {
                    MostrarIngrediente(i, pedido.ingredientes[i]);
                }
                else
                {
                    slotsIngredientes[i].color = new Color(1, 1, 1, 0);
                }
            }
        }
    }
}