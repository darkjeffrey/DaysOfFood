using System.Linq;
using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs;

namespace DaysOfFood
{
    /// <summary>
    /// Swaps each value text in SandBox's MapBar prefab for DofMapBarValueTextWidget, keeping every attribute,
    /// binds its switches to FoodColorMixin, and gives it a hidden separator line centered between two text lines.
    /// </summary>
    [PrefabExtension("MapBar", "descendant::MapBarCustomValueTextWidget")]
    public sealed class MapBarPrefabPatch : CustomPatch<XmlDocument>
    {
        private const string Separator =
            "<Children><Widget Id=\"" + DofMapBarValueTextWidget.SeparatorId + "\" IsVisible=\"false\" DoNotAcceptEvents=\"true\" " +
            "WidthSizePolicy=\"StretchToParent\" HeightSizePolicy=\"Fixed\" SuggestedHeight=\"2\" VerticalAlignment=\"Center\" " +
            "MarginLeft=\"2\" MarginRight=\"2\" Sprite=\"BlankWhiteSquare_9\" Color=\"#F4E1C499\" /></Children>";

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
                widget.SetAttribute("DofSeparator", "@DofSeparator");
                widget.InnerXml = Separator;
                old.ParentNode.ReplaceChild(widget, old);
            }
        }
    }
}
