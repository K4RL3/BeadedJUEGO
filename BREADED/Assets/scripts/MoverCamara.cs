using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform camPos_Mesa;
    public Transform camPos_Toppings;
    public float duracionMovimiento = 0.8f;

    Transform camara;
    Vector3 posicionInicial;
    Vector3 posicionFinal;
    float tiempoTranscurrido = 0f;
    bool moviendo = false;

    void Start()
    {
        camara = Camera.main.transform;
    }

    public void MoverAToppings()
    {
        IniciarMovimiento(camPos_Toppings.position);
    }

    public void MoverAMesa()
    {
        IniciarMovimiento(camPos_Mesa.position);
    }

    public void MoverAEstacion(Transform destino)
    {
        IniciarMovimiento(destino.position);
    }

    void IniciarMovimiento(Vector3 destino)
    {
        posicionInicial = camara.position;
        posicionFinal = destino;
        tiempoTranscurrido = 0f;
        moviendo = true;
    }

    public bool EstaMoviendo()
    {
        return moviendo;
    }

    void Update()
    {
        if (!moviendo) return;

        tiempoTranscurrido += Time.deltaTime;
        float t = tiempoTranscurrido / duracionMovimiento;

        if (t >= 1f)
        {
            camara.position = posicionFinal;
            moviendo = false;
            return;
        }

        float suavizado = Mathf.SmoothStep(0f, 1f, t);
        camara.position = Vector3.Lerp(posicionInicial, posicionFinal, suavizado);
    }
}