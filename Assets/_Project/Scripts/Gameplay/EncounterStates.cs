using UnityEngine;

// ============================================================
// EncounterStates.cs
// Las 5 fases del encuentro: Acercamiento -> Interrogatorio ->
// Inspeccion -> Decision -> Consecuencia. Son clases livianas, sin
// estado propio mas alla de lo que necesitan para actuar sobre el
// EncounterController.
// ============================================================

public class ApproachState : IEncounterState
{
    public void Enter(EncounterController controller)
    {
        Debug.Log($"[Encounter] {controller.CurrentNpc.NpcId} se acerca a la entrada.");
        controller.TransitionTo(new InterrogationState());
    }

    public void Exit(EncounterController controller) { }
}

public class InterrogationState : IEncounterState
{
    public void Enter(EncounterController controller)
    {
        controller.BeginInterrogation();
        Debug.Log("[Encounter] Comienza el interrogatorio.");
    }

    public void Exit(EncounterController controller) { }
}

public class InspectionState : IEncounterState
{
    // Placeholder: aca se van a mostrar los Tells (glitches, documentos)
    // cuando esten conectados a Animator/VFX. Por ahora solo informa
    // el resultado del interrogatorio y avanza.
    public void Enter(EncounterController controller)
    {
        if (controller.ContradictionsFound.Count > 0)
            Debug.Log($"[Encounter] Inspeccion: {controller.ContradictionsFound.Count} contradiccion(es) detectada(s) en el dialogo.");
        else
            Debug.Log("[Encounter] Inspeccion: sin contradicciones detectadas en el dialogo (revisar señales visuales).");

        controller.TransitionTo(new DecisionState());
    }

    public void Exit(EncounterController controller) { }
}

public class DecisionState : IEncounterState
{
    public void Enter(EncounterController controller)
    {
        Debug.Log("[Encounter] Esperando la decision del jugador (dejar pasar / rechazar)...");
    }

    public void Exit(EncounterController controller) { }
}

public class ConsequenceState : IEncounterState
{
    public void Enter(EncounterController controller)
    {
        var outcome = controller.Outcome;
        string resultado = outcome.WasCorrect ? "CORRECTA" : "INCORRECTA";
        string accion = outcome.PlayerAllowedEntry ? "dejo pasar" : "rechazo";
        string verdad = outcome.NpcWasImpostor ? "ERA un impostor" : "NO era un impostor";

        Debug.Log($"[Encounter] Decision {resultado}: el jugador {accion} al NPC, que {verdad}.");
    }

    public void Exit(EncounterController controller) { }
}
