// ============================================================
// IEncounterState.cs
// Contrato comun para cada fase del encuentro en la entrada de la
// base. Deliberadamente no depende de Animator ni de nada visual:
// asi se puede testear la logica de decision sin correr animaciones,
// como definimos en el diseño original.
// ============================================================

public interface IEncounterState
{
    void Enter(EncounterController controller);
    void Exit(EncounterController controller);
}
