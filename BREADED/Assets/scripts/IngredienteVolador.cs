using UnityEngine;

public class IngredienteVolador : MonoBehaviour
{
    public string nombreIngrediente = "Fresa";
    private bool estaAtrapado = false;

    void Start()
    {
        // Se destruye a los 5 segundos si no llega a atraparse
        Invoke("AutoDestruir", 5f);
    }

    public void AtraparEnVaso()
    {
        estaAtrapado = true;
        CancelInvoke("AutoDestruir"); // Cancelamos el borrado
    }

    void AutoDestruir()
    {
        if (!estaAtrapado) Destroy(gameObject);
    }
}