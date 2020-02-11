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
    public class CommitController : ControllerBase
    {
        private readonly ILogger<CommitController> _logger;

        public CommitController(ILogger<CommitController> logger)
        {
            _logger = logger;
        }

        private const string ICON = "i2800";
        private const string ICON_ERROR = "i2493";

        [HttpGet]
        public async Task<LametricRootObject> Get([FromQuery]string repository,[FromQuery]string branch, [FromQuery]int last,[FromQuery] string displayname)
        {
            var root = new LametricRootObject();
            root.frames = new List<Frame>();
            try
            {
                if (last<1)
                    {
                        throw new LastValueException();
                    }

                HttpClient client = new HttpClient();
            
                client.DefaultRequestHeaders.Accept .Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "github-to-lametric");

                var stringTask = client.GetStringAsync($"https://github.com/{repository}/commits/{branch}.atom");
                var xml = await stringTask;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                string json  = JsonConvert.SerializeXmlNode(doc);

                var atomRootObject =  JsonConvert.DeserializeObject<AtomRootObject>(json);
                
                for (int i=0; i<last; i++)
                {
                    if (atomRootObject.feed.entry.Count>=i)
                    {
                        var f = new Frame();
                        f.icon = ICON;
                        f.index = i;
                        f.text = $"{displayname}: {atomRootObject.feed.entry[i].author.name} have commited '{ cleanTitle(atomRootObject.feed.entry[i].title)}' { DateTime.UtcNow.Add(atomRootObject.feed.entry[i].updated-DateTime.Now).Humanize() }";

                        root.frames.Add(f);
                    }
                }

            }
            catch (HttpRequestException e)
            {
                root.frames.Add(new Frame(999, ICON_ERROR, $"ERR01: {repository}/{branch} - {e.Message}"));
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
