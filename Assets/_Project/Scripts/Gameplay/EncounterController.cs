using System;
using System.Collections.Generic;
using UnityEngine;

// ============================================================
// EncounterController.cs
// Dueño del estado actual del encuentro. No sabe nada de UI ni de
// Animator: expone metodos que la UI (o EncounterDebugRunner, para
// pruebas sin UI) llama para avanzar el flujo.
//
// OnStateChanged / OnQuestionChanged: la UI se subscribe a estos
// eventos y refresca SOLO cuando algo realmente cambio (nunca desde
// Update()). Es la misma disciplina de "cachear y refrescar por
// evento" que ya usa LocalizationManager.OnLanguageChanged.
// ============================================================

public class EncounterController : MonoBehaviour
{
    [Header("Datos")]
    [SerializeField] private InterrogationDatabase interrogationDatabase;

    public NPCProfile CurrentNpc { get; private set; }
    public List<string> ContradictionsFound { get; private set; } = new List<string>();
    public EncounterOutcome Outcome { get; private set; }
    public IEncounterState CurrentState { get; private set; }

    public event Action OnStateChanged;
    public event Action OnQuestionChanged;

    private InterrogationSession _session;

    public void StartEncounter(NPCProfile npc)
    {
        CurrentNpc = npc;
        ContradictionsFound = new List<string>();
        Outcome = null;
        TransitionTo(new ApproachState());
    }

    public void TransitionTo(IEncounterState next)
    {
        CurrentState?.Exit(this);
        CurrentState = next;
        CurrentState.Enter(this);
        OnStateChanged?.Invoke();
    }

    // ── Usado durante InterrogationState ──────────────────────

    public void BeginInterrogation()
    {
        _session = new InterrogationSession(interrogationDatabase, CurrentNpc);
        OnQuestionChanged?.Invoke();
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
        else
        {
            OnQuestionChanged?.Invoke();
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
