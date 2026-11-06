using UnityEngine;
using UnityEngine.UI;

public class TicketChiquito : MonoBehaviour
{
    public int miIndice;
    private TicketRail manager;

    public void Configurar(int indice, TicketRail m)
    {
        miIndice = indice;
        manager = m;
        GetComponent<Button>().onClick.AddListener(AlHacerClic);
    }

    void AlHacerClic()
    {
        manager.IntercambiarTicket(miIndice);
    }
}