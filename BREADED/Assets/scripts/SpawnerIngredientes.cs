using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerIngredientes : MonoBehaviour
{
    [Header("Tiempos de Dispensado")]
    public float tiempoDeDispensado = 1f;
    public float intervaloCaida = 0.1f;

    [Header("Ajustes de Caída")]
    public float alturaSpawnY = 0.5f; // Altura desde donde caen
    public float distanciaCamaraZ = 1f; // Qué tan lejos (en Z) está el plano de juego

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
        if (prefabFrutaActual == null) return;

        // 1. Obtenemos la posición del mouse en la pantalla
        Vector3 posMousePantalla = Mouse.current.position.ReadValue();

        // 2. Le decimos a Unity a qué distancia de la cámara está el "plano" de nuestra mesa
        posMousePantalla.z = distanciaCamaraZ;

        // 3. Convertimos esa posición de pantalla a posición del mundo real
        Vector3 posicionMundo = camara.ScreenToWorldPoint(posMousePantalla);

        // 4. Armamos la posición final. 
        // IMPORTANTE: Mantenemos el Z de la cámara para que no se vaya al fondo.
        // Usamos posicionMundo.x (que es donde está tu mouse)
        Vector3 puntoSpawn = new Vector3(posicionMundo.x, alturaSpawnY, posicionMundo.z);

        // 5. Instanciamos
        Quaternion rotacionAleatoria = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        GameObject fruta = Instantiate(prefabFrutaActual, puntoSpawn, rotacionAleatoria);

        // 6. Física de caída
        Rigidbody rb = fruta.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
        }
    }
}