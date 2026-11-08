using UnityEngine;
using UnityEngine.UI;

public class TicketVisual : MonoBehaviour
{
    // Esta función se llama desde el TicketRail
    public void Configurar(Sprite[] ingredientes)
    {
        Image[] slots = GetComponentsInChildren<Image>(true);
        int indiceIngrediente = 0;

        foreach (Image slot in slots)
        {
            // Ignoramos el fondo del ticket, solo queremos los huecos
            if (slot.gameObject != this.gameObject)
            {
                if (indiceIngrediente < ingredientes.Length)
                {
                    slot.sprite = ingredientes[indiceIngrediente];
                    slot.color = new Color(1, 1, 1, 1); // Prender
                    indiceIngrediente++;
                }
                else
                {
                    slot.color = new Color(1, 1, 1, 0); // Apagar
                }
            }
        }
    }
}