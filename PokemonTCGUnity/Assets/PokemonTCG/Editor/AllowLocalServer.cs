using UnityEditor;

namespace PokemonTCG.EditorTools
{
    /// <summary>
    /// The game server runs on http://127.0.0.1, and Unity 6 refuses plain-HTTP requests by
    /// default, so allow them for this project when the editor loads.
    /// </summary>
    [InitializeOnLoad]
    static class AllowLocalServer
    {
        static AllowLocalServer()
        {
            if (PlayerSettings.insecureHttpOption != InsecureHttpOption.AlwaysAllowed)
                PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
        }
    }
}
