using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using github_to_lametric.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Xml;
using github_to_lametric.Helpers.Exceptions;
using Humanizer;

namespace github_to_lametric.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserActivityController : ControllerBase
    {
        private readonly ILogger<CommitController> _logger;

        public UserActivityController(ILogger<CommitController> logger)
        {
            _logger = logger;
        }

        private const string ICON = "i2800";
        private const string ICON_ERROR = "i2493";

        [HttpGet]
        public async Task<LametricRootObject> Get([FromQuery]string username, [FromQuery]int last)
        {
            var root = new LametricRootObject();
            root.frames = new List<Frame>();
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new UserMissingException();
                }

                if (last<1)
                {
                    throw new LastValueException();
                }


                HttpClient client = new HttpClient();
            
                client.DefaultRequestHeaders.Accept .Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "github-to-lametric");

                var url = $"https://github.com/{username}.atom";
                var stringTask = client.GetStringAsync(url);
                var xml = await stringTask;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                var entries = doc.GetElementsByTagName("entry");
                
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ns", doc.DocumentElement.NamespaceURI);
                
                for (int i=0; i<last; i++)
                {
                    var f = new Frame();
                    f.icon = ICON;
                    f.index = i;

                    var nodedate = entries[i].SelectNodes("ns:updated", nsmgr)[0];
                    var date = Convert.ToDateTime(nodedate.InnerText);
                    var dateDiff = DateTime.Now.Add(date-DateTime.Now);

                    var title = entries[i].SelectNodes("ns:title", nsmgr)[0].InnerText;
                    f.text = $"{ cleanTitle(title)}' { dateDiff.Humanize() }";

                    root.frames.Add(f);
                
                }

            }
            catch (HttpRequestException e)
            {
                root.frames.Add(new Frame(999, ICON_ERROR, $"ERR01: {username} - {e.Message}"));
            }
            catch (MyManagedException e)
            {
                root.frames.Add(new Frame(999, ICON_ERROR, e.Message));
            }
            catch (Exception e)
            {
                root.frames.Add(new Frame(999, ICON_ERROR, $"ERR99: {e.Message}"));
            }
            
            return root;
        }

        private string cleanTitle(string title)
        {
            return title.Replace("\r", " ").Replace("\n", " ").Trim();
        }

    }
}
