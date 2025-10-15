using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewPanHat.HatStuff;

public static class PanAttachmentSlots
{
    public const int Hat = 0;
    
    public static void SetupAttachmentSlot(Pan pan)
    {
        if (pan.AttachmentSlotsCount == 0)
        {
            pan.AttachmentSlotsCount = 1;
            return;
        }

        ModEntry.MonitorSingleton.Log(
            "Pan already has attachment slots, added by another mod or an update.",
            LogLevel.Error
        );
    }
    
    public static bool TryGetAttachedHat(Pan pan, [NotNullWhen(true)] out Hat? hat)
    {
        if (pan.attachments[Hat] is HatWrapper wrapper)
        {
            hat = wrapper.InternalHat;
            return true;
        }

        hat = null;
        return false;
    }
}