using System.Linq;
using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs;

namespace DaysOfFood
{
    /// <summary>
    /// Swaps each value text in SandBox's MapBar prefab for DofMapBarValueTextWidget, keeping every attribute,
    /// and binds its DofBlinkReds to FoodColorMixin.
    /// </summary>
    [PrefabExtension("MapBar", "descendant::MapBarCustomValueTextWidget")]
    public sealed class MapBarPrefabPatch : CustomPatch<XmlDocument>
    {
        public override string Id => "DaysOfFoodMapBar";

        public override void Apply(XmlDocument document)
        {
            // ToList: replacing nodes while walking the live node list would skip some.
            foreach (XmlElement old in document.SelectNodes("//MapBarCustomValueTextWidget").OfType<XmlElement>().ToList())
            {
                XmlElement widget = document.CreateElement(nameof(DofMapBarValueTextWidget));
                foreach (XmlAttribute attribute in old.Attributes)
                    widget.SetAttribute(attribute.Name, attribute.Value);
                widget.SetAttribute("DofBlinkReds", "@DofBlinkReds");
                while (old.HasChildNodes)
                    widget.AppendChild(old.FirstChild);
                old.ParentNode.ReplaceChild(widget, old);
            }
        }
    }
}
