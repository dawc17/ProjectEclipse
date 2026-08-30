using System;

public static class OfflineServices
{
    public const string Unavailable = "Service unavailable in this offline build.";

    // Only filesystem content is allowed through legacy download helpers.
    public static bool IsLocalContent(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        if (path.StartsWith("jar:file:", StringComparison.OrdinalIgnoreCase))
            path = path.Substring(4);
        Uri uri;
        return Uri.TryCreate(path, UriKind.Absolute, out uri) && uri.IsFile &&
            !uri.IsUnc && string.IsNullOrEmpty(uri.Host);
    }

    public static void OpenExternalUrl(string url)
    {
        // Recovered storefront/support/news/quest links do not launch a browser.
    }
}
