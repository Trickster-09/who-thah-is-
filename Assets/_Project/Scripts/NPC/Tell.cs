using UnityEngine;

// ============================================================
// Tell.cs
// Una senal visual/comportamental que delata a un impostor (tics,
// glitches, animaciones raras). No es texto -> no va en el CSV de
// localizacion, es un asset de datos aparte que se conecta a
// Animator/VFX. Se asigna opcionalmente a un NPCProfile.
// ============================================================

public enum TellType
{
    Visual,
    Audio,
    Behavior
}

[CreateAssetMenu(fileName = "Tell", menuName = "MKPZ/NPC/Tell")]
public class Tell : ScriptableObject
{
    public string TellId;
    public TellType Type;

    [Tooltip("Nombre del parametro de Animator a disparar cuando ocurre este Tell (opcional).")]
    public string AnimatorTrigger;

    [TextArea]
    [Tooltip("Notas para el equipo, no se muestra al jugador.")]
    public string EditorNotes;
}
