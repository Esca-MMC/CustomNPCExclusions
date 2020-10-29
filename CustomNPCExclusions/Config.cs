using StardewModdingAPI;

namespace CustomNPCExclusions
{
    class Config
    {
        public SButton debugKey { get; set; }

        public Config()
        {
            debugKey = SButton.J;
        }
    }
}
