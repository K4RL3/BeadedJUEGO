using UnityEngine;

public class DetectorTopping : MonoBehaviour
{
    [Header("Padre donde se fijará el topping (Opcional)")]
    public Transform padreDestino;

    void OnTriggerEnter(Collider other)
    {
        IngredienteVolador ing = other.GetComponent<IngredienteVolador>();
        if (ing == null) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.isKinematic) return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider col = other.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Transform objetivoPadre = padreDestino != null ? padreDestino : transform;
        other.transform.SetParent(objetivoPadre);

        ing.AtraparEnVaso();
    }
}