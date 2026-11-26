using System.Collections.Generic;
using UnityEngine;

public static class ParfaitMemoria
{
    public static string NombreVaso = "";
    public static ArrastrarVaso VasoFinal;
    public static List<string> CapasAgregadas = new List<string>();
    public static List<Sprite> IconosCapas = new List<Sprite>();
    public static bool ParfaitListoParaCalificar = false;

    public static void Limpiar()
    {
        NombreVaso = "";
        VasoFinal = null;
        CapasAgregadas.Clear();
        IconosCapas.Clear();
        ParfaitListoParaCalificar = false;
    }

    public static void RegistrarCapa(string nombreIngrediente, Sprite icono = null)
    {
        if (!string.IsNullOrEmpty(nombreIngrediente))
            CapasAgregadas.Add(nombreIngrediente);

        if (icono != null)
            IconosCapas.Add(icono);
    }

    public static void GuardarEstadoFinal(ArrastrarVaso vaso)
    {
        string nombreAnterior = NombreVaso;
        List<string> capasAnteriores = new List<string>(CapasAgregadas);
        List<Sprite> iconosAnteriores = new List<Sprite>(IconosCapas);

        Limpiar();

        CapasAgregadas = capasAnteriores;
        IconosCapas = iconosAnteriores;

        if (vaso == null) return;

        VasoFinal = vaso;
        NombreVaso = !string.IsNullOrEmpty(nombreAnterior) ? nombreAnterior : vaso.gameObject.name;
        ParfaitListoParaCalificar = true;
    }

    public static void PrepararParaCalificacion(ArrastrarVaso vaso)
    {
        GuardarEstadoFinal(vaso);

        if (vaso == null) return;

        vaso.enabled = false;
        vaso.transform.SetParent(null);
        Object.DontDestroyOnLoad(vaso.gameObject);
    }
}