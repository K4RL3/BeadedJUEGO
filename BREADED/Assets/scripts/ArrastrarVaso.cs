using UnityEngine;
using UnityEngine.InputSystem;

public class ArrastrarVaso : MonoBehaviour
{
    private Camera camara;
    private bool estaSiendoArrastrado = false;
    private Vector3 posicionOriginal;

    private bool estaMoviendose = false;
    private Vector3 destinoMovimiento;
    private Transform nuevoPadreDestino;
    private float velocidadMovimiento = 5f;

    public float alturaMinimaY = 0.5f;

    [HideInInspector] public bool procesadoPorBoton = false;

    private Vector3 escalaOriginalVaso;

    void Start()
    {
        camara = Camera.main;
        posicionOriginal = transform.position;
        escalaOriginalVaso = transform.localScale; // Guardamos la escala original
    }

    void Update()
    {
        // 1. Lógica de Arrastre con Mouse
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray rayo = camara.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
            {
                if (golpe.transform == transform || golpe.transform.IsChildOf(transform))
                {
                    NavegadorEstacion nav = FindAnyObjectByType<NavegadorEstacion>();
                    if (nav != null && nav.ObtenerEstacionActual() != 0)
                    {
                        estaSiendoArrastrado = true;
                        transform.position += Vector3.up * 0.5f;
                        procesadoPorBoton = false;

                        transform.SetParent(null);
                    }
                }
            }
        }

        // Soltar el mouse
        if (Mouse.current.leftButton.wasReleasedThisFrame && estaSiendoArrastrado)
        {
            estaSiendoArrastrado = false;
            if (!procesadoPorBoton) RegresarAlInicio();
        }

        // Mover con el mouse
        if (estaSiendoArrastrado)
        {
            Vector2 posMouse = Mouse.current.position.ReadValue();
            Vector3 posMundo = camara.ScreenToWorldPoint(new Vector3(posMouse.x, posMouse.y, Mathf.Abs(camara.transform.position.z - posicionOriginal.z)));

            float nuevaY = Mathf.Max(posMundo.y, alturaMinimaY);
            transform.position = new Vector3(posMundo.x, nuevaY, posicionOriginal.z);
        }

        // 2. Lógica de Movimiento Recto
        if (estaMoviendose)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destinoMovimiento,
                velocidadMovimiento * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, destinoMovimiento) < 0.01f)
            {
                transform.position = destinoMovimiento;
                estaMoviendose = false;

                // ¡AQUÍ ESTÁ EL TRUCO PARA QUE NO DESAPAREZCA!
                if (nuevoPadreDestino != null)
                {
                    transform.SetParent(nuevoPadreDestino);

                    // En lugar de localPosition = 0, forzamos la posición del mundo exacta
                    transform.position = nuevoPadreDestino.position;

                    // Restauramos su escala para que no se deforme
                    transform.localScale = escalaOriginalVaso;
                }

                posicionOriginal = destinoMovimiento;
                Debug.Log("Vaso llegó a su destino");
            }
        }
    }

    void RegresarAlInicio()
    {
        if (!estaSiendoArrastrado && !procesadoPorBoton)
        {
            transform.position = posicionOriginal;
        }
    }

    public void MoverAEstacion(Transform nuevoWorkSpot)
    {
        procesadoPorBoton = true;
        estaSiendoArrastrado = false;
        estaMoviendose = true;

        nuevoPadreDestino = nuevoWorkSpot;

        // Guardamos la posición del WorkSpot antes de que el vaso se mueva
        destinoMovimiento = nuevoWorkSpot.position;

        transform.SetParent(null);
    }
    // NUEVA FUNCIÓN: Para decirle al vaso cuál es su nuevo hogar
    public void ActualizarPosicionBase()
    {
        posicionOriginal = transform.position;
    }
}