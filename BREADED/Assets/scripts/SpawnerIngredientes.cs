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

    [Header("Estaciones y Puntos de Referencia")]
    [Tooltip("Índice de estación en NavegadorEstacion donde el ratón controla Y y Z (X fija)")]
    public int indiceEstacionToppings = 3;
    public Transform puntoDeCaidaPrincipal; // Estación normal (mueve en X e Y, Z fija)
    public Transform puntoDeCaidaToppings;  // Estación de toppings (mueve en Y y Z, X fija)
    public float alturaMaxima = 4f;

    private GameObject prefabIngredienteActual;
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
        return navegador != null
            && navegador.ObtenerEstacionActual() == indiceEstacionToppings
            && puntoDeCaidaToppings != null;
    }

    public void PrepararBowl(GameObject prefab)
    {
        prefabIngredienteActual = prefab;
    }

    public void DispararLluviaDeFrutas()
    {
        if (prefabIngredienteActual == null) return;

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
        if (prefabIngredienteActual == null) return;

        LanzarUnIngrediente();
        prefabIngredienteActual = null;
    }

    IEnumerator RutinaDispensado()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoDeDispensado)
        {
            LanzarUnIngrediente();
            yield return new WaitForSeconds(intervaloCaida);
            tiempoTranscurrido += intervaloCaida;
        }

        prefabIngredienteActual = null;
        rutinaActiva = null;
    }

    void LanzarUnIngrediente()
    {
        bool enToppings = EsEstacionToppings();
        Transform puntoActivo = enToppings ? puntoDeCaidaToppings : puntoDeCaidaPrincipal;

        if (prefabIngredienteActual == null || puntoActivo == null || camara == null || Mouse.current == null)
            return;

        Vector2 posMouse = Mouse.current.position.ReadValue();
        Ray rayo = camara.ScreenPointToRay(posMouse);

        Vector3 puntoSpawn = puntoActivo.position + Vector3.up * 1.5f;

        if (enToppings)
        {
            // Estación de Toppings: X fija en puntoDeCaidaToppings.x, ratón controla Y y Z
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
            // Estaciones principales: Z fija en puntoDeCaidaPrincipal.z, ratón controla X e Y
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
        GameObject instancia = Instantiate(prefabIngredienteActual, puntoSpawn, rotacionAleatoria);

        Rigidbody rb = instancia.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
        }
    }
}