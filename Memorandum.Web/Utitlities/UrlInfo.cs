using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;

namespace Memorandum.Web.Utitlities
{
    class UrlInfoParser : IDisposable
    {
        public string Title { get; }
        public string FileName { get; }
        public string ImageUrl { get; }
        public string ImageFileName => ImageUrl != null ? Path.GetFileName(new Uri(ImageUrl).LocalPath) : null;

        public MemoryStream ContentStream => _outputStream;

        static readonly Regex TitleRegex = new Regex(@"(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase);
        static readonly Regex OgImageRegex = new Regex("(?:og:image[^<]+content\\s?=\\s?\")([^\"]*)(?:\")", RegexOptions.IgnoreCase);
        static readonly Regex AppleIconRegex = new Regex("(?:rel[^<]+apple-touch-icon[^<]+href\\s?=\\s?\")([^\"]*)(?:\")", RegexOptions.IgnoreCase);
        static readonly Regex FaviconRegex = new Regex("(?:rel[^<]+icon[^<]+href\\s?=\\s?\")([^\"]*)(?:\")", RegexOptions.IgnoreCase);
        private readonly MemoryStream _outputStream;


        public UrlInfoParser(string url)
        {
            var requestUri = new Uri(url, UriKind.Absolute);
            Title = Path.GetFileName(requestUri.LocalPath);

            var request = WebRequest.Create(requestUri.AbsoluteUri) as HttpWebRequest;
            if (request == null)
                throw new InvalidOperationException("Invalid Url");

            request.UseDefaultCredentials = true;
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException)
            {
                throw new InvalidOperationException("Can't load URL");
            }

            if (response == null)
                throw new InvalidOperationException("Can't load URL");

            Title = Path.GetFileName(response.ResponseUri.LocalPath);
            FileName = Path.GetFileName(response.ResponseUri.LocalPath);
            if (!Path.HasExtension(FileName))
            {
                var ct = response.ContentType.Split(';')[0];
                FileName +=
                    MimeTypes.MimeTypeMap.GetExtension(ct);
            }

            _outputStream = new MemoryStream();
            using (var responseStream = response.GetResponseStream())
            {
                if(responseStream == null)
                    throw new InvalidOperationException("Can't load URL");
                responseStream.CopyTo(_outputStream);
            }

            // Try to get title from text html
            if (response.ContentType.StartsWith("text/html"))
            {
                var encoding = response.CharacterSet != null ? Encoding.GetEncoding(response.CharacterSet) : Encoding.UTF8;
                var page = encoding.GetString(_outputStream.ToArray());
                Title = GetTitleFromWebPage(page);
                var imageRelPath = GetImageFromWebPage(requestUri, page);
                ImageUrl = new Uri(requestUri, new Uri(imageRelPath, UriKind.RelativeOrAbsolute)).AbsoluteUri;
            }
            // Try to get title from pdf meta
            else if (response.ContentType.Contains("pdf") || Path.GetExtension(response.ResponseUri.LocalPath).Equals(".pdf"))
            {
                var pdf = new PdfReader(_outputStream.ToArray());
                if (pdf.Info.ContainsKey("Title"))
                {
                    var title = pdf.Info["Title"];
                    if (!string.IsNullOrWhiteSpace(title) &&
                        !title.Trim().ToLower().Equals("untitled"))
                        Title = title;
                }
            }
            // Try to get title from content disposition
            else
            {
                var cd = response.GetResponseHeader("Content-Disposition");
                if (!string.IsNullOrEmpty(cd))
                {
                    var filename = new ContentDisposition(cd).FileName;
                    if (!string.IsNullOrEmpty(filename))
                    {
                        Title = WebUtility.UrlDecode(filename);
                        FileName = WebUtility.UrlDecode(filename);
                    }
                }
            }

            response.Close();
        }

        public void SaveContent(string path)
        {
            using (var writer = File.OpenWrite(path))
            {
                _outputStream.Seek(0, SeekOrigin.Begin);
                _outputStream.CopyTo(writer);
            }
        }
        public void SaveImage(string path)
        {
            using (var web = new WebClient())
            {
                web.DownloadFile(ImageUrl, path);
            }
        }

        private static string GetTitleFromWebPage(string page)
        {
            return TitleRegex.Match(page).Value.Trim();
        }

        private static string GetImageFromWebPage(Uri uri, string page)
        {
            var match = OgImageRegex.Match(page);
            if (match.Success)
                return match.Groups[1].Value.Trim();
            
            match = AppleIconRegex.Match(page);
            if (match.Success)
                return match.Groups[1].Value.Trim();

            match = FaviconRegex.Match(page);
            if (match.Success)
                return match.Groups[1].Value.Trim();
            
            return uri.GetLeftPart(UriPartial.Authority) + "/favicon.ico";
        }

        public void Dispose()
        {
            _outputStream?.Close();
            _outputStream?.Dispose();
        }
    }
}
