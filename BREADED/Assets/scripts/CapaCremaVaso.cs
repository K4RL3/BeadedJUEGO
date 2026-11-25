using System;
using UnityEngine;

public class CapaCremaVaso : MonoBehaviour
{
    [Serializable]
    public struct OpcionCrema
    {
        public string nombreSabor;
        public Transform modeloCrema;
        [HideInInspector] public Vector3 escalaBase;
    }

    [Header("Los 3 Sabores (Hijos del Vaso)")]
    public OpcionCrema[] saboresCrema;

    [Header("Datos para Calificación")]
    public bool tieneCrema = false;
    public string saborAplicado = "";
    public float precisionCrema = 0f;

    void Awake()
    {
        for (int i = 0; i < saboresCrema.Length; i++)
        {
            if (saboresCrema[i].modeloCrema != null)
            {
                saboresCrema[i].escalaBase = saboresCrema[i].modeloCrema.localScale;
                saboresCrema[i].modeloCrema.localScale = Vector3.zero;
                saboresCrema[i].modeloCrema.gameObject.SetActive(false);
            }
        }
    }

    public void AplicarCapaPorMinijuego(float precision, float factorAltura, string sabor)
    {
        // Apagamos cualquier crema previa y buscamos el sabor pedido
        for (int i = 0; i < saboresCrema.Length; i++)
        {
            Transform modelo = saboresCrema[i].modeloCrema;
            if (modelo == null) continue;

            bool coincide = string.Equals(saboresCrema[i].nombreSabor, sabor, StringComparison.OrdinalIgnoreCase);
            modelo.gameObject.SetActive(coincide);

            if (coincide)
            {
                tieneCrema = true;
                saborAplicado = saboresCrema[i].nombreSabor;
                precisionCrema = precision;

                Vector3 baseScale = saboresCrema[i].escalaBase;
                modelo.localScale = Vector3.zero;

                Vector3 escalaFinal = new Vector3(
                    baseScale.x * (factorAltura > 1.2f ? 1.15f : 1f),
                    baseScale.y * factorAltura,
                    baseScale.z * (factorAltura > 1.2f ? 1.15f : 1f)
                );

                LeanTween.scale(modelo.gameObject, escalaFinal, 0.35f).setEaseOutBack();
            }
        }
    }

    public void LimpiarCrema()
    {
        tieneCrema = false;
        saborAplicado = "";
        precisionCrema = 0f;

        for (int i = 0; i < saboresCrema.Length; i++)
        {
            Transform modelo = saboresCrema[i].modeloCrema;
            if (modelo != null)
            {
                LeanTween.cancel(modelo.gameObject);
                modelo.localScale = Vector3.zero;
                modelo.gameObject.SetActive(false);
            }
        }
    }
}