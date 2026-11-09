using UnityEngine;

public class DetectorVaso : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        IngredienteVolador ing = other.GetComponent<IngredienteVolador>();

        if (ing != null)
        {
            // El ingrediente lleg� al fondo
            Debug.Log("�" + other.name + " cay� dentro del vaso!");

            // Detenemos su f�sica por completo para que se acumule
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
            }

            // Desactivamos su collider para que otros ingredientes puedan caer encima sin rebotar
            Collider col = other.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Lo hacemos hijo del vaso para que si movemos el vaso, la fruta se mueva con �l
            other.transform.SetParent(this.transform);
        }
    }
}