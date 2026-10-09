using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform camara;

    public Transform camPos_Mesa;
    public Transform camPos_Toppings;

    public float velocidadCamara = 6f;

    private Vector3 destino;
    private bool moviendo = false;

    public void MoverAToppings()
    {
        destino = camPos_Toppings.position;
        moviendo = true;
        Debug.Log("Cámara viajando a toppings...");
    }

    public void MoverAMesa()
    {
        destino = camPos_Mesa.position;
        moviendo = true;
        Debug.Log("Cámara viajando a la mesa...");
    }

    void Update()
    {
        if (!moviendo) return;

        camara.position = Vector3.MoveTowards(
            camara.position,
            destino,
            velocidadCamara * Time.deltaTime
        );

        float distancia = Vector3.Distance(camara.position, destino);

        if (distancia < 0.01f)
        {
            camara.position = destino;
            moviendo = false;
            Debug.Log("¡Cámara llegó a su destino!");
        }
    }
}
