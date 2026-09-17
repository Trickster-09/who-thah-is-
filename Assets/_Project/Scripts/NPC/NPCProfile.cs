using System.Collections.Generic;
using UnityEngine;

// ============================================================
// NPCProfile.cs
// Un visitante que llega a la base: quien dice ser vs. quien es
// realmente. Referencia el nodo inicial del grafo de interrogatorio,
// las reglas de contradiccion que aplican si es impostor, y las
// senales visuales/comportamentales (Tell) que tiene asignadas.
// ============================================================

[CreateAssetMenu(fileName = "NPCProfile", menuName = "MKPZ/NPC/Profile")]
public class NPCProfile : ScriptableObject
{
    [Header("Identidad")]
    public string NpcId;

    [Tooltip("Clave de localizacion del nombre que el NPC dice tener (puede no coincidir con la realidad).")]
    public string DeclaredNameKey;

    [Tooltip("Verdad de fondo: si este NPC es realmente un impostor.")]
    public bool IsImpostor;

    [Header("Interrogatorio")]
    [Tooltip("Id del primer nodo del grafo de interrogatorio para este NPC (ver InterrogationDatabase).")]
    public string StartNodeId;

    [Header("Contradicciones (relevante sobre todo si IsImpostor)")]
    public List<ContradictionRule> ContradictionRules = new List<ContradictionRule>();

    [Header("Señales visuales/comportamentales")]
    public List<Tell> Tells = new List<Tell>();

    /// <summary>
    /// Compara los flags que se fueron marcando durante el interrogatorio
    /// contra las reglas de contradiccion de este NPC. Devuelve una
    /// descripcion por cada contradiccion encontrada (vacio si no hay
    /// ninguna).
    /// </summary>
    public List<string> EvaluateContradictions(HashSet<string> collectedFlags)
    {
        var found = new List<string>();

        foreach (var rule in ContradictionRules)
        {
            if (string.IsNullOrEmpty(rule.FlagA) || string.IsNullOrEmpty(rule.FlagB))
                continue;

            if (collectedFlags.Contains(rule.FlagA) && collectedFlags.Contains(rule.FlagB))
                found.Add($"{rule.FlagA} vs {rule.FlagB}");
        }

        return found;
    }
}
