using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewPanHat.HatStuff;
using StardewPanHat.Patches;

namespace StardewPanHat;

internal class ModEntry : Mod
{
    public const string ModAuthorName = "Malkavian242";
    public static IMonitor MonitorSingleton { get; private set; } = null!;
    private static string _keyQualifier = null!;

    /// <summary> Adds the unique mod ID to the beginning of a key. </summary>
    public static string QualifyKey(string key) => _keyQualifier + key; 
    
    public override void Entry(IModHelper helper)
    {
        MonitorSingleton = Monitor;
        _keyQualifier = ModManifest.UniqueID + '/';
        
        HandleHarmonyPatches();
        helper.Events.GameLoop.GameLaunched += RegisterSerializableTypes;
        helper.Events.Multiplayer.PeerContextReceived += VerifyPeerMods;
    }

    private void HandleHarmonyPatches()
    {
        Harmony harmony = new(ModManifest.UniqueID);
        PanPatches.PatchAll(harmony);
        ToolPatches.PatchAll(harmony);
        UtilityPatches.PatchAll(harmony);
        InventoryMenuPatches.PatchAll(harmony);
        InventoryPagePatches.PatchAll(harmony);
    }

    private void RegisterSerializableTypes(object? sender, GameLaunchedEventArgs evt)
    {
        var api = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
        if (api == null)
            throw new($"{ModManifest.UniqueID} requires SpaceCore!");
        
        api.RegisterSerializerType(typeof(HatWrapper));
        Monitor.Log($"{ModManifest.UniqueID} registered ${nameof(HatWrapper)} with SpaceCore.");
    }
    
    private void VerifyPeerMods(object? sender, PeerContextReceivedEventArgs evt)
    {
        if (evt.Peer.GetMod(ModManifest.UniqueID) == null)
            Monitor.Log($"Player with ID {evt.Peer.PlayerID} does not have {ModManifest.UniqueID} installed.");
    }
}