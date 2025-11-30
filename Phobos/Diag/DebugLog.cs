using System.Diagnostics;

namespace Phobos.Diag;

public static class DebugLog
{
    [Conditional("DEBUG")]
    public static void Write(string message)
    {
        Plugin.Log.LogInfo(message);
    }
}