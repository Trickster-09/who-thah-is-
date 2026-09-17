using System.Collections.Generic;

// ============================================================
// InterrogationSession.cs
// Camina el grafo de interrogatorio para UN NPC concreto: guarda que
// flags se fueron marcando segun las respuestas elegidas, y al llegar
// a "END" puede evaluarse contra las reglas de contradiccion del
// NPCProfile. Clase de runtime pura (no ScriptableObject, no
// MonoBehaviour) para poder testearla sin correr la escena.
// ============================================================

public class InterrogationSession
{
    private readonly InterrogationDatabase _database;
    private readonly NPCProfile _npc;
    private readonly HashSet<string> _flags = new HashSet<string>();

    public string CurrentNodeId { get; private set; }
    public bool IsFinished => CurrentNodeId == InterrogationDatabase.END_NODE_ID || CurrentNodeId == null;

    public InterrogationSession(InterrogationDatabase database, NPCProfile npc)
    {
        _database = database;
        _npc = npc;
        CurrentNodeId = npc.StartNodeId;
    }

    public InterrogationNode GetCurrentNode() => _database.GetNode(CurrentNodeId);

    public void ChooseAnswer(int answerIndex)
    {
        var node = GetCurrentNode();
        if (node == null || answerIndex < 0 || answerIndex >= node.Answers.Count) return;

        var answer = node.Answers[answerIndex];
        if (!string.IsNullOrEmpty(answer.SetsFlag))
            _flags.Add(answer.SetsFlag);

        CurrentNodeId = answer.NextNodeId;
    }

    public List<string> FinishAndEvaluate() => _npc.EvaluateContradictions(_flags);
}
