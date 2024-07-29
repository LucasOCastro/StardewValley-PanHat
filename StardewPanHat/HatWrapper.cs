using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewPanHat;

public class HatWrapper : Object
{
    public Hat InternalHat { get; }

    public override string DisplayName => InternalHat.DisplayName;

    public override string TypeDefinitionId => "(H)";

    public HatWrapper(Hat hat) : base(hat.ItemId, 1)
    {
        InternalHat = hat;
    }
}