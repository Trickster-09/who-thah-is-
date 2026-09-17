using System.Collections.Generic;
using System.Text;

// ============================================================
// CsvUtils.cs
// Parser de una linea de CSV compartido, respeta comillas y comas
// embebidas. Lo usa InterrogationDatabase; a futuro LocalizationManager
// podria migrar a este mismo parser en vez de mantener su propia copia
// (no se tocó en esta pasada para no romper nada ya probado y commiteado).
// ============================================================

public static class CsvUtils
{
    public static string[] ParseLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var current = new StringBuilder();

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
}
