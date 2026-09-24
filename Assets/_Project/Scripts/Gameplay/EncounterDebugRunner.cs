using UnityEngine;

// ============================================================
// EncounterDebugRunner.cs
//
// v2 — antes de que existiera la UI real, este script resolvia todo
// el interrogatorio de una sola vez dentro de Start() (un while que
// elegia respuestas "scripteadas" sin ceder el control ni un frame).
// Eso funcionaba para validar la maquina de estados por consola, pero
// es incompatible con la UI real: para cuando cualquier otro script
// (como InterrogationUIController) llegaba a subscribirse a los
// eventos del encuentro, el encuentro entero ya habia terminado.
//
// Ahora este script solo dispara el encuentro. Las respuestas las
// elige el jugador haciendo clic en los botones reales
// (InterrogationUIController -> EncounterController.AnswerInterrogation).
// Lo unico que todavia no tiene botón en la UI es la decision final
// (dejar pasar / rechazar), asi que este runner la resuelve
// automáticamente en cuanto el encuentro llega a DecisionState,
// usando el mismo patrón basado en eventos (nada de polling).
// ============================================================

public class EncounterDebugRunner : MonoBehaviour
{
    [SerializeField] private EncounterController controller;
    [SerializeField] private NPCProfile npcProfile;

    [Tooltip("Que decide el jugador al final: dejarlo pasar o no. " +
             "Temporal hasta que haya botones de decision en la UI.")]
    [SerializeField] private bool playerAllowsEntry = false;

    private void Start()
    {
        if (controller == null || npcProfile == null)
        {
            Debug.LogError("[EncounterDebugRunner] Falta asignar Controller o Npc Profile en el Inspector.");
            return;
        }

        controller.OnStateChanged += HandleStateChanged;
        controller.StartEncounter(npcProfile);
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged()
    {
        if (controller.CurrentState is DecisionState)
        {
            Debug.Log("[EncounterDebugRunner] Interrogatorio terminado, resolviendo decision automaticamente (falta UI de decision).");
            controller.DecidePlayer(playerAllowsEntry);
        }
    }
}
