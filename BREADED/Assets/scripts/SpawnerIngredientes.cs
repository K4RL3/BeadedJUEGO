using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerIngredientes : MonoBehaviour
{
    [Header("Tiempos de Dispensado")]
    public float tiempoDeDispensado = 1f;
    public float intervaloCaida = 0.1f;

    [Header("Punto de Referencia (Arrastra 'centro (1)' aquí)")]
    public Transform puntoDeCaida;
    public float alturaMaxima = 4f; // Altura máxima si mueves el mouse muy arriba

    private GameObject prefabFrutaActual;
    private Camera camara;

    void Start()
    {
        camara = Camera.main;
    }

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

        // 1. Obtenemos la posición del mouse en pantalla (de 0 a Screen.width y 0 a Screen.height)
        Vector2 posMouse = Mouse.current.position.ReadValue();

        // 2. Proyectamos esa posición a coordenadas del mundo. 
        // Le pasamos la Z de tu 'centro (1)' para que respete la profundidad.
        Vector3 posMundo = camara.ScreenToWorldPoint(new Vector3(posMouse.x, posMouse.y, Mathf.Abs(camara.transform.position.z - puntoDeCaida.position.z)));

        // 3. Armamos la posición final:
        // - X: Donde está el mouse
        // - Y: La altura del mouse (limitada a alturaMaxima)
        // - Z: La Z exacta de tu 'centro (1)'
        float posX = posMundo.x;
        float posY = Mathf.Clamp(posMundo.y, puntoDeCaida.position.y + 1f, alturaMaxima);
        float posZ = puntoDeCaida.position.z;

        Vector3 puntoSpawn = new Vector3(posX, posY, posZ);

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