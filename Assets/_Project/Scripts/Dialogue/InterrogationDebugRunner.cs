using UnityEngine;

// ============================================================
// InterrogationDebugRunner.cs
// Corre el interrogatorio automaticamente al entrar en Play, eligiendo
// respuestas fijas (scriptedAnswers) para probar que el flujo completo
// -grafo + flags + deteccion de contradicciones- funciona de punta a
// punta. Es solo para validar el sistema; se reemplaza por UI real
// cuando se diseñe la pantalla de interrogatorio.
// ============================================================

public class InterrogationDebugRunner : MonoBehaviour
{
    [SerializeField] private InterrogationDatabase database;
    [SerializeField] private NPCProfile npcProfile;

    [Tooltip("Indice de respuesta a elegir en cada nodo, en orden (0 = primera opcion).")]
    [SerializeField] private int[] scriptedAnswers = new int[] { 0, 0 };

    private void Start()
    {
        if (database == null || npcProfile == null)
        {
            Debug.LogError("[InterrogationDebugRunner] Falta asignar Database o Npc Profile en el Inspector.");
            return;
        }

        var session = new InterrogationSession(database, npcProfile);
        int step = 0;

        while (!session.IsFinished)
        {
            var node = session.GetCurrentNode();
            if (node == null)
            {
                Debug.LogWarning("[InterrogationDebugRunner] Nodo actual nulo, corto la simulacion.");
                break;
            }

            Debug.Log($"[Interrogatorio] {node.Speaker}: {LocalizationManager.Get(node.TextKey)}");

            int answerIndex = step < scriptedAnswers.Length ? scriptedAnswers[step] : 0;
            if (node.Answers.Count == 0 || answerIndex >= node.Answers.Count)
            {
                Debug.LogWarning($"[InterrogationDebugRunner] El nodo '{node.Id}' no tiene respuesta en el indice {answerIndex}.");
                break;
            }

            Debug.Log($"[Interrogatorio] Jugador elige: {LocalizationManager.Get(node.Answers[answerIndex].TextKey)}");

            session.ChooseAnswer(answerIndex);
            step++;
        }

        var contradictions = session.FinishAndEvaluate();
        if (contradictions.Count > 0)
            Debug.Log($"[Interrogatorio] Contradicciones detectadas ({contradictions.Count}): {string.Join(" | ", contradictions)}");
        else
            Debug.Log("[Interrogatorio] No se detectaron contradicciones.");
    }
}
