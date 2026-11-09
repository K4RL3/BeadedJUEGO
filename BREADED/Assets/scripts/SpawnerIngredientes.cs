using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerIngredientes : MonoBehaviour
{
    [Header("Tiempos de Dispensado")]
    public float tiempoDeDispensado = 1f;
    public float intervaloCaida = 0.1f;

    [Header("Punto de Caída (Arrastra 'centro (1)' aquí)")]
    public Transform puntoDeCaida; // <--- Arrastra tu objeto "centro (1)" al Inspector

    [Header("Ajustes de Altura")]
    public float alturaSpawnY = 2.5f; // Altura desde donde caen

    private GameObject prefabFrutaActual;

    public void PrepararBowl(GameObject prefab)
    {
        prefabFrutaActual = prefab;
    }

    public void DispararLluviaDeFrutas()
    {
        if (prefabFrutaActual != null)
        {
            StartCoroutine(RutinaDispensado());
        }
    }

    IEnumerator RutinaDispensado()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoDeDispensado)
        {
            LanzarUnaFruta();
            yield return new WaitForSeconds(intervaloCaida);
            tiempoTranscurrido += intervaloCaida;
        }

        prefabFrutaActual = null;
    }

    void LanzarUnaFruta()
    {
        if (prefabFrutaActual == null || puntoDeCaida == null) return;

        // 1. Tomamos la posición exacta de tu objeto "centro (1)"
        Vector3 puntoSpawn = puntoDeCaida.position;

        // 2. Le sumamos la altura para que caiga desde arriba
        puntoSpawn.y = alturaSpawnY;

        // 3. Le damos un pequeño desvío aleatorio en X y Z para que no se apilen en una sola línea perfecta
        float desvioX = Random.Range(-0.2f, 0.2f);
        float desvioZ = Random.Range(-0.1f, 0.1f);
        puntoSpawn += new Vector3(desvioX, 0, desvioZ);

        // 4. Instanciamos
        Quaternion rotacionAleatoria = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        GameObject fruta = Instantiate(prefabFrutaActual, puntoSpawn, rotacionAleatoria);

        // 5. Física de caída
        Rigidbody rb = fruta.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
        }
    }
}