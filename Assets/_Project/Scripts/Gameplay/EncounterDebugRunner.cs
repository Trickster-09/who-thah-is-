using UnityEngine;

// ============================================================
// EncounterDebugRunner.cs
// Corre un encuentro completo automaticamente al entrar en Play, para
// validar la maquina de estados de punta a punta antes de construir
// la UI real. Reemplaza en la práctica a InterrogationDebugRunner
// (que solo probaba el grafo, no el flujo completo del encuentro).
// ============================================================

public class EncounterDebugRunner : MonoBehaviour
{
    [SerializeField] private EncounterController controller;
    [SerializeField] private NPCProfile npcProfile;

    [Tooltip("Indice de respuesta a elegir en cada pregunta, en orden.")]
    [SerializeField] private int[] scriptedAnswers = new int[] { 0, 0 };

    [Tooltip("Que decide el jugador al final: dejarlo pasar o no.")]
    [SerializeField] private bool playerAllowsEntry = false;

    private void Start()
    {
        if (controller == null || npcProfile == null)
        {
            Debug.LogError("[EncounterDebugRunner] Falta asignar Controller o Npc Profile en el Inspector.");
            return;
        }

        controller.StartEncounter(npcProfile);

        int step = 0;
        while (controller.CurrentState is InterrogationState)
        {
            var node = controller.GetCurrentQuestion();
            if (node == null) break;

            Debug.Log($"[Interrogatorio] {node.Speaker}: {LocalizationManager.Get(node.TextKey)}");

            int answerIndex = step < scriptedAnswers.Length ? scriptedAnswers[step] : 0;
            if (answerIndex >= node.Answers.Count) break;

            Debug.Log($"[Interrogatorio] Jugador elige: {LocalizationManager.Get(node.Answers[answerIndex].TextKey)}");
            controller.AnswerInterrogation(answerIndex);
            step++;
        }

        controller.DecidePlayer(playerAllowsEntry);
    }
}
