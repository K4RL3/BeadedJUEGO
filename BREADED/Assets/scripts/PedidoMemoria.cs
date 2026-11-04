using System.Collections.Generic;
using UnityEngine;

public static class PedidoMemoria
{
    // Guarda las imágenes del pedido actual
    public static Sprite[] IngredientesActuales;

    // Guarda los nombres de los NPCs que ya fueron atendidos
    public static List<string> NpcsAtendidos = new List<string>();
}