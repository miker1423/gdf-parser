using QuickGraph;

namespace GDFParser.Models
{
    public class GDFEdge : IEdge<GDFNode>
    {
        public GDFNode Source { get; set; }
        public GDFNode Target { get; set; }
    }
}
