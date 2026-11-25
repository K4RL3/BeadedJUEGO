using UnityEngine;
using UnityEngine.InputSystem;

public class ArrastrarVaso : MonoBehaviour
{
    private enum EjeArrastre
    {
        Ninguno,
        EjeX,
        EjeZ
    }

    public bool esVasoSeleccionado = false;
    public bool ocultarNoSeleccionadosAlAvanzar = false;

    [HideInInspector] public bool procesadoPorBoton = false;

    private Camera camara;
    private NavegadorEstacion navegador;

    private bool estaSiendoArrastrado = false;
    private Vector3 posicionOriginal;
    private Vector3 posicionInicioArrastre;
    private Vector3 offsetArrastre;
    private float alturaPlanoArrastre;
    private EjeArrastre ejeActual = EjeArrastre.Ninguno;

    private float offsetAlturaLevantado = 0.5f;
    private float velocidadMovimiento = 5f;

    private bool estaMoviendose = false;
    private Vector3 destinoMovimiento;
    private Transform nuevoPadreDestino;
    private Vector3 escalaOriginalVaso;

    void Start()
    {
        camara = Camera.main;
        navegador = FindAnyObjectByType<NavegadorEstacion>();
        posicionOriginal = transform.position;
        escalaOriginalVaso = transform.localScale;
    }

    public void SeleccionarEsteVaso()
    {
        ArrastrarVaso[] todosLosVasos = FindObjectsByType<ArrastrarVaso>();
        for (int i = 0; i < todosLosVasos.Length; i++)
        {
            todosLosVasos[i].esVasoSeleccionado = (todosLosVasos[i] == this);
        }
    }

    private EjeArrastre ObtenerEjePorEstacion(int estacion)
    {
        switch (estacion)
        {
            case 1:
            case 3:
                return EjeArrastre.EjeX;
            case 2:
                return EjeArrastre.EjeZ;
            case 0:
            default:
                return EjeArrastre.Ninguno;
        }
    }

    private ZonaBotonUI BuscarZonaSobreCursor(Vector2 posicionPantalla)
    {
        ZonaBotonUI[] zonas = FindObjectsByType<ZonaBotonUI>();
        for (int i = 0; i < zonas.Length; i++)
        {
            if (zonas[i] != null && zonas[i].EstaSobreZona(posicionPantalla))
            {
                return zonas[i];
            }
        }
        return null;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // 1. Iniciar arrastre con clic izquierdo
        if (Mouse.current.leftButton.wasPressedThisFrame && !estaMoviendose)
        {
            Ray rayo = camara.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
            {
                if (golpe.transform == transform || golpe.transform.IsChildOf(transform))
                {
                    int estacionIdx = navegador != null ? navegador.ObtenerEstacionActual() : 0;
                    EjeArrastre ejePermitido = ObtenerEjePorEstacion(estacionIdx);

                    if (ejePermitido != EjeArrastre.Ninguno)
                    {
                        SeleccionarEsteVaso();
                        ejeActual = ejePermitido;

                        estaSiendoArrastrado = true;
                        procesadoPorBoton = false;
                        transform.SetParent(null);

                        transform.position += Vector3.up * offsetAlturaLevantado;
                        posicionInicioArrastre = transform.position;
                        alturaPlanoArrastre = transform.position.y;

                        Plane planoHorizontal = new Plane(Vector3.up, new Vector3(0f, alturaPlanoArrastre, 0f));
                        if (planoHorizontal.Raycast(rayo, out float distancia))
                        {
                            Vector3 puntoEnPlano = rayo.GetPoint(distancia);
                            offsetArrastre = transform.position - puntoEnPlano;
                            offsetArrastre.y = 0f;
                        }
                        else
                        {
                            offsetArrastre = Vector3.zero;
                        }
                    }
                }
            }
        }

        // 2. Arrastre en el eje de la estación actual
        if (estaSiendoArrastrado)
        {
            Ray rayoHorizontal = camara.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane planoHorizontal = new Plane(Vector3.up, new Vector3(0f, alturaPlanoArrastre, 0f));

            if (planoHorizontal.Raycast(rayoHorizontal, out float distHor))
            {
                Vector3 puntoEnPlano = rayoHorizontal.GetPoint(distHor) + offsetArrastre;
                Vector3 nuevaPos = posicionInicioArrastre;

                if (ejeActual == EjeArrastre.EjeX)
                {
                    nuevaPos.x = puntoEnPlano.x;
                }
                else if (ejeActual == EjeArrastre.EjeZ)
                {
                    nuevaPos.z = puntoEnPlano.z;
                }

                nuevaPos.y = alturaPlanoArrastre;
                transform.position = nuevaPos;
            }
        }

        // 3. Soltar mouse
        if (Mouse.current.leftButton.wasReleasedThisFrame && estaSiendoArrastrado)
        {
            estaSiendoArrastrado = false;
            ejeActual = EjeArrastre.Ninguno;

            ZonaBotonUI zonaDestino = BuscarZonaSobreCursor(Mouse.current.position.ReadValue());
            if (zonaDestino != null)
            {
                zonaDestino.EjecutarAccionDesdeArrastre(this);
                return;
            }

            if (!procesadoPorBoton)
            {
                RegresarAlInicio();
            }
        }

        // 4. Movimiento recto hacia la estación (palomita verde)
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

                if (nuevoPadreDestino != null)
                {
                    transform.SetParent(nuevoPadreDestino);
                    transform.position = nuevoPadreDestino.position;
                    transform.localScale = escalaOriginalVaso;
                }

                posicionOriginal = transform.position;
                Debug.Log("Vaso llegó a su destino: " + gameObject.name);
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

    public void MoverAEstacion(Transform nuevoWorkSpot, bool forzarMovimiento = false)
    {
        if (!esVasoSeleccionado && !forzarMovimiento)
        {
            if (ocultarNoSeleccionadosAlAvanzar)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        procesadoPorBoton = true;
        estaSiendoArrastrado = false;
        ejeActual = EjeArrastre.Ninguno;
        estaMoviendose = true;

        nuevoPadreDestino = nuevoWorkSpot;
        destinoMovimiento = nuevoWorkSpot.position;

        transform.SetParent(null);
    }

    public void ActualizarPosicionBase()
    {
        posicionOriginal = transform.position;
    }
}