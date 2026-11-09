using UnityEngine;
using UnityEngine.InputSystem;

public class Vasos : MonoBehaviour
{
    [Header("Referencias")]
    public Transform vasoChico;
    public Transform vasoGrande;
    public Transform workSpot_Toppings;
    public GameObject prefabVasoChico;
    public GameObject prefabVasoGrande;

    [Header("Ruta del Vaso (Bézier)")]
    public Transform puntoControl1;
    public Transform puntoControl2;

    [Header("Ajustes")]
    public float escalaFinal = 1.5f;
    public float velocidadMovimiento = 3f;

    public static bool vasoYaElegido = false;
    public static bool eraChico = false;

    Transform vasoElegido;
    Transform ultimoHover;
    bool yaEligio;
    bool terminocrecimiento;
    bool llegoAToppings;
    bool vasoEsChico;
    float escalaOriginalChico;
    float escalaOriginalGrande;
    float escalaOriginalElegida;
    Vector3 posicionOriginalElegida;
    float tiempoCrecimiento;

    GameObject reemplazoActual;
    Vector3 posicionDestinoReemplazo;
    bool reemplazoCayendo;

    float tiempoDeViaje = 0f;
    float duracionViaje = 1f;

    void Start()
    {
        escalaOriginalChico = vasoChico.localScale.x;
        escalaOriginalGrande = vasoGrande.localScale.x;

        // Si venimos de la cocina y ya hay un vaso elegido, lo restauramos
        if (vasoYaElegido)
        {
            SpawnearVasoGuardado();
        }
    }

    void SpawnearVasoGuardado()
    {
        GameObject prefab = eraChico ? prefabVasoChico : prefabVasoGrande;
        GameObject vaso = Instantiate(prefab, workSpot_Toppings.position, workSpot_Toppings.rotation);

        float escala = eraChico ? escalaOriginalChico : escalaOriginalGrande;

        vaso.transform.SetParent(workSpot_Toppings);
        vaso.transform.localPosition = Vector3.zero;
        vaso.transform.localRotation = Quaternion.identity;
        vaso.transform.localScale = Vector3.one * escala;

        // Bloqueamos la selección porque ya hay un vaso en la mesa
        yaEligio = true;
        terminocrecimiento = true;
        llegoAToppings = true;
    }

    void Update()
    {
        // SI NO HAY VASO ELEGIDO, PERMITIMOS HOVER Y CLIC
        if (!yaEligio)
        {
            DetectarHover();
            DetectarClick();
        }
        // SI YA SE ELIGIÓ, SE MUEVE
        else if (!terminocrecimiento)
        {
            Crecer();
        }
        else
        {
            if (reemplazoCayendo) MoverReemplazo();
            if (!llegoAToppings) MoverAToppingsCurva();
        }
    }

    void DetectarHover()
    {
        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            // Usamos IsChildOf para detectar si tocamos el vaso o alguno de sus hijos (como el Mesh Collider)
            bool tocoChico = golpe.transform == vasoChico || golpe.transform.IsChildOf(vasoChico);
            bool tocoGrande = golpe.transform == vasoGrande || golpe.transform.IsChildOf(vasoGrande);

            if (tocoChico)
            {
                if (ultimoHover != vasoChico && ManejadorAudio.Instance != null)
                    ManejadorAudio.Instance.PlayHover();
                ultimoHover = vasoChico;

                vasoChico.localScale = Vector3.one * (escalaOriginalChico * 1.1f);
                vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            }
            else if (tocoGrande)
            {
                if (ultimoHover != vasoGrande && ManejadorAudio.Instance != null)
                    ManejadorAudio.Instance.PlayHover();
                ultimoHover = vasoGrande;

                vasoGrande.localScale = Vector3.one * (escalaOriginalGrande * 1.1f);
                vasoChico.localScale = Vector3.one * escalaOriginalChico;
            }
            else
            {
                ultimoHover = null;
                vasoChico.localScale = Vector3.one * escalaOriginalChico;
                vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            }
        }
    }

    void DetectarClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            bool tocoChico = golpe.transform == vasoChico || golpe.transform.IsChildOf(vasoChico);
            bool tocoGrande = golpe.transform == vasoGrande || golpe.transform.IsChildOf(vasoGrande);

            if (tocoChico)
            {
                vasoElegido = vasoChico;
                vasoEsChico = true;
                escalaOriginalElegida = escalaOriginalChico;
                posicionOriginalElegida = vasoChico.position;
                yaEligio = true;
                tiempoCrecimiento = 0f;

                vasoYaElegido = true;
                eraChico = true;

                if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlaySeleccion();
                if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayCrecimiento();

                Debug.Log("Elegiste el vaso CHICO");
            }
            else if (tocoGrande)
            {
                vasoElegido = vasoGrande;
                vasoEsChico = false;
                escalaOriginalElegida = escalaOriginalGrande;
                posicionOriginalElegida = vasoGrande.position;
                yaEligio = true;
                tiempoCrecimiento = 0f;

                vasoYaElegido = true;
                eraChico = false;

                if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlaySeleccion();
                if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayCrecimiento();

                Debug.Log("Elegiste el vaso GRANDE");
            }
        }
    }

    void Crecer()
    {
        tiempoCrecimiento += Time.deltaTime;
        float t = tiempoCrecimiento / 0.3f;

        if (t >= 1f)
        {
            vasoElegido.localScale = Vector3.one * (escalaOriginalElegida * escalaFinal);
            terminocrecimiento = true;
            SpawnReplace();
            return;
        }

        float suavizado = Mathf.SmoothStep(0f, 1f, t);
        float escalaActual = Mathf.Lerp(escalaOriginalElegida, escalaOriginalElegida * escalaFinal, suavizado);
        vasoElegido.localScale = Vector3.one * escalaActual;
    }

    void SpawnReplace()
    {
        GameObject prefab = vasoEsChico ? prefabVasoChico : prefabVasoGrande;

        Vector3 spawnPos = posicionOriginalElegida + Vector3.up * 3f;
        reemplazoActual = Instantiate(prefab, spawnPos, Quaternion.identity);
        reemplazoActual.transform.localScale = Vector3.one * escalaOriginalElegida;

        posicionDestinoReemplazo = posicionOriginalElegida;
        reemplazoCayendo = true;
    }

    void MoverReemplazo()
    {
        if (reemplazoActual == null)
        {
            reemplazoCayendo = false;
            return;
        }

        reemplazoActual.transform.position = Vector3.MoveTowards(
            reemplazoActual.transform.position,
            posicionDestinoReemplazo,
            velocidadMovimiento * Time.deltaTime
        );

        float distancia = Vector3.Distance(reemplazoActual.transform.position, posicionDestinoReemplazo);

        if (distancia < 0.01f)
        {
            reemplazoActual.transform.position = posicionDestinoReemplazo;
            reemplazoCayendo = false;

            if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayCaidaVaso();
        }
    }

    void MoverAToppingsCurva()
    {
        if (puntoControl1 == null && puntoControl2 == null)
        {
            MoverEnLineaRecta(workSpot_Toppings.position);
            return;
        }

        float distanciaTotal = Vector3.Distance(posicionOriginalElegida, workSpot_Toppings.position);
        duracionViaje = distanciaTotal / velocidadMovimiento;

        tiempoDeViaje += Time.deltaTime;
        float t = tiempoDeViaje / duracionViaje;

        if (t >= 1f)
        {
            MoverEnLineaRecta(workSpot_Toppings.position);
            return;
        }

        Vector3 p0 = posicionOriginalElegida;
        Vector3 p3 = workSpot_Toppings.position;

        if (puntoControl1 != null && puntoControl2 == null)
        {
            Vector3 p1 = puntoControl1.position;
            vasoElegido.position = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p3;
        }
        else if (puntoControl1 != null && puntoControl2 != null)
        {
            Vector3 p1 = puntoControl1.position;
            Vector3 p2 = puntoControl2.position;

            vasoElegido.position = Mathf.Pow(1 - t, 3) * p0 +
                                   3 * Mathf.Pow(1 - t, 2) * t * p1 +
                                   3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                                   Mathf.Pow(t, 3) * p3;
        }
    }

    void MoverEnLineaRecta(Vector3 destino)
    {
        vasoElegido.position = Vector3.MoveTowards(
            vasoElegido.position,
            destino,
            velocidadMovimiento * Time.deltaTime
        );

        if (Vector3.Distance(vasoElegido.position, destino) < 0.01f)
        {
            vasoElegido.position = destino;
            vasoElegido.rotation = workSpot_Toppings.rotation;

            vasoElegido.SetParent(workSpot_Toppings);
            vasoElegido.localPosition = Vector3.zero;
            vasoElegido.localRotation = Quaternion.identity;
            vasoElegido.localScale = Vector3.one * escalaOriginalElegida;

            llegoAToppings = true;
            Debug.Log("¡Vaso llegó a la mesa de toppings!");
        }
    }
}