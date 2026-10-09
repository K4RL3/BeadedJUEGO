using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.SceneView;

public class Vasos : MonoBehaviour
{
    public Transform vasoChico;
    public Transform vasoGrande;
    public Transform workSpot;

    public float velocidadMovimiento = 3f;
    public float escalaFinal = 1.3f;

    public MoverCamara cameraMover;

    private Transform vasoElegido = null;
    private bool yaEligio = false;
    private bool yaLlego = false;
    private float escalaOriginalChico;
    private float escalaOriginalGrande;

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
                yaEligio = true;
                Debug.Log("Elegiste el vaso CHICO");
            }
            else if (golpe.transform == vasoGrande)
            {
                vasoElegido = vasoGrande;
                yaEligio = true;
                Debug.Log("Elegiste el vaso GRANDE");
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
            vasoElegido.localScale *= escalaFinal;
            yaLlego = true;
            Debug.Log("¡Vaso listo en el centro!");

            if (cameraMover != null)
                cameraMover.MoverAToppings();
        }
    }
}