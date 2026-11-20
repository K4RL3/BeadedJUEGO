using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerIngredientes : MonoBehaviour
{
    [Header("Modo de Dispensado")]
    public bool soloUnoPorVez = false;

    [Header("Tiempos de Dispensado (Modo Lluvia)")]
    public float tiempoDeDispensado = 1f;
    public float intervaloCaida = 0.1f;

    [Header("Puntos de Referencia")]
    public Transform puntoDeCaida;          // Estación 1 (mueve en X, Z fijo)
    public Transform puntoDeCaidaToppings;  // Estación 2 (mueve en Z, X fijo)
    public float alturaMaxima = 4f;

    private GameObject prefabFrutaActual;
    private Camera camara;
    private NavegadorEstacion navegador;
    private Coroutine rutinaActiva;

    void Start()
    {
        camara = Camera.main;
        navegador = FindAnyObjectByType<NavegadorEstacion>();
    }

    private bool EsEstacionToppings()
    {
        return navegador != null && navegador.ObtenerEstacionActual() == 2 && puntoDeCaidaToppings != null;
    }

    public void PrepararBowl(GameObject prefab)
    {
        prefabFrutaActual = prefab;
    }

    public void DispararLluviaDeFrutas()
    {
        if (prefabFrutaActual == null) return;

        if (soloUnoPorVez)
        {
            DispararUnoSolo();
            return;
        }

        if (rutinaActiva != null)
        {
            StopCoroutine(rutinaActiva);
        }
        rutinaActiva = StartCoroutine(RutinaDispensado());
    }

    public void DispararUnoSolo()
    {
        if (prefabFrutaActual == null) return;

        LanzarUnaFruta();
        prefabFrutaActual = null;
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
        rutinaActiva = null;
    }

    void LanzarUnaFruta()
    {
        bool enToppings = EsEstacionToppings();
        Transform puntoActivo = enToppings ? puntoDeCaidaToppings : puntoDeCaida;

        if (prefabFrutaActual == null || puntoActivo == null || camara == null || Mouse.current == null)
            return;

        Vector2 posMouse = Mouse.current.position.ReadValue();
        Ray rayo = camara.ScreenPointToRay(posMouse);

        Vector3 puntoSpawn = puntoActivo.position + Vector3.up * 1.5f;

        if (enToppings)
        {
            // Estación 2 (Toppings): X fija en puntoDeCaidaToppings.x, ratón controla Y y Z
            Plane planoLateral = new Plane(Vector3.right, puntoActivo.position);
            if (planoLateral.Raycast(rayo, out float distancia))
            {
                Vector3 impacto = rayo.GetPoint(distancia);
                float posY = Mathf.Clamp(impacto.y, puntoActivo.position.y + 1f, alturaMaxima);
                puntoSpawn = new Vector3(puntoActivo.position.x, posY, impacto.z);
            }
        }
        else
        {
            // Estación 1 (Armado): Z fija en puntoDeCaida.z, ratón controla X e Y
            Plane planoFrontal = new Plane(Vector3.forward, puntoActivo.position);
            if (planoFrontal.Raycast(rayo, out float distancia))
            {
                Vector3 impacto = rayo.GetPoint(distancia);
                float posY = Mathf.Clamp(impacto.y, puntoActivo.position.y + 1f, alturaMaxima);
                puntoSpawn = new Vector3(impacto.x, posY, puntoActivo.position.z);
            }
        }

        Quaternion rotacionAleatoria = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );
        GameObject fruta = Instantiate(prefabFrutaActual, puntoSpawn, rotacionAleatoria);

        Rigidbody rb = fruta.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
        }
    }
}