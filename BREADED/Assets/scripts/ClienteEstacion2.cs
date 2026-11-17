using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // Para buscar en el proyecto
#endif

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
        if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
        {
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            return;
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
                string nombreOriginal = frutasReales[i].sprite.name;
                string nombreBurbuja = nombreOriginal + "_Burbuja";

                Sprite spriteBurbuja = BuscarSpriteEnProyecto(nombreBurbuja);

                if (spriteBurbuja != null)
                {
                    imagenEnBurbuja.sprite = spriteBurbuja;
                }
                else
                {
                    imagenEnBurbuja.sprite = frutasReales[i].sprite; // Fallback
                }

                imagenEnBurbuja.color = new Color(1, 1, 1, 1);
                imagenEnBurbuja.preserveAspect = true;
            }

            frutasReales[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (imagenEnBurbuja != null) imagenEnBurbuja.color = new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(0.3f);
        }

        PedidoMemoria.TicketsActivos.Add(prefabDeEsteTicket);
        PedidoMemoria.NpcsAtendidos.Add(gameObject.name);

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(escenaCocina);
    }

    // --- FUNCIÓN PARA BUSCAR EL SPRITE EN TU CARPETA EXACTA ---
    private Sprite BuscarSpriteEnProyecto(string nombre)
    {
#if UNITY_EDITOR
        // Busca todos los sprites (incluso si no están en Resources)
        string[] guids = AssetDatabase.FindAssets("t:Sprite " + nombre);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Verifica que esté en tu carpeta de Sprites > Ingredientes
            if (path.Contains("Assets/Sprites/Ingredientes/") && path.EndsWith(nombre + ".png"))
            {
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
        }
#endif
        return null;
    }
}