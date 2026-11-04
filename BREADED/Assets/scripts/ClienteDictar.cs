using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClienteDictar : MonoBehaviour
{
    [Header("UI")]
    public GameObject burbujaDialogo;
    public Image imagenEnBurbuja;

    [Header("Datos")]
    public Sprite[] ingredientes;
    public TicketUI ticketUI;
    public string escenaCocina = "EstaciónArmado";

    void Start()
    {
        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (imagenEnBurbuja != null) imagenEnBurbuja.color = new Color(1, 1, 1, 0);
        StartCoroutine(SecuenciaDictado());
    }

    IEnumerator SecuenciaDictado()
    {
        yield return new WaitForSeconds(1f); // Pausa inicial

        for (int i = 0; i < ingredientes.Length; i++)
        {
            // 1. Mostrar en la burbuja
            if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
            if (imagenEnBurbuja != null)
            {
                imagenEnBurbuja.sprite = ingredientes[i];
                imagenEnBurbuja.color = new Color(1, 1, 1, 1);
            }

            yield return new WaitForSeconds(1f); // Espera 1 segundo

            // 2. Ocultar de la burbuja
            if (imagenEnBurbuja != null) imagenEnBurbuja.color = new Color(1, 1, 1, 0);
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);

            // 3. Escribir en el ticket visual de la escena
            if (ticketUI != null) ticketUI.MostrarIngrediente(i, ingredientes[i]);

            yield return new WaitForSeconds(0.5f); // Pausa entre ingredientes
        }

        // 4. Guardar en el OrderManager y pasar a la cocina automáticamente
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.AgregarPedido(this.name, ingredientes);
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(escenaCocina);
    }
}