using UnityEngine;
using UnityEngine.UI;

public class TicketVisual : MonoBehaviour
{
    public Image[] slotsIngredientes;

    // Para llenarlo de golpe (Cocina)
    public void Configurar(Sprite[] ingredientes)
    {
        for (int i = 0; i < slotsIngredientes.Length; i++)
        {
            slotsIngredientes[i].sprite = null;
            slotsIngredientes[i].color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < ingredientes.Length; i++)
        {
            if (i < slotsIngredientes.Length)
            {
                slotsIngredientes[i].sprite = ingredientes[i];
                slotsIngredientes[i].color = new Color(1, 1, 1, 1);
            }
        }
    }

    // Para llenarlo de a uno (Estación 2)
    public void AgregarIcono(Sprite sprite)
    {
        for (int i = 0; i < slotsIngredientes.Length; i++)
        {
            if (slotsIngredientes[i].sprite == null)
            {
                slotsIngredientes[i].sprite = sprite;
                slotsIngredientes[i].color = new Color(1, 1, 1, 1);
                break;
            }
        }
    }
}