using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MIMESenderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Add Logic App URL
            var url = "https://prod-11.australiaeast.logic.azure.com:443/workflows/XXXX/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=YYYYY";

            //TODO: Add multipart boundary 
            var boundary = "------=_Part_111_1187512472.1527646642182";

            //TODO: Assign Part0 Content-ID
            var part0contentId = "<1956585333.1527646642182.apache-soap.MUCDWMV12>";

            //TODO: Assign Part1 (TripPlanOutput) Content-ID
            var part1contentId = "<2014414661.1527646642182.apache-soap.MUCDWMV12>";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(url));

            requestMessage.Headers.ExpectContinue = false;

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent(boundary);

            XmlDocument document0 = new XmlDocument();
            document0.Load(@".\files\SOAPMessage.xml");            
            var part0 = new StringContent(document0.OuterXml);
            part0.Headers.Remove("Content-Type");
            part0.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            part0.Headers.Add("Content-Transfer-Encoding", "8bit");
            part0.Headers.Add("Content-ID", part0contentId);
            part0.Headers.ContentLength = 470;
            multiPartContent.Add(part0);

            XmlDocument document1 = new XmlDocument();
            document1.Load(@".\files\tripplan.xml");
            var part1 = new StringContent(document1.OuterXml);
            part1.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            part1.Headers.Add("Content-Transfer-Encoding", "8bit");
            part1.Headers.Add("Content-ID", part1contentId);
            part1.Headers.ContentLength = 79553;
            multiPartContent.Add(part1);

            multiPartContent.Headers.Remove("Content-Type");
            multiPartContent.Headers.Add("Content-Type", "multipart/related; boundary=\""+ boundary +"\"");

            requestMessage.Content = multiPartContent;

            var contentBody = requestMessage.Content.ReadAsStringAsync().Result;
            var header = requestMessage.Headers.ToString();

            var httpClient = new HttpClient();

            Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
            var httpResponse = httpRequest.Result;

            Console.WriteLine($"Response code {httpResponse.StatusCode} ");               

            Console.ReadLine();


        }

    }
}
