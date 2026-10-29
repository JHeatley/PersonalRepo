namespace PokemonTCG.AI
{
    /// <summary>
    /// The version of the training code, which the app and the background trainer (Tools/sim-train/sim-train.exe, built from
    /// the same code) both carry. The app runs the background trainer only if it reports at least this version
    /// ("sim-train version"): an older one may not read the AI's file as it is now (a bigger network, say) and would save its
    /// own idea of it over the file. Raise it when the brain's files or the background job change.
    /// </summary>
    public static class TrainerVersion
    {
        public const int Number = 171;
    }
}
