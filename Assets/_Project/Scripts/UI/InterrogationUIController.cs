using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
// InterrogationUIController.cs
// Muestra el interrogatorio con UI real, reemplazando las respuestas
// scripteadas de EncounterDebugRunner. Se subscribe a los eventos de
// EncounterController (OnStateChanged, OnQuestionChanged) en vez de
// consultar cada frame en Update() -> cero costo cuando no hay nada
// nuevo que mostrar.
//
// No sabe nada del grafo ni del CSV: solo pide "la pregunta actual" y
// "las respuestas actuales" al controller y arma botones dinamicos,
// uno por respuesta.
//
// Setup en el Editor (con los sprites de Fantasy UI Borders ya
// importados):
//   1. Panel Root: el panel completo del interrogatorio (imagen con
//      Fantasy UI Borders, tipo de imagen "Sliced"). Asignalo a
//      "Panel Root". Empieza desactivado, este script lo prende/apaga.
//   2. Speaker Text y Question Text: dos TMP_Text dentro del panel.
//   3. Answers Container: un objeto vacio dentro del panel con un
//      Vertical Layout Group (para que los botones se acomoden solos).
//   4. Answer Button Prefab: un prefab de boton (Image "Sliced" con
//      un sprite de Fantasy UI Borders + un TMP_Text hijo para el
//      texto de la respuesta).
// ============================================================

public class InterrogationUIController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EncounterController controller;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Transform answersContainer;
    [SerializeField] private Button answerButtonPrefab;

    private readonly List<Button> _spawnedButtons = new List<Button>();

    private void OnEnable()
    {
        if (controller == null)
        {
            Debug.LogError("[InterrogationUIController] Falta asignar el EncounterController.");
            return;
        }

        controller.OnStateChanged += HandleStateChanged;
        controller.OnQuestionChanged += RefreshQuestion;

        // Catch-up: si el encuentro ya avanzo antes de que este script se
        // habilitara (el orden de ejecucion entre scripts de distintos
        // GameObjects no esta garantizado), nos ponemos al dia a mano en
        // vez de esperar el proximo evento, que podria no llegar nunca.
        HandleStateChanged();
        RefreshQuestion();
    }

    private void OnDisable()
    {
        if (controller == null) return;

        controller.OnStateChanged -= HandleStateChanged;
        controller.OnQuestionChanged -= RefreshQuestion;
    }

    private void HandleStateChanged()
    {
        bool inInterrogation = controller.CurrentState is InterrogationState;
        if (panelRoot != null)
            panelRoot.SetActive(inInterrogation);
    }

    private void RefreshQuestion()
    {
        var node = controller.GetCurrentQuestion();
        if (node == null) return;

        if (speakerText != null) speakerText.text = node.Speaker;
        if (questionText != null) questionText.text = LocalizationManager.Get(node.TextKey);

        BuildAnswerButtons(node);
    }

    private void BuildAnswerButtons(InterrogationNode node)
    {
        foreach (var btn in _spawnedButtons)
            if (btn != null) Destroy(btn.gameObject);
        _spawnedButtons.Clear();

        if (answersContainer == null || answerButtonPrefab == null)
        {
            Debug.LogWarning("[InterrogationUIController] Falta Answers Container o Answer Button Prefab.");
            return;
        }

        for (int i = 0; i < node.Answers.Count; i++)
        {
            int index = i; // capturar valor para el closure del listener
            var button = Instantiate(answerButtonPrefab, answersContainer);

            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = LocalizationManager.Get(node.Answers[i].TextKey);

            button.onClick.AddListener(() => OnAnswerClicked(index));
            _spawnedButtons.Add(button);
        }
    }

    private void OnAnswerClicked(int index)
    {
        controller.AnswerInterrogation(index);
    }
}
