using Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Services.Sitemap
{
    public class SitemapBuilder
    {
        private readonly XNamespace NS = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly List<SitemapUrl> _urls = new();

        public void AddUrl(string url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null)
        {
            _urls.Add(new SitemapUrl
            {
                Url = url,
                Modified = modified,
                ChangeFrequency = changeFrequency,
                Priority = priority
            });
        }

        public void WriteSitemap(Stream stream)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                CloseOutput = false // Prevents the writer from closing the stream
            };

            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", NS.NamespaceName);

                foreach (var url in _urls)
                {
                    WriteUrlElement(writer, url);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            stream.Position = 0; // Reset the stream position
        }

        private void WriteUrlElement(XmlWriter writer, SitemapUrl url)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", NS.NamespaceName, url.Url);

            if (url.Modified.HasValue)
            {
                writer.WriteElementString("lastmod", NS.NamespaceName,
                    url.Modified.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }

            if (url.ChangeFrequency.HasValue)
            {
                writer.WriteElementString("changefreq", NS.NamespaceName,
                    url.ChangeFrequency.Value.ToString().ToLower());
            }

            if (url.Priority.HasValue)
            {
                writer.WriteElementString("priority", NS.NamespaceName,
                    url.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture));
            }

            writer.WriteEndElement();
        }
    }

    public class SitemapUrl
    {
        public string Url { get; set; }
        public DateTime? Modified { get; set; }
        public ChangeFrequency? ChangeFrequency { get; set; }
        public double? Priority { get; set; }
    }

   
}