using System;
using System.Collections.Generic;
using UnityEngine;

// ============================================================
// LocalizationManager.cs  (v3 — basado en tabla CSV)
//
// Setup:
//   1. Crea el asset: clic derecho > Create > MKPZ > Localization > Manager
//   2. Arrastra Assets/_Project/Data/Localization/Tables/strings.csv
//      al campo "Csv Table" en el Inspector
//   3. IMPORTANTE: guardá este asset dentro de una carpeta llamada
//      "Resources" (ej. Assets/_Project/Data/Localization/Resources/LocalizationManager.asset)
//      y que se llame exactamente "LocalizationManager".
//      Esto garantiza que se cargue en memoria automáticamente al
//      primer uso, aunque ningún componente de la escena lo referencie.
//
// Uso desde cualquier script:
//   LocalizationManager.Get("trait.empathy.name")    → "Empatía" o "Empathy"
//   LocalizationManager.Get("ui.test.progress", 2, 8) → "Pregunta 2 de 8"
//
// Convención para texto multilínea en el CSV:
//   Un salto de línea real DENTRO de una celda corta la fila antes de
//   tiempo. Para texto largo con saltos de línea, escribí "\n" literal
//   (barra + n) dentro de la celda — el parser lo convierte a salto de
//   línea real al leerlo.
// ============================================================

[CreateAssetMenu(fileName = "LocalizationManager", menuName = "MKPZ/Localization/Manager")]
public class LocalizationManager : ScriptableObject
{
    // ── Singleton ────────────────────────────────────────────

    private static LocalizationManager _instance;

    /// <summary>
    /// Acceso al manager. Si todavía no se cargó nada (ninguna escena o
    /// componente lo referenció), lo carga automáticamente desde
    /// Resources/LocalizationManager. Ver nota de setup arriba.
    /// </summary>
    public static LocalizationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<LocalizationManager>("LocalizationManager");

            if (_instance == null)
                Debug.LogError("[Localization] No se encontró el asset 'LocalizationManager' en " +
                                "una carpeta Resources/. Moverlo ahí o asegurarte de que algo lo " +
                                "cargue antes del primer Get().");

            return _instance;
        }
    }

    private void OnEnable()
    {
        _instance = this;
        ParseCSV();
        LoadSavedLanguage();
    }

    // ── Inspector ────────────────────────────────────────────

    [Header("Tabla de localización (.csv)")]
    [SerializeField] private TextAsset csvTable;

    [Header("Idioma por defecto")]
    [SerializeField] private GameLanguage defaultLanguage = GameLanguage.Spanish;

    // ── Estado interno ───────────────────────────────────────

    private GameLanguage _currentLanguage;

    // Estructura: key → (idioma → texto)
    private Dictionary<string, Dictionary<GameLanguage, string>> _table;

    public event Action OnLanguageChanged;

    public GameLanguage CurrentLanguage => _currentLanguage;

    // ── API estática ─────────────────────────────────────────

    public static string Get(string key)
    {
        var instance = Instance;
        if (instance == null)
            return key;

        return instance.GetText(key);
    }

    public static string Get(string key, params object[] args)
    {
        string raw = Get(key);
        try   { return string.Format(raw, args); }
        catch { return raw; }
    }

    // ── Cambio de idioma ─────────────────────────────────────

    public void SetLanguage(GameLanguage language)
    {
        if (_currentLanguage == language) return;

        _currentLanguage = language;
        SaveLanguage();
        OnLanguageChanged?.Invoke();

        Debug.Log($"[Localization] Idioma → {language}");
    }

    // ── Lógica interna ───────────────────────────────────────

    private string GetText(string key)
    {
        if (_table == null)
        {
            Debug.LogError("[Localization] La tabla no está cargada.");
            return key;
        }

        if (!_table.TryGetValue(key, out var langs))
        {
            Debug.LogWarning($"[Localization] Clave no encontrada: '{key}'");
            return key;
        }

        if (langs.TryGetValue(_currentLanguage, out string text) && !string.IsNullOrEmpty(text))
            return text;

        if (langs.TryGetValue(GameLanguage.Spanish, out string fallback) && !string.IsNullOrEmpty(fallback))
        {
            Debug.LogWarning($"[Localization] Fallback a español para '{key}' en {_currentLanguage}");
            return fallback;
        }

        return key;
    }

    // ── Parser CSV ───────────────────────────────────────────

    private string[] _headers;

    private void ParseCSV()
    {
        _table = new Dictionary<string, Dictionary<GameLanguage, string>>();
        _headers = null;

        if (csvTable == null)
        {
            Debug.LogError("[Localization] No hay CSV asignado en el Inspector del LocalizationManager.");
            return;
        }

        string[] lines = csvTable.text.Split('\n');

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

            string[] columns = ParseCSVLine(line);

            if (_headers == null)
            {
                _headers = columns;
                continue;
            }

            if (columns.Length < 2) continue;

            string key = columns[0].Trim();
            if (string.IsNullOrEmpty(key)) continue;

            if (_table.ContainsKey(key))
                Debug.LogWarning($"[Localization] Clave duplicada en el CSV: '{key}' (se sobrescribió la fila anterior).");

            var langDict = new Dictionary<GameLanguage, string>();

            for (int i = 1; i < _headers.Length && i < columns.Length; i++)
            {
                string langCode = _headers[i].Trim().ToUpper();
                GameLanguage? lang = LangCodeToEnum(langCode);
                if (lang.HasValue)
                {
                    string text = columns[i].Trim().Replace("\\n", "\n");
                    langDict[lang.Value] = text;
                }
            }

            _table[key] = langDict;
        }

        Debug.Log($"[Localization] Tabla cargada: {_table.Count} claves.");

#if UNITY_EDITOR
        ValidateTranslations();
#endif
    }

#if UNITY_EDITOR
    private void ValidateTranslations()
    {
        if (_headers == null) return;

        foreach (var entry in _table)
        {
            for (int i = 1; i < _headers.Length; i++)
            {
                GameLanguage? lang = LangCodeToEnum(_headers[i].Trim().ToUpper());
                if (!lang.HasValue) continue;

                if (!entry.Value.TryGetValue(lang.Value, out string text) || string.IsNullOrEmpty(text))
                    Debug.LogWarning($"[Localization] '{entry.Key}' no tiene traducción a {lang.Value}.");
            }
        }
    }
#endif

    private GameLanguage? LangCodeToEnum(string code) =>
        code switch
        {
            "ES" => GameLanguage.Spanish,
            "EN" => GameLanguage.English,
            _    => (GameLanguage?)null
        };

    private string[] ParseCSVLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields.ToArray();
    }

    // ── Persistencia ─────────────────────────────────────────

    private const string PREF_KEY = "MKPZ_Language";

    private void SaveLanguage() =>
        PlayerPrefs.SetString(PREF_KEY, _currentLanguage.ToString());

    private void LoadSavedLanguage()
    {
        string saved = PlayerPrefs.GetString(PREF_KEY, defaultLanguage.ToString());
        _currentLanguage = Enum.TryParse(saved, out GameLanguage lang) ? lang : defaultLanguage;
    }
}
