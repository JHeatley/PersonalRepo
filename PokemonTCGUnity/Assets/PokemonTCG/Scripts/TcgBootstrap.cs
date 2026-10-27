using UnityEngine;

namespace PokemonTCG
{
    /// <summary>Creates the front end in whatever scene is open, so pressing Play just works.</summary>
    public static class TcgBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (Object.FindFirstObjectByType<TcgApp>() == null)
                new GameObject("Pokemon TCG").AddComponent<TcgApp>();
        }
    }
}
