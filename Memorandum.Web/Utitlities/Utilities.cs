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
    internal static class Utilities
    {
        public static string GetWebPageTitle(string url)
        {
            // Create a request to the url
            var request = WebRequest.Create(url) as HttpWebRequest;

            if (request == null) return null;

            // Use the user's credentials
            request.UseDefaultCredentials = true;

            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException)
            {
                return Path.GetFileName(new Uri(url).LocalPath);
            }

            if (response == null)
                return Path.GetFileName(new Uri(url).LocalPath);


            // Regular expression for an HTML title
            const string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            var headers = new List<string>(response.Headers.AllKeys);
            
            // Try to get title from text html
            if (response.ContentType.StartsWith("text/html"))
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                var page = encoding.GetString(FromResponseStream(response.GetResponseStream()));
                var ex = new Regex(regex, RegexOptions.IgnoreCase);
                response.Close();
                return ex.Match(page).Value.Trim();
            }

            // Try to get title from pdf meta
            if (response.ContentType.Contains("pdf") || Path.GetExtension(response.ResponseUri.LocalPath).Equals("pdf"))
            {
                var pdf = new PdfReader(FromResponseStream(response.GetResponseStream()));
                if (pdf.Info.ContainsKey("Title"))
                {
                    var title = pdf.Info["Title"];
                    if (!string.IsNullOrWhiteSpace(title) && 
                        !title.Trim().ToLower().Equals("untitled"))
                        return title;
                }
            }
            response.Close();

            // Try to get title from content disposition
            if (headers.Contains("Content-Disposition"))
            {
                var cd = response.Headers["content-disposition"];
                if (!string.IsNullOrEmpty(cd))
                {
                    var filename = new ContentDisposition(cd).FileName;
                    if (!string.IsNullOrEmpty(filename))
                        return filename;
                }
            }

            // Get from URI finally
            return Path.GetFileName(response.ResponseUri.LocalPath);
        }
    
        public static byte[] FromResponseStream(Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
    }
}