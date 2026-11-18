using UnityEngine;

public class DetectorVaso : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        IngredienteVolador ing = other.GetComponent<IngredienteVolador>();

        if (ing != null)
        {
            // 1. Frenamos la fruta en seco ANTES de hacerla hija
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // Apagamos la física para que no se mueva
            }

            // 2. Desactivamos el Collider para que la siguiente fruta no rebote contra esta
            Collider col = other.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // 3. ¡LA MAGIA! Hacemos la fruta hija del vaso
            other.transform.SetParent(this.transform);

            // 4. Si la fruta tiene un script de ingrediente, le avisamos
            if (ing != null) ing.AtraparEnVaso();
        }
    }
}