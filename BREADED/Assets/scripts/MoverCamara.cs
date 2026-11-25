using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform camPos_Mesa;
    public Transform camPos_Toppings;
    public float duracionMovimiento = 0.8f;

    Transform camara;
    Vector3 posicionInicial;
    Vector3 posicionFinal;
    Quaternion rotacionInicial;
    Quaternion rotacionFinal;
    float tiempoTranscurrido = 0f;
    bool moviendo = false;

    // Guardamos la rotación original de la cámara al iniciar
    private Quaternion rotacionBaseCamara;
    private NavegadorEstacion navegador;

    void Awake()
    {
        camara = Camera.main.transform;
        // Guardamos el ángulo perfecto que ya tienes configurado
        rotacionBaseCamara = camara.rotation;
        navegador = GetComponent<NavegadorEstacion>();
    }

    public void MoverAToppings() => IniciarMovimiento(camPos_Toppings);
    public void MoverAMesa() => IniciarMovimiento(camPos_Mesa);
    public void MoverAEstacion(Transform destino) => IniciarMovimiento(destino);

    void IniciarMovimiento(Transform destino)
    {
        posicionInicial = camara.position;
        posicionFinal = destino.position;

        rotacionInicial = camara.rotation;

        int estacionIdx = navegador != null ? navegador.ObtenerEstacionActual() : 0;

        // 0 (Vasos) y 1 (Fruta): en línea, sin girar (mismo Y base)
        // 2 (Crema): +90° a la derecha
        // 3 (Toppings): otros +90° a la derecha (+180° desde el inicio)
        float giroAdicionalY = 0f;
        if (estacionIdx == 2)
        {
            giroAdicionalY = 90f;
        }
        else if (estacionIdx == 3)
        {
            giroAdicionalY = 180f;
        }

        float anguloY = rotacionBaseCamara.eulerAngles.y + giroAdicionalY;

        // Tomamos los ángulos X y Z originales de la cámara para no perder la inclinación
        float anguloXOriginal = rotacionBaseCamara.eulerAngles.x;
        float anguloZOriginal = rotacionBaseCamara.eulerAngles.z;

        rotacionFinal = Quaternion.Euler(anguloXOriginal, anguloY, anguloZOriginal);

        tiempoTranscurrido = 0f;
        moviendo = true;
    }

    public bool EstaMoviendo() => moviendo;

    void Update()
    {
        if (!moviendo) return;

        tiempoTranscurrido += Time.deltaTime;
        float t = tiempoTranscurrido / duracionMovimiento;

        if (t >= 1f)
        {
            camara.position = posicionFinal;
            camara.rotation = rotacionFinal;
            moviendo = false;
            return;
        }

        float suavizado = Mathf.SmoothStep(0f, 1f, t);

        camara.position = Vector3.Lerp(posicionInicial, posicionFinal, suavizado);
        // Usamos Slerp para rotar suavemente hacia el nuevo ángulo
        camara.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, suavizado);
    }
}