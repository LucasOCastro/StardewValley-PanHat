using Netcode;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewPanHat;

public class HatWrapper : Object
{
    private readonly NetRef<Hat> _internalHat = new();
    public Hat InternalHat => _internalHat.Value;

    public override string DisplayName => InternalHat.DisplayName;

    public override string TypeDefinitionId => "(H)";

    public HatWrapper(Hat hat) : base(hat.ItemId, 1)
    {
        _internalHat.Value = hat;
    }

    protected override void initNetFields()
    {
        base.initNetFields();
        NetFields.AddField(_internalHat, nameof(_internalHat));
    }
}