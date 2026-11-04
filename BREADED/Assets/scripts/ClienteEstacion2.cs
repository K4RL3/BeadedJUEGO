using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClienteEstacion2 : MonoBehaviour
{
    [Header("UI del NPC")]
    public GameObject burbujaDialogo;    // El objeto de la nube
    public GameObject contenedorIngrediente; // El Empty donde va el sprite
    public Image spriteIngrediente;      // El componente Image de ese Empty

    [Header("Pedido")]
    public Sprite[] ingredientesQueQuiere;
    public string escenaCocina = "EstaciónArmado";
    public TicketVisual ticketVisual;

    void Start()
    {
        // 1. Apagamos todo al inicio
        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (contenedorIngrediente != null) contenedorIngrediente.SetActive(false);

        // 2. Guardamos en memoria para la cocina
        PedidoMemoria.IngredientesActuales = ingredientesQueQuiere;

        // 3. Iniciamos el dictado
        StartCoroutine(DictarPedido());
    }

    IEnumerator DictarPedido()
    {
        yield return new WaitForSeconds(1f); // Pausa para entrar en escena

        for (int i = 0; i < ingredientesQueQuiere.Length; i++)
        {
            // 1. Encendemos la burbuja y el contenedor del ingrediente
            if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
            if (contenedorIngrediente != null) contenedorIngrediente.SetActive(true);

            // 2. Asignamos la imagen del ingrediente
            if (spriteIngrediente != null)
            {
                spriteIngrediente.sprite = ingredientesQueQuiere[i];
                spriteIngrediente.preserveAspect = true; // Evita que se deforme
            }

            // 3. Lo mandamos al ticket (por si lo tienes en esta escena)
            if (ticketVisual != null) ticketVisual.AgregarIcono(ingredientesQueQuiere[i]);

            // 4. Esperamos 1 segundo (aquí se ve el ingrediente sobre la burbuja)
            yield return new WaitForSeconds(1f);

            // 5. Apagamos la burbuja y el contenedor (desaparecen)
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (contenedorIngrediente != null) contenedorIngrediente.SetActive(false);

            // 6. Pequeña pausa antes del siguiente ingrediente
            yield return new WaitForSeconds(0.3f);
        }

        // 7. Terminó el dictado, pasamos a la cocina
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(escenaCocina);
    }
}