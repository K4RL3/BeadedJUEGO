using UnityEngine;

public class IngredienteVolador : MonoBehaviour
{
    public string nombreIngrediente = "Fresa";
    private bool estaAtrapado = false;

    void Start()
    {
        // Se destruye a los 5 segundos si no cae en el vaso (para no llenar la escena de basura)
        Invoke("AutoDestruir", 5f);
    }

    public void AtraparEnVaso()
    {
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayIngredienteVolador();
        estaAtrapado = true;
        CancelInvoke("AutoDestruir"); // Cancelamos el temporizador, ya está a salvo
        Debug.Log("Ingrediente asegurado en el vaso.");
    }

    private void LimpiarIngredientesDeVaso(Transform vaso)
    {
        if (vaso == null) return;

        for (int i = vaso.childCount - 1; i >= 0; i--)
        {
            Transform hijo = vaso.GetChild(i);

            // Si el hijo es un ingrediente o tiene IngredienteVolador, lo destruimos
            if (hijo.GetComponentInChildren<IngredienteVolador>(true) != null ||
                hijo.GetComponent<IngredienteVolador>() != null)
            {
                Destroy(hijo.gameObject);
            }
        }
    }

    void AutoDestruir()
    {
        if (!estaAtrapado) Destroy(gameObject);
    }
}