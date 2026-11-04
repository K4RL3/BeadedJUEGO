using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClienteEstacion2 : MonoBehaviour
{
    [Header("UI del NPC")]
    public GameObject burbujaDialogo;
    public Image[] todosLosIconosEnBurbuja;

    [Header("Pedido")]
    public int[] indicesDelPedido;
    public string escenaCocina = "EstaciónArmado";
    public TicketVisual ticketVisual;

    void Start()
    {
        if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
        {
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            return;
        }

        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);

        for (int i = 0; i < todosLosIconosEnBurbuja.Length; i++)
        {
            todosLosIconosEnBurbuja[i].gameObject.SetActive(false);
        }

        StartCoroutine(DictarPedido());
    }

    IEnumerator DictarPedido()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < indicesDelPedido.Length; i++)
        {
            int indiceSprite = indicesDelPedido[i];

            if (burbujaDialogo != null) burbujaDialogo.SetActive(true);

            if (indiceSprite < todosLosIconosEnBurbuja.Length)
            {
                todosLosIconosEnBurbuja[indiceSprite].gameObject.SetActive(true);
            }

            if (ticketVisual != null)
            {
                ticketVisual.AgregarIcono(todosLosIconosEnBurbuja[indiceSprite].sprite);
            }

            yield return new WaitForSeconds(1f);

            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);

            if (indiceSprite < todosLosIconosEnBurbuja.Length)
            {
                todosLosIconosEnBurbuja[indiceSprite].gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.3f);
        }

        if (!PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
        {
            PedidoMemoria.NpcsAtendidos.Add(gameObject.name);
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(escenaCocina);
    }
}