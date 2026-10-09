using UnityEngine;
using UnityEngine.InputSystem;

public class Vasos : MonoBehaviour
{
    public Transform vasoChico;
    public Transform vasoGrande;
    public Transform workSpot;
    public Transform workSpot_Toppings;
    public Transform camara;

    public float velocidadMovimiento = 3f;
    public float escalaFinal = 1.3f;

    public MoverCamara cameraMover;

    private Transform vasoElegido = null;
    private bool yaEligio = false;
    private bool yaLlego = false;
    private bool camaraLlego = false;
    private bool vasoEnToppings = false;
    private float escalaOriginalChico;
    private float escalaOriginalGrande;
    private float escalaOriginalElegida;

    void Start()
    {
        escalaOriginalChico = vasoChico.localScale.x;
        escalaOriginalGrande = vasoGrande.localScale.x;
    }

    void Update()
    {
        if (!yaEligio)
        {
            DetectarHover();
            DetectarClick();
        }
        else if (!yaLlego)
        {
            MoverAlCentro();
        }
        else if (!camaraLlego)
        {
            VerificarCamara();
        }
        else if (!vasoEnToppings)
        {
            ColocarVasoEnToppings();
        }
    }

    void DetectarHover()
    {
        Ray rayo = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(rayo, out RaycastHit golpe, 100f))
        {
            if (golpe.transform == vasoChico)
            {
                vasoChico.localScale = Vector3.one * (escalaOriginalChico * 1.1f);
                vasoGrande.localScale = Vector3.one * escalaOriginalGrande;
            }
            else if (golpe.transform == vasoGrande)
            {
                vasoGrande.localScale = Vector3.one * (escalaOriginalGrande * 1.1f);
                vasoChico.localScale = Vector3.one * escalaOriginalChico;
            }
            else
            {
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
                escalaOriginalElegida = escalaOriginalChico;
                yaEligio = true;
            }
            else if (golpe.transform == vasoGrande)
            {
                vasoElegido = vasoGrande;
                escalaOriginalElegida = escalaOriginalGrande;
                yaEligio = true;
            }
        }
    }

    void MoverAlCentro()
    {
        vasoElegido.position = Vector3.MoveTowards(
            vasoElegido.position,
            workSpot.position,
            velocidadMovimiento * Time.deltaTime
        );

        float distancia = Vector3.Distance(vasoElegido.position, workSpot.position);

        if (distancia < 0.01f)
        {
            vasoElegido.position = workSpot.position;
            vasoElegido.localScale = Vector3.one * (escalaOriginalElegida * escalaFinal);
            yaLlego = true;

            vasoElegido.SetParent(camara);

            if (cameraMover != null)
                cameraMover.MoverAToppings();
        }
    }

    void VerificarCamara()
    {
        if (cameraMover != null && !cameraMover.EstaMoviendo())
        {
            camaraLlego = true;
            vasoElegido.SetParent(null);
        }
    }

    void ColocarVasoEnToppings()
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

            vasoEnToppings = true;
        }
    }
}