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
        estaAtrapado = true;
        CancelInvoke("AutoDestruir"); // Cancelamos el temporizador, ya está a salvo
        Debug.Log("Ingrediente asegurado en el vaso.");
    }

    void AutoDestruir()
    {
        if (!estaAtrapado) Destroy(gameObject);
    }
}