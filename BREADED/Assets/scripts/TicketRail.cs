using System.Collections.Generic;
using UnityEngine;

public class TicketRail : MonoBehaviour
{
    [Header("UI")]
    public Transform lugarTicketGrande;
    public Transform rielContenedor;

    [Header("Prefabs")]
    public GameObject prefabTicketGrande;
    public GameObject prefabTicketChiquito;

    private GameObject ticketGrandeActual;
    private int indiceActualGrande = -1;

    void Start()
    {
        foreach (Transform child in rielContenedor) Destroy(child.gameObject);

        if (PedidoMemoria.PedidosActivos.Count > 0)
        {
            MostrarEnGrande(PedidoMemoria.PedidosActivos.Count - 1);
        }
    }

    public void MostrarEnGrande(int indice)
    {
        if (ticketGrandeActual != null)
        {
            Destroy(ticketGrandeActual);
            CrearChiquito(indiceActualGrande);
        }
        else
        {
            for (int i = 0; i < indice; i++) CrearChiquito(i);
        }

        ticketGrandeActual = Instantiate(prefabTicketGrande, lugarTicketGrande);

        // Usa tu script TicketVisual
        ticketGrandeActual.GetComponent<TicketVisual>().Configurar(PedidoMemoria.PedidosActivos[indice]);
        indiceActualGrande = indice;
    }

    public void CrearChiquito(int indice)
    {
        GameObject chiquito = Instantiate(prefabTicketChiquito, rielContenedor);

        // ¡AQUÍ ESTÁ LA CORRECCIÓN! Usamos tu script TicketChiquito
        TicketChiquito scriptChiquito = chiquito.GetComponent<TicketChiquito>();
        if (scriptChiquito != null)
        {
            scriptChiquito.Configurar(indice, this);
        }
    }

    public void IntercambiarTicket(int indiceClickeado)
    {
        foreach (Transform child in rielContenedor)
        {
            // Buscamos usando tu script TicketChiquito
            TicketChiquito btn = child.GetComponent<TicketChiquito>();
            if (btn != null && btn.miIndice == indiceClickeado)
            {
                Destroy(child.gameObject);
                break;
            }
        }
        MostrarEnGrande(indiceClickeado);
    }
}