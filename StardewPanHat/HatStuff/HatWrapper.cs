using System.Xml.Serialization;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewPanHat.HatStuff;

[XmlType($"Mods_{ModEntry.ModAuthorName}_{nameof(HatWrapper)}")]
public class HatWrapper : Object
{
    [XmlElement(nameof(_internalHat))]
    private readonly NetRef<Hat> _internalHat = new();
    public Hat InternalHat => _internalHat.Value;

    public override string TypeDefinitionId => ItemRegistry.type_hat;

    public HatWrapper()
    {
        _internalHat.Value = new();
    }

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