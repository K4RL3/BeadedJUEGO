using UnityEngine;
using UnityEngine.UI;

public class TicketVisual : MonoBehaviour
{
    // Arrastra aquí todos tus íconos pre-acomodados
    public GameObject[] todosLosIconos;

    void Start()
    {
        LimpiarTicket();

        // Si hay pedido en memoria, encendemos los que tocan
        if (PedidoMemoria.IngredientesActuales != null)
        {
            foreach (Sprite sprite in PedidoMemoria.IngredientesActuales)
            {
                AgregarIcono(sprite);
            }
        }
    }

    public void LimpiarTicket()
    {
        for (int i = 0; i < todosLosIconos.Length; i++)
        {
            todosLosIconos[i].SetActive(false);
        }
    }

    // En lugar de crear uno nuevo, busca el que ya existe y lo enciende
    public void AgregarIcono(Sprite sprite)
    {
        for (int i = 0; i < todosLosIconos.Length; i++)
        {
            // Si el nombre del objeto coincide con el nombre de la imagen, lo prende
            if (todosLosIconos[i].name == sprite.name)
            {
                todosLosIconos[i].SetActive(true);
                return; // Sale del bucle para no encender más
            }
        }
    }
}