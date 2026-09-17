#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ============================================================
// TestNpcSetupTool.cs (Editor only)
// Crea de un clic los assets necesarios para probar el interrogatorio
// de punta a punta, sin arrastrar nada a mano en el Inspector:
//   - InterrogationDatabase enlazado a los CSV de prueba
//   - Un NPCProfile de prueba con una regla de contradiccion ya
//     cargada (claim_soldier + claim_never_been_here)
//
// Uso: menu Tools > MKPZ > Crear NPC de prueba (Interrogatorio)
// Se puede correr varias veces, reutiliza los assets si ya existen.
// ============================================================

public static class TestNpcSetupTool
{
    private const string DialogueFolder = "Assets/_Project/Data/DialogueGraphs";
    private const string NpcFolder = "Assets/_Project/Data/NPCProfiles";

    [MenuItem("Tools/MKPZ/Crear NPC de prueba (Interrogatorio)")]
    public static void CreateTestSetup()
    {
        var db = AssetDatabase.LoadAssetAtPath<InterrogationDatabase>($"{DialogueFolder}/InterrogationDatabase.asset");
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<InterrogationDatabase>();
            AssetDatabase.CreateAsset(db, $"{DialogueFolder}/InterrogationDatabase.asset");
        }

        var nodesCsv = AssetDatabase.LoadAssetAtPath<TextAsset>($"{DialogueFolder}/interrogation_nodes.csv");
        var answersCsv = AssetDatabase.LoadAssetAtPath<TextAsset>($"{DialogueFolder}/interrogation_answers.csv");

        if (nodesCsv == null || answersCsv == null)
        {
            Debug.LogError("[MKPZ] No encuentro interrogation_nodes.csv / interrogation_answers.csv en " + DialogueFolder);
            return;
        }

        var dbSerialized = new SerializedObject(db);
        dbSerialized.FindProperty("nodesCsv").objectReferenceValue = nodesCsv;
        dbSerialized.FindProperty("answersCsv").objectReferenceValue = answersCsv;
        dbSerialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(db);

        var npc = AssetDatabase.LoadAssetAtPath<NPCProfile>($"{NpcFolder}/NPC_TestSoldier.asset");
        if (npc == null)
        {
            npc = ScriptableObject.CreateInstance<NPCProfile>();
            AssetDatabase.CreateAsset(npc, $"{NpcFolder}/NPC_TestSoldier.asset");
        }

        npc.NpcId = "test_soldier";
        npc.IsImpostor = true;
        npc.StartNodeId = "q1";
        npc.ContradictionRules = new List<ContradictionRule>
        {
            new ContradictionRule { FlagA = "claim_soldier", FlagB = "claim_never_been_here" }
        };
        EditorUtility.SetDirty(npc);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[MKPZ] Listo: InterrogationDatabase.asset y NPC_TestSoldier.asset creados/actualizados. " +
                   "Agrega un GameObject vacio a la escena, ponele el componente InterrogationDebugRunner, " +
                   "y arrastrale estos dos assets.");
        Selection.activeObject = npc;
    }
}
#endif
