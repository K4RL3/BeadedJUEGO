using UnityEngine;
using UnityEngine.InputSystem;

public class Vasos : MonoBehaviour
{
    public Transform vasoChico;
    public Transform vasoGrande;
    public Transform workSpot_Toppings;
    public GameObject prefabVasoChico;
    public GameObject prefabVasoGrande;
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

    void Start()
    {
        escalaOriginalChico = vasoChico.localScale.x;
        escalaOriginalGrande = vasoGrande.localScale.x;

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
        vaso.transform.localScale = Vector3.one * escala;

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
            if (!llegoAToppings) MoverAToppings();
        }
    }

    void DetectarHover()
    {
        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            if (golpe.transform == vasoChico)
            {
                if (ultimoHover != vasoChico && ManejadorAudio.Instance != null)
                    ManejadorAudio.Instance.PlayHover();
                ultimoHover = vasoChico;

                vasoChico.localScale = Vector3.one * (escalaOriginalChico * 1.1f);
                vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            }
            else if (golpe.transform == vasoGrande)
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
            if (golpe.transform == vasoChico)
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
            else if (golpe.transform == vasoGrande)
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

    void MoverAToppings()
    {
        vasoElegido.position = Vector3.MoveTowards(
            vasoElegido.position,
            workSpot_Toppings.position,
            velocidadMovimiento * Time.deltaTime
        );

        float distancia = Vector3.Distance(vasoElegido.position, workSpot_Toppings.position);

        if (distancia < 0.01f)
        {
            vasoElegido.position = workSpot_Toppings.position;
            vasoElegido.rotation = workSpot_Toppings.rotation;
            vasoElegido.localScale = Vector3.one * escalaOriginalElegida;
            llegoAToppings = true;
            Debug.Log("¡Vaso llegó a la mesa de toppings!");
        }
    }
}