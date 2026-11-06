using System.Collections;
using System.Collections.Generic;
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
    public TicketVisual ticketEnEstacion2;

    void Start()
    {
        if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name)) return;

        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        for (int i = 0; i < todosLosIconosEnBurbuja.Length; i++)
            todosLosIconosEnBurbuja[i].gameObject.SetActive(false);

        StartCoroutine(DictarPedido());
    }

    IEnumerator DictarPedido()
    {
        yield return new WaitForSeconds(1f);
        List<Sprite> spritesDeEstePedido = new List<Sprite>();

        for (int i = 0; i < indicesDelPedido.Length; i++)
        {
            int indiceSprite = indicesDelPedido[i];
            if (burbujaDialogo != null) burbujaDialogo.SetActive(true);

            if (indiceSprite < todosLosIconosEnBurbuja.Length)
            {
                todosLosIconosEnBurbuja[indiceSprite].gameObject.SetActive(true);
                Sprite sprite = todosLosIconosEnBurbuja[indiceSprite].sprite;
                spritesDeEstePedido.Add(sprite);

                if (ticketEnEstacion2 != null) ticketEnEstacion2.AgregarIcono(sprite);
            }

            yield return new WaitForSeconds(1f);

            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (indiceSprite < todosLosIconosEnBurbuja.Length)
                todosLosIconosEnBurbuja[indiceSprite].gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);
        }

        // GUARDAMOS EN PEDIDOMEMORIA
        PedidoMemoria.PedidosActivos.Add(spritesDeEstePedido.ToArray());
        PedidoMemoria.NpcsAtendidos.Add(gameObject.name);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(escenaCocina);
    }
}