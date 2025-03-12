using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

namespace Kharazmi.AspNetCore.Web.ActionResults
{
    /// <summary>
    /// An ASP.NET Core RSS Feed Renderer.
    /// </summary>
    public class FeedResult : ActionResult
    {
        private const string Atom10Namespace = "https://www.w3.org/2005/Atom";
        private readonly List<SyndicationAttribute> _attributes = new List<SyndicationAttribute>
                {
                   new SyndicationAttribute("xmlns:atom", Atom10Namespace)
                };

        private readonly FeedChannel _feedChannel;
        private IHttpRequestInfoService _httpContextInfo;

        /// <summary>
        /// An ASP.NET Core RSS Feed Renderer.
        /// </summary>
        /// <param name="feedChannel">Channel's info</param>
        public FeedResult(FeedChannel feedChannel)
        {
            _feedChannel = feedChannel;
        }

        /// <summary>
        /// Executes the result operation of the action method asynchronously.
        /// </summary>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _httpContextInfo = context.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();
            await writeSyndicationFeedToResponseAsync(context).ConfigureAwait(false);
        }

        private async Task writeSyndicationFeedToResponseAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            var mediaType = new MediaTypeHeaderValue("application/rss+xml")
            {
                CharSet = Encoding.UTF8.WebName
            };
            response.ContentType = mediaType.ToString();

            var xws = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, Async = true };
            using (var xmlWriter = XmlWriter.Create(response.Body, xws))
            {
                var formatter = new RssFormatter(_attributes, xmlWriter.Settings);
                var rssFeedWriter = await getRssFeedWriterAsync(xmlWriter).ConfigureAwait(false);
                await writeSyndicationItemsAsync(formatter, rssFeedWriter).ConfigureAwait(false);
                await xmlWriter.FlushAsync().ConfigureAwait(false);
            }
        }

        private async Task writeSyndicationItemsAsync(RssFormatter formatter, RssFeedWriter rssFeedWriter)
        {
            foreach (var item in getSyndicationItems())
            {
                var content = new SyndicationContent(formatter.CreateContent(item));
                content.AddField(new SyndicationContent("atom:updated", Atom10Namespace, item.LastUpdated.ToString("r")));
                await rssFeedWriter.Write(content).ConfigureAwait(false);
            }
        }

        private async Task<RssFeedWriter> getRssFeedWriterAsync(XmlWriter xmlWriter)
        {
            var rssFeedWriter = new RssFeedWriter(xmlWriter, _attributes);
            await addChannelIdentityAsync(rssFeedWriter).ConfigureAwait(false);
            await addChannelLastUpdatedTimeAsync(rssFeedWriter).ConfigureAwait(false);
            await addChannelImageAsync(rssFeedWriter).ConfigureAwait(false);
            return rssFeedWriter;
        }

        private async Task addChannelLastUpdatedTimeAsync(RssFeedWriter rssFeedWriter)
        {
            if (_feedChannel.RssItems == null || !_feedChannel.RssItems.Any())
            {
                return;
            }

            await rssFeedWriter.WriteLastBuildDate(
                _feedChannel.RssItems.OrderByDescending(x => x.LastUpdatedTime).First().LastUpdatedTime).ConfigureAwait(false);
        }

        private async Task addChannelIdentityAsync(RssFeedWriter rssFeedWriter)
        {
            await rssFeedWriter.WriteDescription(_feedChannel.FeedDescription.RemoveHexadecimalSymbols()).ConfigureAwait(false);
            await rssFeedWriter.WriteCopyright(_feedChannel.FeedCopyright.RemoveHexadecimalSymbols()).ConfigureAwait(false);
            await rssFeedWriter.WriteTitle(_feedChannel.FeedTitle.RemoveHexadecimalSymbols()).ConfigureAwait(false);
            await rssFeedWriter.WriteLanguage(new CultureInfo(_feedChannel.CultureName)).ConfigureAwait(false);
            await rssFeedWriter.WriteRaw($"<atom:link href=\"{_httpContextInfo.GetRawUrl()}\" rel=\"self\" type=\"application/rss+xml\" />").ConfigureAwait(false);
            await rssFeedWriter.Write(new SyndicationLink(_httpContextInfo.GetBaseUri(), relationshipType: RssElementNames.Link)).ConfigureAwait(false);
        }

        private async Task addChannelImageAsync(RssFeedWriter rssFeedWriter)
        {
            if (string.IsNullOrWhiteSpace(_feedChannel.FeedImageContentPath))
            {
                return;
            }

            var syndicationImage = new SyndicationImage(_httpContextInfo.AbsoluteContent(_feedChannel.FeedImageContentPath))
            {
                Title = _feedChannel.FeedImageTitle,
                Link = new SyndicationLink(_httpContextInfo.AbsoluteContent(_feedChannel.FeedImageContentPath))
            };
            await rssFeedWriter.Write(syndicationImage).ConfigureAwait(false);
        }

        private IEnumerable<SyndicationItem> getSyndicationItems()
        {
            foreach (var item in _feedChannel.RssItems)
            {
                var uri = new Uri(QueryHelpers.AddQueryString(item.Url,
                                new Dictionary<string, string>
                                {
                                    { "utm_source", "feed" },
                                    { "utm_medium", "rss" },
                                    { "utm_campaign", "featured" },
                                    { "utm_updated", getUpdatedStamp(item) }
                                }));
                var syndicationItem = new SyndicationItem
                {
                    Title = item.Title.RemoveHexadecimalSymbols(),
                    Id = uri.ToString(),
                    Description = item.Content.WrapInDirectionalDiv().RemoveHexadecimalSymbols(),
                    Published = item.PublishDate,
                    LastUpdated = item.LastUpdatedTime
                };
                syndicationItem.AddLink(new SyndicationLink(uri));
                syndicationItem.AddContributor(new SyndicationPerson(item.AuthorName, item.AuthorName));
                foreach (var category in item.Categories)
                {
                    syndicationItem.AddCategory(new SyndicationCategory(category));
                }
                yield return syndicationItem;
            }
        }

        private static string getUpdatedStamp(FeedItem item)
        {
            return item.LastUpdatedTime.ToString();
        }
    }
}