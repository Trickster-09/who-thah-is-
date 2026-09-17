// ============================================================
// GameLanguage.cs
// Enum central de idiomas del juego.
//
// Para añadir un idioma nuevo (ej. Francés):
//   1. Agrega el valor aquí (ej. French)
//   2. Agrega la columna correspondiente en strings.csv (ej. "FR")
//   3. Agrega un case en LocalizationManager.LangCodeToEnum()
//
// Nota: la documentación previa de este archivo mencionaba agregar un
// campo a una struct "LocalizedString" (ej. "public string fr"). Esa
// struct ya no existe en el sistema actual — la tabla se arma como
// Dictionary<GameLanguage, string> por clave, así que ese paso ya no
// aplica. Dejamos esta nota para que no se siga esa instrucción vieja.
// ============================================================

public enum GameLanguage
{
    Spanish,    // Español (idioma por defecto)
    English     // English
}
