// ============================================================
// LocalizationKeys.cs
// Constantes que corresponden a las claves del strings.csv.
//
// Por qué existe este archivo:
//   Sin él, escribirías LocalizationManager.Get("trait.empathy.name")
//   en 10 lugares distintos. Si algún día cambias la clave en el CSV
//   tendrías que buscar y reemplazar en todo el proyecto.
//   Con este archivo cambias solo una línea aquí.
//
//   También evita typos: "trait.emapthy.name" compilaría sin error
//   pero fallaría en runtime. LK.Traits.EMPATHY_NAME no puede tener typos.
//
// Convención: LK = abreviatura de LocalizationKeys (menos verboso).
//
// NOTA: Traits y Questions (trait.*, q01/q02) parecen venir de un
// proyecto anterior (test de personalidad), no del juego de guardia de
// base. Se mantienen porque sirven para validar que el sistema
// compila y corre de punta a punta. LK.Interrogation es el primer
// bloque de claves reales del juego (grafo de interrogatorio de
// prueba con una contradicción: soldado que dice nunca haber estado
// en la base).
// ============================================================

public static class LK
{
    // ── Rasgos de personalidad ───────────────────────────────
    public static class Traits
    {
        // Empatía
        public const string EMPATHY_NAME    = "trait.empathy.name";
        public const string EMPATHY_SHORT   = "trait.empathy.short";
        public const string EMPATHY_RESULT  = "trait.empathy.result";
        public const string EMPATHY_FLAVOR  = "trait.empathy.flavor";

        // Razón
        public const string REASON_NAME     = "trait.reason.name";
        public const string REASON_SHORT    = "trait.reason.short";
        public const string REASON_RESULT   = "trait.reason.result";
        public const string REASON_FLAVOR   = "trait.reason.flavor";

        // Instinto
        public const string INSTINCT_NAME   = "trait.instinct.name";
        public const string INSTINCT_SHORT  = "trait.instinct.short";
        public const string INSTINCT_RESULT = "trait.instinct.result";
        public const string INSTINCT_FLAVOR = "trait.instinct.flavor";

        // Voluntad
        public const string WILL_NAME       = "trait.will.name";
        public const string WILL_SHORT      = "trait.will.short";
        public const string WILL_RESULT     = "trait.will.result";
        public const string WILL_FLAVOR     = "trait.will.flavor";
    }

    // ── Preguntas del test (legado, ver nota arriba) ─────────
    // Patrón: Q[número].narrator / Q[número].text / Q[número].answer.[índice]
    public static class Questions
    {
        public const string Q01_NARRATOR = "q01.narrator";
        public const string Q01_TEXT     = "q01.text";
        public const string Q01_A0       = "q01.answer.0";
        public const string Q01_A1       = "q01.answer.1";
        public const string Q01_A2       = "q01.answer.2";
        public const string Q01_A3       = "q01.answer.3";

        public const string Q02_NARRATOR = "q02.narrator";
        public const string Q02_TEXT     = "q02.text";
        public const string Q02_A0       = "q02.answer.0";
        public const string Q02_A1       = "q02.answer.1";
        public const string Q02_A2       = "q02.answer.2";
        public const string Q02_A3       = "q02.answer.3";
    }

    // ── UI general ───────────────────────────────────────────
    public static class UI
    {
        public const string TEST_PROGRESS       = "ui.test.progress";
        public const string TEST_NAME_PROMPT    = "ui.test.name.prompt";
        public const string TEST_NAME_HOLDER    = "ui.test.name.placeholder";
        public const string TEST_NAME_CONFIRM   = "ui.test.name.confirm";
        public const string TEST_RESULT_TITLE   = "ui.test.result.title";
        public const string TEST_RESULT_CONTINUE = "ui.test.result.continue";
    }

    // ── Nombres de idiomas (para el menú de opciones) ────────
    public static class Languages
    {
        public const string SPANISH = "lang.spanish";
        public const string ENGLISH = "lang.english";
    }

    // ── Interrogatorio en la base (contenido real del juego) ──
    // Usado por Assets/_Project/Data/DialogueGraphs/interrogation_*.csv
    public static class Interrogation
    {
        public const string Q1_TEXT           = "interrogation.q1.text";
        public const string Q1_ANSWER_SOLDIER = "interrogation.q1.answer.soldier";
        public const string Q1_ANSWER_TRADER  = "interrogation.q1.answer.trader";
        public const string Q1_ANSWER_REFUGEE = "interrogation.q1.answer.refugee";

        public const string Q2_TEXT              = "interrogation.q2.text";
        public const string Q2_ANSWER_NEVER_BEEN = "interrogation.q2.answer.never_been";
        public const string Q2_ANSWER_LONG_ABSENCE = "interrogation.q2.answer.long_absence";
    }
}
