using System;
using UnityEngine;
using System.IO;

namespace EasyBugReporter {
    public interface IReportWriter {
        // beginning
        void Begin(Stream stream);
        void Begin(TextWriter writer);
        void End();

        // prelude
        void Prelude(string title, DateTime now);

        // sections
        void BeginSection(string sectionName, bool defaultOpen = true, ReportStyle style = default);
        void EndSection();
        
        // contents
        void Header(string text, ReportStyle style = default);
        void Text(string text, ReportStyle style = default);
        void InlineText(string text, ReportStyle style = default);
        void Image(Texture2D texture, string imageName = null);
        
        // spacing
        void NextLine();
        void Space();

        // Layout
        bool SupportsTables { get; }
        void BeginTable();
        void EndTable();
        void BeginTableRow(ReportStyle style = default);
        void EndTableRow();
        void BeginTableCell(ReportStyle style = default);
        void EndTableCell();
    }

    public struct ReportStyle {
        public FontStyle FontStyle;
        public Color32 Color;
        public Color32 BackgroundColor;

        public ReportStyle(FontStyle style, Color32 color, Color32 bg) {
            FontStyle = style;
            Color = color;
            BackgroundColor = bg;
        }

        static public implicit operator ReportStyle(Color color) {
            return new ReportStyle(FontStyle.Normal, color, default);
        }

        static public implicit operator ReportStyle(Color32 color) {
            return new ReportStyle(FontStyle.Normal, color, default);
        }

        static public implicit operator ReportStyle(FontStyle style) {
            return new ReportStyle(style, default, default);
        }
    }

    static public class ReportUtils {
        static public readonly ReportStyle DefaultTableHeaderStyle = new ReportStyle(FontStyle.Bold, default, default);

        static public string TextureToBase64(Texture2D texture) {
            return Convert.ToBase64String(texture.EncodeToPNG());
        }

        static public string TextureToBase64Html(Texture2D texture) {
            return "data:image/png;base64," + TextureToBase64(texture);
        }

        static public void TableCellText(this IReportWriter writer, string text, ReportStyle style = default) {
            writer.BeginTableCell();
            writer.InlineText(text, style);
            writer.EndTableCell();
        }

        static public void TableCellHeader(this IReportWriter writer, string text) {
            writer.BeginTableCell();
            writer.InlineText(text, DefaultTableHeaderStyle);
            writer.EndTableCell();
        }

        static public void TableCellHeader(this IReportWriter writer, string text, ReportStyle style) {
            writer.BeginTableCell();
            writer.InlineText(text, style);
            writer.EndTableCell();
        }
    }

    public enum ReportFormat : byte {
        Html = 0,
        Text = 1,

        Custom = 255
    }
}