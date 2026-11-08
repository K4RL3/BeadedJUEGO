using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClienteEstacion2 : MonoBehaviour
{
    [Header("UI del NPC")]
    public GameObject burbujaDialogo;
    public Image imagenEnBurbuja;

    [Header("Pedido")]
    public GameObject prefabDeEsteTicket;
    public Transform marcoTicketEstacion2;
    public string escenaCocina = "EstaciónArmado";

    void Start()
    {
        // 1. REVISAMOS SI ESTE NPC YA FUE ATENDIDO POR SU NOMBRE
        if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
        {
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            return; // Si ya pidió, se detiene aquí y no dicta nada
        }

        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);

        StartCoroutine(DictarPedido());
    }

    IEnumerator DictarPedido()
    {
        yield return new WaitForSeconds(1f);

        foreach (Transform child in marcoTicketEstacion2) Destroy(child.gameObject);

        GameObject ticketEnEscena = Instantiate(prefabDeEsteTicket, marcoTicketEstacion2);

        RectTransform rect = ticketEnEscena.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        Image[] ingredientesEnElTicket = ticketEnEscena.GetComponentsInChildren<Image>(true);
        List<Image> frutasReales = new List<Image>();

        foreach (Image img in ingredientesEnElTicket)
        {
            if (img.sprite != null && img.gameObject != ticketEnEscena)
            {
                frutasReales.Add(img);
                img.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < frutasReales.Count; i++)
        {
            if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
            if (imagenEnBurbuja != null)
            {
                imagenEnBurbuja.sprite = frutasReales[i].sprite;
                imagenEnBurbuja.color = new Color(1, 1, 1, 1);
            }

            frutasReales[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (imagenEnBurbuja != null) imagenEnBurbuja.color = new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(0.3f);
        }

        // 2. GUARDAMOS EL PEDIDO Y MARCAMOS AL NPC COMO ATENDIDO
        PedidoMemoria.TicketsActivos.Add(prefabDeEsteTicket);
        PedidoMemoria.NpcsAtendidos.Add(gameObject.name); // <-- AQUÍ SE GUARDA SU NOMBRE

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(escenaCocina);
    }
}