using System.Xml.Serialization;
using Netcode;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewPanHat;

[XmlType($"Mods_{ModEntry.ModAuthorName}_{nameof(HatWrapper)}")]
public class HatWrapper : Object
{
    [XmlElement(nameof(_internalHat))]
    private readonly NetRef<Hat> _internalHat = new();
    public Hat InternalHat => _internalHat.Value;

    public override string DisplayName => InternalHat.DisplayName;

    public override string TypeDefinitionId => "(H)";

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