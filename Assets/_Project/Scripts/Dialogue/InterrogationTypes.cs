using System.Collections.Generic;

// ============================================================
// InterrogationTypes.cs
// Estructuras de datos del grafo de interrogatorio (no son assets de
// Unity, se construyen en memoria al parsear los CSV).
// ============================================================

public class InterrogationAnswer
{
    public string TextKey;
    public string NextNodeId;
    public string SetsFlag;
}

public class InterrogationNode
{
    public string Id;
    public string Speaker;
    public string TextKey;
    public string RequiresFlag;
    public List<InterrogationAnswer> Answers = new List<InterrogationAnswer>();
}
