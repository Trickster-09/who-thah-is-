using UnityEngine;

// ============================================================
// ContradictionRule.cs
// Par de flags que, si ambos quedaron marcados durante el
// interrogatorio, se consideran una contradiccion del NPC.
// Ejemplo: FlagA="claim_soldier", FlagB="claim_never_been_here"
// ============================================================

[System.Serializable]
public class ContradictionRule
{
    public string FlagA;
    public string FlagB;

    [Tooltip("Opcional: clave de localizacion que explica la contradiccion (para UI/debug).")]
    public string DescriptionKey;
}
