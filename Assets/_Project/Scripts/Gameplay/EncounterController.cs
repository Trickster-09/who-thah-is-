using UnityEngine;

// ============================================================
// EncounterController.cs
// Dueño del estado actual del encuentro. No sabe nada de UI ni de
// Animator: expone metodos que la UI (o, por ahora, EncounterDebugRunner)
// llama para avanzar el flujo, y dispara Debug.Log en cada transicion
// para poder validar todo por consola antes de construir la pantalla
// real.
// ============================================================

public class EncounterController : MonoBehaviour
{
    [Header("Datos")]
    [SerializeField] private InterrogationDatabase interrogationDatabase;

    public NPCProfile CurrentNpc { get; private set; }
    public System.Collections.Generic.List<string> ContradictionsFound { get; private set; } = new System.Collections.Generic.List<string>();
    public EncounterOutcome Outcome { get; private set; }
    public IEncounterState CurrentState { get; private set; }

    private InterrogationSession _session;

    public void StartEncounter(NPCProfile npc)
    {
        CurrentNpc = npc;
        ContradictionsFound = new System.Collections.Generic.List<string>();
        Outcome = null;
        TransitionTo(new ApproachState());
    }

    public void TransitionTo(IEncounterState next)
    {
        CurrentState?.Exit(this);
        CurrentState = next;
        CurrentState.Enter(this);
    }

    // ── Usado durante InterrogationState ──────────────────────

    public void BeginInterrogation()
    {
        _session = new InterrogationSession(interrogationDatabase, CurrentNpc);
    }

    public InterrogationNode GetCurrentQuestion() => _session?.GetCurrentNode();

    public void AnswerInterrogation(int answerIndex)
    {
        if (_session == null || _session.IsFinished) return;

        _session.ChooseAnswer(answerIndex);

        if (_session.IsFinished)
        {
            ContradictionsFound = _session.FinishAndEvaluate();
            TransitionTo(new InspectionState());
        }
    }

    // ── Usado durante DecisionState ───────────────────────────

    public void DecidePlayer(bool allowEntry)
    {
        bool shouldHaveAllowed = !CurrentNpc.IsImpostor;
        Outcome = new EncounterOutcome
        {
            PlayerAllowedEntry = allowEntry,
            NpcWasImpostor = CurrentNpc.IsImpostor,
            WasCorrect = allowEntry == shouldHaveAllowed,
            ContradictionsFound = ContradictionsFound
        };

        TransitionTo(new ConsequenceState());
    }
}
