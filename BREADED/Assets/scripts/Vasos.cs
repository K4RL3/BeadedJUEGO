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

    Vector3 posInicialChico;
    Vector3 posInicialGrande;
    Quaternion rotInicialChico;
    Quaternion rotInicialGrande;
    Transform padreInicialChico;
    Transform padreInicialGrande;

    Vector3 posicionOriginalElegida;
    float tiempoCrecimiento;

    GameObject reemplazoActual;
    GameObject vasoSpawneadoActual;
    Vector3 posicionDestinoReemplazo;
    bool reemplazoCayendo;

    float tiempoDeViaje = 0f;
    float duracionViaje = 1f;

    void Start()
    {
        if (vasoChico != null)
        {
            escalaOriginalChico = vasoChico.localScale.x;
            posInicialChico = vasoChico.position;
            rotInicialChico = vasoChico.rotation;
            padreInicialChico = vasoChico.parent;
            ActivarColliders(vasoChico);
        }

        if (vasoGrande != null)
        {
            escalaOriginalGrande = vasoGrande.localScale.x;
            posInicialGrande = vasoGrande.position;
            rotInicialGrande = vasoGrande.rotation;
            padreInicialGrande = vasoGrande.parent;
            ActivarColliders(vasoGrande);
        }

        Physics.SyncTransforms();

        if (vasoYaElegido)
        {
            SpawnearVasoGuardado();
        }
    }

    void ActivarColliders(Transform objetivo)
    {
        if (objetivo == null) return;

        Collider[] cols = objetivo.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] != null)
                cols[i].enabled = true;
        }
    }

    void SpawnearVasoGuardado()
    {
        if (vasoSpawneadoActual != null)
        {
            Destroy(vasoSpawneadoActual);
        }

        GameObject prefab = eraChico ? prefabVasoChico : prefabVasoGrande;
        if (prefab == null || workSpot_Toppings == null) return;

        GameObject vaso = Instantiate(prefab, workSpot_Toppings.position, workSpot_Toppings.rotation);
        vasoSpawneadoActual = vaso;

        float escala = eraChico ? escalaOriginalChico : escalaOriginalGrande;

        vaso.transform.SetParent(workSpot_Toppings);
        vaso.transform.localPosition = Vector3.zero;
        vaso.transform.localRotation = Quaternion.identity;
        vaso.transform.localScale = Vector3.one * escala;
        ActivarColliders(vaso.transform);
        Physics.SyncTransforms();

        yaEligio = true;
        terminocrecimiento = true;
        llegoAToppings = true;
    }

    void Update()
    {
        if (!yaEligio)
        {
            DetectarHover();
            DetectarClick();
        }
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
        if (Mouse.current == null || Camera.main == null) return;

        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            bool tocoChico = vasoChico != null && vasoChico.gameObject.activeInHierarchy &&
                             (golpe.transform == vasoChico || golpe.transform.IsChildOf(vasoChico));
            bool tocoGrande = vasoGrande != null && vasoGrande.gameObject.activeInHierarchy &&
                              (golpe.transform == vasoGrande || golpe.transform.IsChildOf(vasoGrande));

            if (tocoChico)
            {
                if (ultimoHover != vasoChico && ManejadorAudio.Instance != null)
                    ManejadorAudio.Instance.PlayHover();
                ultimoHover = vasoChico;

                vasoChico.localScale = Vector3.one * (escalaOriginalChico * 1.1f);
                if (vasoGrande != null)
                    vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            }
            else if (tocoGrande)
            {
                if (ultimoHover != vasoGrande && ManejadorAudio.Instance != null)
                    ManejadorAudio.Instance.PlayHover();
                ultimoHover = vasoGrande;

                vasoGrande.localScale = Vector3.one * (escalaOriginalGrande * 1.1f);
                if (vasoChico != null)
                    vasoChico.localScale = Vector3.one * escalaOriginalChico;
            }
            else
            {
                ultimoHover = null;
                if (vasoChico != null) vasoChico.localScale = Vector3.one * escalaOriginalChico;
                if (vasoGrande != null) vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            }
        }
    }

    void DetectarClick()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || Camera.main == null)
            return;

        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            bool tocoChico = vasoChico != null && vasoChico.gameObject.activeInHierarchy &&
                             (golpe.transform == vasoChico || golpe.transform.IsChildOf(vasoChico));
            bool tocoGrande = vasoGrande != null && vasoGrande.gameObject.activeInHierarchy &&
                              (golpe.transform == vasoGrande || golpe.transform.IsChildOf(vasoGrande));

            if (tocoChico)
            {
                vasoElegido = vasoChico;
                vasoEsChico = true;
                escalaOriginalElegida = escalaOriginalChico;
                posicionOriginalElegida = posInicialChico;
                IniciarSeleccion();
                Debug.Log("Elegiste el vaso CHICO");
            }
            else if (tocoGrande)
            {
                vasoElegido = vasoGrande;
                vasoEsChico = false;
                escalaOriginalElegida = escalaOriginalGrande;
                posicionOriginalElegida = posInicialGrande;
                IniciarSeleccion();
                Debug.Log("Elegiste el vaso GRANDE");
            }
        }
    }

    void IniciarSeleccion()
    {
        tiempoDeViaje = 0f;
        tiempoCrecimiento = 0f;
        yaEligio = true;
        terminocrecimiento = false;
        llegoAToppings = false;

        vasoYaElegido = true;
        eraChico = vasoEsChico;

        ActivarColliders(vasoElegido);
        Physics.SyncTransforms();

        ArrastrarVaso arr = vasoElegido != null ? vasoElegido.GetComponent<ArrastrarVaso>() : null;
        if (arr != null) arr.SeleccionarEsteVaso();

        if (ManejadorAudio.Instance != null)
        {
            ManejadorAudio.Instance.PlaySeleccion();
            ManejadorAudio.Instance.PlayCrecimiento();
        }
    }

    void Crecer()
    {
        if (vasoElegido == null)
        {
            terminocrecimiento = true;
            return;
        }

        tiempoCrecimiento += Time.deltaTime;
        float t = tiempoCrecimiento / 0.3f;

        if (t >= 1f)
        {
            vasoElegido.localScale = Vector3.one * (escalaOriginalElegida * escalaFinal);
            Physics.SyncTransforms();
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
        if (prefab == null) return;

        Vector3 spawnPos = posicionOriginalElegida + Vector3.up * 3f;
        reemplazoActual = Instantiate(prefab, spawnPos, Quaternion.identity);
        reemplazoActual.transform.localScale = Vector3.one * escalaOriginalElegida;
        ActivarColliders(reemplazoActual.transform);
        Physics.SyncTransforms();

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
            Physics.SyncTransforms();

            if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayCaidaVaso();
        }
    }

    void MoverAToppingsCurva()
    {
        if (vasoElegido == null || workSpot_Toppings == null)
        {
            llegoAToppings = true;
            return;
        }

        if (puntoControl1 == null && puntoControl2 == null)
        {
            MoverEnLineaRecta(workSpot_Toppings.position);
            return;
        }

        float distanciaTotal = Vector3.Distance(posicionOriginalElegida, workSpot_Toppings.position);
        duracionViaje = Mathf.Max(0.01f, distanciaTotal / velocidadMovimiento);

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
        if (vasoElegido == null)
        {
            llegoAToppings = true;
            return;
        }

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
            ActivarColliders(vasoElegido);
            Physics.SyncTransforms();

            ArrastrarVaso arrastrar = vasoElegido.GetComponent<ArrastrarVaso>();
            if (arrastrar != null)
            {
                arrastrar.SeleccionarEsteVaso();
                arrastrar.ActualizarPosicionBase();
            }

            llegoAToppings = true;
            Debug.Log("¡Vaso llegó a la mesa de toppings!");
        }
    }

    private void LimpiarIngredientesDeVaso(Transform vaso)
    {
        if (vaso == null) return;

        for (int i = vaso.childCount - 1; i >= 0; i--)
        {
            Transform hijo = vaso.GetChild(i);

            if (hijo.GetComponentInChildren<IngredienteVolador>(true) != null ||
                hijo.GetComponent<IngredienteVolador>() != null)
            {
                Destroy(hijo.gameObject);
            }
        }
    }

    public void EliminarVasoActual()
    {
        if (vasoSpawneadoActual != null)
        {
            Destroy(vasoSpawneadoActual);
            vasoSpawneadoActual = null;
        }

        if (reemplazoActual != null)
        {
            Destroy(reemplazoActual);
            reemplazoActual = null;
        }

        if (workSpot_Toppings != null)
        {
            for (int i = workSpot_Toppings.childCount - 1; i >= 0; i--)
            {
                Transform hijo = workSpot_Toppings.GetChild(i);
                if (hijo != vasoChico && hijo != vasoGrande)
                {
                    Destroy(hijo.gameObject);
                }
            }
        }

        if (vasoElegido != null && vasoElegido != vasoChico && vasoElegido != vasoGrande)
        {
            Destroy(vasoElegido.gameObject);
        }

        LimpiarIngredientesDeVaso(vasoChico);
        LimpiarIngredientesDeVaso(vasoGrande);

        if (vasoChico != null)
        {
            vasoChico.gameObject.SetActive(true);
            vasoChico.SetParent(padreInicialChico);
            vasoChico.position = posInicialChico;
            vasoChico.rotation = rotInicialChico;
            vasoChico.localScale = Vector3.one * escalaOriginalChico;
            ActivarColliders(vasoChico);
        }

        if (vasoGrande != null)
        {
            vasoGrande.gameObject.SetActive(true);
            vasoGrande.SetParent(padreInicialGrande);
            vasoGrande.position = posInicialGrande;
            vasoGrande.rotation = rotInicialGrande;
            vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            ActivarColliders(vasoGrande);
        }

        Physics.SyncTransforms();

        vasoElegido = null;
        ultimoHover = null;
        yaEligio = false;
        terminocrecimiento = false;
        llegoAToppings = false;
        reemplazoCayendo = false;
        tiempoCrecimiento = 0f;
        tiempoDeViaje = 0f;

        vasoYaElegido = false;
        eraChico = false;

        Debug.Log("Vasos restaurados. Ya puedes volver a elegir el vaso grande o el chico.");
    }
}