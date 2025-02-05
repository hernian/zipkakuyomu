using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Browser;
using AngleSharp.Dom;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Security.Policy;

namespace zipkakuyomu
{
    internal partial class KakuYomuNavigater(WebView2 webView2)
    {
        public class Error(string msg) : Exception(msg)
        {
        }

        private const string JS_GET_HTML = "document.documentElement.outerHTML";

        private readonly WebView2 _webView2 = webView2;

        public async Task NavigateAsync(ZipBook zipBook, string urlTitlePage, CancellationToken ct)
        {
            using var sem = new SemaphoreSlim(0);
            // semのスコープをこの関数内に閉じたいので、WebView2.NavigationCompletedイベントハンドラをローカル関数とする
            void eh(object? w, CoreWebView2NavigationCompletedEventArgs e)
            {
                Debug.WriteLine("Navigation Completed");
                sem.Release();
            }

            try
            {
                _webView2.NavigationCompleted += eh;

                var htmlParser = new HtmlParser();
                _webView2.CoreWebView2.Navigate(urlTitlePage);
                await sem.WaitAsync(ct);
                ct.ThrowIfCancellationRequested();
                var strTitleDoc = await GetNavigateResultAsync();
                var urlNextEpisode = await Task.Run<string>(() =>
                {
                    var doc = htmlParser.ParseDocument(strTitleDoc);
                    var elemMain = (IHtmlElement?)doc.QuerySelector("main") ?? throw new KakuYomuNavigater.Error("No Main Element");
                    var ankerFirstPage = QuerySelectorAndTextContent(elemMain, "a", "1話目から読む");
                    var href = ankerFirstPage.GetAttribute("href") ?? throw new KakuYomuNavigater.Error($"No Firs Page");
                    return href;
                });
                while (urlNextEpisode != string.Empty)
                {
                    // デバッグ用にページ遷移まで3秒の猶予を持たせる
                    // await Task.Delay(3000);
                    ct.ThrowIfCancellationRequested();
                    Debug.WriteLine($"NavigateTo: {urlNextEpisode}");
                    var js = $"location.href = '{urlNextEpisode}'";
                    await _webView2.CoreWebView2.ExecuteScriptAsync(js);
                    await sem.WaitAsync(ct);
                    ct.ThrowIfCancellationRequested();
                    var strEpisodeDoc = await GetNavigateResultAsync();
                    urlNextEpisode = await Task.Run<string>(() =>
                    {
                        var doc = htmlParser.ParseDocument(strEpisodeDoc);
                        var urlNext = GetLink(doc, "rel", "next");
                        var episodeTitle = GetEpisodeTitle(doc);
                        var episodeNumber = GetEpisodeNumber(episodeTitle);
                        Debug.WriteLine($"{episodeNumber:d4}: {episodeTitle}");
                        var episodeParagraphs = GetEpisodeParagraphs(doc);
                        zipBook.AddEpisode(episodeNumber, episodeTitle, episodeParagraphs);
                        return urlNext;
                    });
                };
            }
            finally
            {
                _webView2.NavigationCompleted -= eh;
            }
        }

        private static void DebugWriteScriptErrer(CoreWebView2ScriptException ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Name: {ex.Name}");
            sb.AppendLine($"LineNumber: {ex.LineNumber}");
            sb.AppendLine($"ColumnNumber: {ex.ColumnNumber}");
            Debug.Write(sb.ToString());
        }

        private async Task<string> GetNavigateResultAsync()
        {
            var res = await _webView2.CoreWebView2.ExecuteScriptWithResultAsync(JS_GET_HTML);
            if (!res.Succeeded)
            {
                DebugWriteScriptErrer(res.Exception);
                throw new KakuYomuNavigater.Error("Getting HTML Error");
            }
            res.TryGetResultAsString(out string strDoc, out int isValid);
            if (isValid == 0)
            {
                Debug.WriteLine("Scpipt Result Is Not A String.");
                throw new KakuYomuNavigater.Error("Japascript Error");
            }
            return strDoc;
        }
        private static IElement QuerySelectorAndTextContent(IElement elem, string selector, string keyText)
        {
            var res = elem.QuerySelectorAll(selector);
            foreach (var e in res)
            {
                if (e.TextContent == keyText)
                {
                    return e;
                }
            }
            throw new KakuYomuNavigater.Error($"Not Found. Selector: {selector}, KeyText: {keyText}");
        }

        private static string GetLink(IHtmlDocument doc, string attr, string val)
        {
            var links = doc.QuerySelectorAll("link");
            foreach (var link in links)
            {
                var attrVal = link.GetAttribute(attr);
                if (attrVal == val)
                {
                    var href = link.GetAttribute("href") ?? throw new KakuYomuNavigater.Error("No href");
                    return href;
                }
            }
            return string.Empty;
        }

        private static ReadOnlyCollection<string> GetEpisodeParagraphs(IHtmlDocument doc)
        {
            var regexpParagraphId = RegexParagraphId();
            var list = new List<string>();
            var paragraphs = doc.QuerySelectorAll("p");
            foreach (var p in paragraphs)
            {
                var m = regexpParagraphId.Match(p.Id ?? "");
                if (m.Success)
                {
                    list.Add(p.TextContent);
                }
            }
            return list.AsReadOnly();
        }

        private static string GetEpisodeTitle(IHtmlDocument doc)
        {
            var paragraphs = doc.QuerySelectorAll("p");
            foreach (var paragraph in paragraphs)
            {
                if (paragraph.ClassList.Contains("widget-episodeTitle"))
                {
                    return paragraph.TextContent;
                }
            }
            throw new KakuYomuNavigater.Error("Episode Title Not Found");
        }

        private static int GetEpisodeNumber(string title)
        {
            var regexp = RegexEpisodeNumber();
            var match = regexp.Match(title);
            if (!match.Success)
            {
                throw new KakuYomuNavigater.Error("No Eposode Number in Invalid Title");
            }
            var sb = new StringBuilder();
            foreach (var ch in match.Groups[1].Value)
            {
                if (('０' <= ch) && (ch <= '９'))
                {
                    var han = (char)(ch - '０' + '0');
                    sb.Append(han);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            var episodeNumber = int.Parse(sb.ToString());
            return episodeNumber;
        }

        [GeneratedRegex(@"^p\d+$")]
        private static partial Regex RegexParagraphId();

        [GeneratedRegex(@"^第?([0-9０-９]+)")]
        private static partial Regex RegexEpisodeNumber();
    }
}
