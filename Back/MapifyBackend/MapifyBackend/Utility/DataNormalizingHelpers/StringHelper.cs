namespace MapifyBackend.Utility.DataNormalizingHelpers;

public static class StringHelper
{
    public static string Capitalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return char.ToUpper(value[0]) + value[1..].ToLower();
    }
}