using UnityEngine;

public class DetectorVaso : MonoBehaviour
{
    [Header("Estadísticas de este Vaso")]
    public int ingredientesEnEsteVaso = 0;

    // Variable estática para llevar el conteo global de todos los vasos (opcional)
    public static int totalIngredientesGlobal = 0;

    void OnTriggerEnter(Collider other)
    {
        // 1. Revisamos si lo que entró es un ingrediente
        IngredienteVolador ing = other.GetComponent<IngredienteVolador>();

        if (ing != null)
        {
            // 2. LÓGICA DE PUNTUACIÓN
            ingredientesEnEsteVaso++;
            totalIngredientesGlobal++;

            // Imprimimos en consola el conteo
            Debug.Log("¡" + ing.nombreIngrediente + " atrapada! Vaso actual: " + ingredientesEnEsteVaso + " | Total global: " + totalIngredientesGlobal);

            // 3. LÓGICA DE ATRAPAR LA FRUTA
            other.transform.SetParent(this.transform); // Se vuelve hijo del vaso

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // Quitamos física
            }

            Collider col = other.GetComponent<Collider>();
            if (col != null) col.enabled = false; // Quitamos colisión para que las demás caigan encima

            ing.AtraparEnVaso(); // Le avisamos a la fruta
        }
    }

    // Función para evaluar si cumpliste la orden (para probar)
    public void EvaluarPedido(int ingredientesEsperados)
    {
        Debug.Log("------ EVALUANDO PEDIDO ------");

        if (ingredientesEnEsteVaso == ingredientesEsperados)
        {
            Debug.Log("todo cayo");
        }
        else if (ingredientesEnEsteVaso < ingredientesEsperados)
        {
            Debug.Log("Faltaron ingredientes." + ingredientesEnEsteVaso + " de " + ingredientesEsperados);
        }

    }

    // Función para borrar el último ingrediente (por si se equivoca)
    public void BorrarUltimoIngrediente()
    {
        if (transform.childCount == 0) return;

        Transform ultimoHijo = transform.GetChild(transform.childCount - 1);
        Destroy(ultimoHijo.gameObject);

        // Restamos al contador
        ingredientesEnEsteVaso--;
        totalIngredientesGlobal--;

        Debug.Log("Ingrediente borrado. Quedan: " + ingredientesEnEsteVaso);
    }
}