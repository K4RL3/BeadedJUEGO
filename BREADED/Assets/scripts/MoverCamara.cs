using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform camara;
    public Transform camPos_Mesa;
    public Transform camPos_Toppings;
    public float duracionMovimiento = .5f;

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private float tiempoTranscurrido = 0f;
    private bool moviendo = false;

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