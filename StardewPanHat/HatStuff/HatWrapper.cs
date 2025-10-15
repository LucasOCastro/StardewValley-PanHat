using System.Xml.Serialization;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewPanHat.HatStuff;

[XmlType($"Mods_{ModEntry.ModAuthorName}_{nameof(HatWrapper)}")]
public class HatWrapper : Object
{
    [XmlElement(nameof(InternalHatNetRef))]
    public readonly NetRef<Hat> InternalHatNetRef = new();
    
    public Hat InternalHat => InternalHatNetRef.Value;

    public override string TypeDefinitionId => ItemRegistry.type_hat;

    public HatWrapper()
    {
    }

    public HatWrapper(Hat hat) : base(hat.ItemId, 1)
    {
        InternalHatNetRef.Value = hat;
    }

    protected override void initNetFields()
    {
        base.initNetFields();
        NetFields.AddField(InternalHatNetRef, nameof(InternalHatNetRef));
    }
}