using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using github_to_lametric.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml;

namespace github_to_lametric.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommitController : ControllerBase
    {
        private readonly ILogger<CommitController> _logger;

        public CommitController(ILogger<CommitController> logger)
        {
            _logger = logger;
        }

        private const string ICON = "#2800";

        [HttpGet]
        public async Task<LametricRootObject> Get([FromQuery]string repository,[FromQuery]string branch, [FromQuery]int last,[FromQuery] string displayname)
        {
            HttpClient client = new HttpClient();
            
            client.DefaultRequestHeaders.Accept .Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "github-to-lametric");

            var stringTask = client.GetStringAsync($"https://github.com/{repository}/commits/{branch}.atom");
            var xml = await stringTask;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string json  = JsonConvert.SerializeXmlNode(doc);

            var atomRootObject =  JsonConvert.DeserializeObject<AtomRootObject>(json);
            
            var root = new LametricRootObject();
            root.frames = new List<Frame>();
            
            for (int i=0; i<last; i++)
            {
                if (atomRootObject.feed.entry.Count>=i)
                {
                    var f = new Frame();
                    f.icon = ICON;
                    f.index = i;
                    f.text = $"{displayname}: {atomRootObject.feed.entry[i].author.name} have commited { cleanTitle(atomRootObject.feed.entry[i].title)} { makeElapsedString(atomRootObject.feed.entry[i].updated)} ago";

                    root.frames.Add(f);
                }
            }

            return root;
        }

        private string cleanTitle(string title)
        {
            var output = title.Replace("\n", " ");
            return output.Trim();
        }

        private string makeElapsedString(DateTime updated)
        {
            string time = "";
            var diff = (DateTime.Now - updated);

            if (diff.TotalDays > 1)
            {
                time = $"{(int)diff.TotalDays} days";
            }
            else if (diff.TotalHours > 1)
            {
                time = $"{(int)diff.TotalHours} hours";
            }
            else if (diff.TotalMinutes > 1)
            {
                time = $"{(int)diff.TotalMinutes} minutes";
            }

            return time;
        }
    }
}
