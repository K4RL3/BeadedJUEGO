using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClienteEstacion2 : MonoBehaviour
{
    [Header("UI del NPC")]
    public GameObject burbujaDialogo;       // El fondo de la nube
    public Image[] todosLosIconosEnBurbuja; // AQUÍ ARRASTRA TODOS LOS SPRITES DE LA BURBUJA

    [Header("Pedido")]
    public int[] indicesDelPedido;          // Los números de los ingredientes que pide ESTE NPC (ej. 0 para Fresa, 2 para Kiwi)
    public string escenaCocina = "EstaciónArmado";
    public TicketVisual ticketVisual;

    void Start()
    {
        // Apagamos la burbuja y todos los ingredientes al inicio
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

        // Recorremos solo los ingredientes que este NPC específico quiere
        for (int i = 0; i < indicesDelPedido.Length; i++)
        {
            int indiceSprite = indicesDelPedido[i];

            // 1. Mostramos la nube y el ingrediente específico
            if (burbujaDialogo != null) burbujaDialogo.SetActive(true);

            if (indiceSprite < todosLosIconosEnBurbuja.Length)
            {
                todosLosIconosEnBurbuja[indiceSprite].gameObject.SetActive(true);
            }

            // 2. Lo mandamos al ticket
            if (ticketVisual != null)
            {
                // Asumiendo que tu TicketVisual también usa índices o le pasas el Sprite directo
                ticketVisual.AgregarIcono(todosLosIconosEnBurbuja[indiceSprite].sprite);
            }

            // 3. Esperamos 1 segundo
            yield return new WaitForSeconds(1f);

            // 4. Apagamos la nube y el ingrediente
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);

            if (indiceSprite < todosLosIconosEnBurbuja.Length)
            {
                todosLosIconosEnBurbuja[indiceSprite].gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(escenaCocina);
    }
}