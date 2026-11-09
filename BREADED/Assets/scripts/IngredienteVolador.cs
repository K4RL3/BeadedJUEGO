using UnityEngine;

public class IngredienteVolador : MonoBehaviour
{
    public string nombreIngrediente = "Fresa";
    private bool yaCayoEnElVaso = false;

    void Start()
    {
        // Si quieres que se destruyan al caer al piso (fuera del vaso), pon un l�mite de Y
        // Si est�n muy abajo, desaparecen para no llenar la escena de basura.
        if (transform.position.y < -2f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si choca contra el vaso o contra otra fruta que ya est� en el vaso...
        if (!yaCayoEnElVaso && (collision.gameObject.CompareTag("Vaso") || collision.gameObject.CompareTag("Ingrediente")))
        {
            yaCayoEnElVaso = true;

            // Opcional: Etiquetamos este ingrediente como "Ingrediente" para que los dem�s sepan que pueden caer encima
            gameObject.tag = "Ingrediente";

            Debug.Log("�" + nombreIngrediente + " se acomod� en el vaso!");

            // Peque�o truco de f�sica: Bajamos un poco el centro de masa para que no se caigan f�cil
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Dejamos que la f�sica act�e un momento, pero le bajamos la velocidad para que no ruede
                rb.linearVelocity *= 0.5f;
                rb.angularVelocity *= 0.5f;
            }
        }
    }
}