using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PanComido.Infraestructura.ServiciosExternos
{
    internal class MetricCard : IComponent
    {
        private readonly string _title;
        private readonly string _value;
        private readonly string _change;

        public MetricCard(string title, string value, string change)
        {
            _title = title;
            _value = value;
            _change = change;
        }

        public void Compose(IContainer container)
        {
            container
                .Border(1)
                .BorderColor("#E2E8F0")
                .Background("#FFFFFF")
                .Padding(14)
                .Column(col =>
                {
                    col.Spacing(4);
                    col.Item().Text(_title.ToUpperInvariant()).FontSize(7.5f).FontColor("#64748B").Bold();
                    col.Item().Text(_value).FontSize(20).Bold().FontColor("#0F172A");
                    col.Item().Text(_change).FontSize(8).FontColor(_change.Contains("-") ? "#D8081C" : "#6ABF3F").Bold();
                });
        }
    }

    internal class SummaryCard : IComponent
    {
        private readonly string _title;
        private readonly string _value;
        private readonly string _detail;
        private readonly string _accentColor;

        public SummaryCard(string title, string value, string detail, string accentColor)
        {
            _title = title;
            _value = value;
            _detail = detail;
            _accentColor = accentColor;
        }

        public void Compose(IContainer container)
        {
            container
                .Border(1)
                .BorderColor("#E2E8F0")
                .Background("#FFFFFF")
                .Padding(12)
                .Column(col =>
                {
                    col.Spacing(4);
                    col.Item().Text(_title.ToUpperInvariant()).FontSize(7.5f).FontColor("#64748B").Bold();
                    col.Item().Text(_value).FontSize(18).Bold().FontColor(_accentColor);
                    col.Item().Text(_detail).FontSize(8).FontColor("#475569");
                });
        }
    }
}
