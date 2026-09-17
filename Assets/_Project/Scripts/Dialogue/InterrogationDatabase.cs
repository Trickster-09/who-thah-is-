using System.Collections.Generic;
using UnityEngine;

// ============================================================
// InterrogationDatabase.cs
// Carga el grafo de interrogatorio desde dos CSV:
//   - interrogation_nodes.csv   (node_id, speaker, text_key, requires_flag)
//   - interrogation_answers.csv (node_id, answer_text_key, next_node_id, sets_flag)
//
// A diferencia de LocalizationManager, esto NO es un singleton global:
// se asigna directamente (referencia serializada) a quien maneje un
// encuentro, porque no todo el juego necesita el grafo todo el tiempo
// como sí necesita los textos.
//
// "END" como next_node_id marca el final del interrogatorio.
// ============================================================

[CreateAssetMenu(fileName = "InterrogationDatabase", menuName = "MKPZ/Dialogue/Interrogation Database")]
public class InterrogationDatabase : ScriptableObject
{
    public const string END_NODE_ID = "END";

    [Header("Tablas CSV")]
    [SerializeField] private TextAsset nodesCsv;
    [SerializeField] private TextAsset answersCsv;

    private Dictionary<string, InterrogationNode> _nodes;

    private void OnEnable()
    {
        Load();
    }

    public void Load()
    {
        _nodes = new Dictionary<string, InterrogationNode>();

        ParseNodes();
        ParseAnswers();

        Debug.Log($"[Interrogation] Grafo cargado: {_nodes.Count} nodos.");

#if UNITY_EDITOR
        ValidateGraph();
#endif
    }

    public InterrogationNode GetNode(string nodeId)
    {
        if (_nodes == null) Load();
        if (nodeId == END_NODE_ID || string.IsNullOrEmpty(nodeId)) return null;

        if (_nodes.TryGetValue(nodeId, out var node))
            return node;

        Debug.LogWarning($"[Interrogation] Nodo no encontrado: '{nodeId}'");
        return null;
    }

    private void ParseNodes()
    {
        if (nodesCsv == null)
        {
            Debug.LogError("[Interrogation] No hay CSV de nodos asignado.");
            return;
        }

        string[] lines = nodesCsv.text.Split('\n');
        string[] headers = null;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

            string[] columns = CsvUtils.ParseLine(line);

            if (headers == null) { headers = columns; continue; }
            if (columns.Length < 3) continue;

            string id = columns[0].Trim();
            if (string.IsNullOrEmpty(id)) continue;

            if (_nodes.ContainsKey(id))
                Debug.LogWarning($"[Interrogation] node_id duplicado: '{id}'");

            _nodes[id] = new InterrogationNode
            {
                Id = id,
                Speaker = columns[1].Trim(),
                TextKey = columns[2].Trim(),
                RequiresFlag = columns.Length > 3 ? columns[3].Trim() : ""
            };
        }
    }

    private void ParseAnswers()
    {
        if (answersCsv == null)
        {
            Debug.LogError("[Interrogation] No hay CSV de respuestas asignado.");
            return;
        }

        string[] lines = answersCsv.text.Split('\n');
        string[] headers = null;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

            string[] columns = CsvUtils.ParseLine(line);

            if (headers == null) { headers = columns; continue; }
            if (columns.Length < 3) continue;

            string nodeId = columns[0].Trim();
            if (string.IsNullOrEmpty(nodeId)) continue;

            if (!_nodes.TryGetValue(nodeId, out var node))
            {
                Debug.LogWarning($"[Interrogation] Respuesta apunta a un node_id inexistente: '{nodeId}'");
                continue;
            }

            node.Answers.Add(new InterrogationAnswer
            {
                TextKey = columns[1].Trim(),
                NextNodeId = columns[2].Trim(),
                SetsFlag = columns.Length > 3 ? columns[3].Trim() : ""
            });
        }
    }

#if UNITY_EDITOR
    private void ValidateGraph()
    {
        foreach (var node in _nodes.Values)
        {
            if (node.Answers.Count == 0)
                Debug.LogWarning($"[Interrogation] El nodo '{node.Id}' no tiene respuestas (callejon sin salida).");

            foreach (var answer in node.Answers)
            {
                if (answer.NextNodeId != END_NODE_ID && !_nodes.ContainsKey(answer.NextNodeId))
                    Debug.LogWarning($"[Interrogation] El nodo '{node.Id}' tiene una respuesta que apunta a un next_node_id inexistente: '{answer.NextNodeId}'");
            }
        }
    }
#endif
}
