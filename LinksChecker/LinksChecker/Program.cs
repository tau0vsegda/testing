using AngleSharp;
using AngleSharp.Dom;
using System.Net;
using System.Text.RegularExpressions;

namespace MakeAGETRequest_charp
{
    class LinksChecker
    {
        private const string SOURCE_URL = "https://www.travelline.ru";
        //private const string SOURCE_URL = "http://links.qatl.ru";
        private const string COR_PATH = "D:\\study\\тестирование\\LinksChecker\\LinksChecker\\CorrectLinks.txt";
        private const string INCOR_PATH = "D:\\study\\тестирование\\LinksChecker\\LinksChecker\\IncorrectLinks.txt";
        private static HashSet<string> allLinks = new();
        private static HashSet<string> tempCorLinks = new();
        private static HashSet<string> nowLinks = new();
        private static int corLinks = 0;
        private static int incorLinks = 0;
        private static async Task<IDocument> GetDocument(string sourceUrl)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IDocument document = await BrowsingContext.New(config)
                .OpenAsync(sourceUrl);
            return document;
        }
        private static void AddEditLinks(HashSet<string> tempLinks, string link)
        {
            Regex reg1 = new Regex("//");
            Regex reg2 = new Regex(SOURCE_URL);
            Regex reg3 = new Regex("tel");
            Regex reg4 = new Regex("mailto");
            if (!(reg1.IsMatch(link) || reg3.IsMatch(link) || reg4.IsMatch(link)))
            {
                
                if (link.Length > 0 && link[0] != '/')
                {
                    link = '/' + link;
                }
                tempLinks.Add(SOURCE_URL + link);
            }
            else if (reg2.IsMatch(link))
            {
                tempLinks.Add(link);
            }
        }

        private static void ParseDocumentByAttribute(IDocument document, string attribute, HashSet<string> tempLinks)
        {
            IHtmlCollection<IElement> links = document.QuerySelectorAll("*[" + attribute + "]");
            foreach (IElement link in links)
            {
                string? newLink = link.GetAttribute(attribute);
                if (newLink != null)
                {
                    AddEditLinks(tempLinks, newLink);
                }

            }
        }
        private static HashSet<string> ParseDocument(IDocument document)
        {
            HashSet<string> nextLinks = new();
            ParseDocumentByAttribute(document, "href", nextLinks);
            ParseDocumentByAttribute(document, "src", nextLinks);
            return nextLinks;
        }
        private static int GetStatusCode(string link)
        {
            int statusCode;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link);
            webRequest.AllowAutoRedirect = false;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }
            statusCode = (int)response.StatusCode;
            return statusCode;
        }

        private static void CheckNowLinks(HashSet<string> nowLinks, HashSet<string> nextLinks, StreamWriter writerCor, StreamWriter writerIncor)
        {
            int statusCode;
            foreach (string link in nowLinks)
            {
                if (!allLinks.Contains(link))
                {
                    allLinks.Add(link);
                    statusCode = GetStatusCode(link);
                    if (statusCode < 300)
                    {
                        corLinks++;
                        writerCor.WriteLine("{0} {1}", link, statusCode);
                        nextLinks.Add(link);
                    }
                    else
                    {
                        incorLinks++;
                        writerIncor.WriteLine("{0} {1}", link, statusCode);
                    }
                }
                
            }
        }
        static async Task Main(string[] args)
        {

            using StreamWriter writerCor = new StreamWriter(COR_PATH);
            using StreamWriter writerIncor = new StreamWriter(INCOR_PATH);
            allLinks.Add(SOURCE_URL);
            if(GetStatusCode(SOURCE_URL) < 300)
            {
                tempCorLinks.Add(SOURCE_URL);
            }
            while (tempCorLinks.Count() > 0)
            {
                Console.WriteLine("{0} {1}",allLinks.Count, tempCorLinks.Count);
                foreach(string link in tempCorLinks)
                {
                    nowLinks.UnionWith(ParseDocument(await GetDocument(link)));
                }
                tempCorLinks.Clear();
                CheckNowLinks(nowLinks, tempCorLinks, writerCor, writerIncor);
            }
            DateTime date = DateTime.Now;
            writerCor.WriteLine("{0} correct links", corLinks);
            writerIncor.WriteLine("{0} incorrect links", incorLinks);
            writerCor.WriteLine("Date of checking: {0}", date);
            writerIncor.WriteLine("Date of checking: {0}", date);
        }
    }
}