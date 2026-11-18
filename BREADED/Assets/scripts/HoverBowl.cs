using UnityEngine;
using UnityEngine.InputSystem;

public class HoverBowl : MonoBehaviour
{
    // Variables privadas: NO aparecen en el Inspector
    private float escalaHover = 1.1f;
    private float alturaHover = 0.2f;
    private float velocidadTransicion = 10f;

    private Vector3 escalaOriginal;
    private Vector3 posicionOriginal;
    private Vector3 escalaObjetivo;
    private Vector3 posicionObjetivo;
    private bool estaHover = false;

    private Camera camara;

    void Start()
    {
        camara = Camera.main;

        escalaOriginal = transform.localScale;
        posicionOriginal = transform.localPosition;

        escalaObjetivo = escalaOriginal;
        posicionObjetivo = posicionOriginal;
    }

    void Update()
    {
        Ray rayo = camara.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            if (golpe.transform == transform || golpe.transform.IsChildOf(transform))
            {
                if (!estaHover)
                {
                    estaHover = true;
                    escalaObjetivo = escalaOriginal * escalaHover;
                    posicionObjetivo = posicionOriginal + new Vector3(0, alturaHover, 0);

                    // 🎵 Usamos el ManejadorAudio global.
                    // Si tienes un sonido de Hover, usa PlayHover(). 
                    // Si no, puedes usar PlaySeleccion() para que suene un clic suave.
                    if (ManejadorAudio.Instance != null)
                        ManejadorAudio.Instance.PlaySeleccion();
                }
            }
            else
            {
                QuitarHover();
            }
        }
        else
        {
            QuitarHover();
        }

        transform.localScale = Vector3.Lerp(transform.localScale, escalaObjetivo, Time.deltaTime * velocidadTransicion);
        transform.localPosition = Vector3.Lerp(transform.localPosition, posicionObjetivo, Time.deltaTime * velocidadTransicion);
    }

    void QuitarHover()
    {
        if (estaHover)
        {
            estaHover = false;
            escalaObjetivo = escalaOriginal;
            posicionObjetivo = posicionOriginal;
        }
    }
}