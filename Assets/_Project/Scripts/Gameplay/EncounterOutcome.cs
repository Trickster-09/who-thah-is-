using System.Collections.Generic;

// ============================================================
// EncounterOutcome.cs
// Resultado final de un encuentro: que decidio el jugador, la verdad
// de fondo, si acerto, y que contradicciones se habian detectado.
// ============================================================

public class EncounterOutcome
{
    public bool PlayerAllowedEntry;
    public bool NpcWasImpostor;
    public bool WasCorrect;
    public List<string> ContradictionsFound;
}
