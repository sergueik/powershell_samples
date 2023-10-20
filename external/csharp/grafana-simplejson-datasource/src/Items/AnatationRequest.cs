namespace Grafana.SimpleJson.Example.Items
{
    public class AnatationRequest
    {
        public Range Range { get; set; }
        public RangeRaw RangeRaw { get; set; }
        public Annotation Annotation { get; set; }
    }

    public class Annotation
    {
        public string Name { get; set; }
        public string Datasource { get; set; }
        public string IconColor { get; set; }
        public bool Enable { get; set; }
        public string Query { get; set; }
    }
}
