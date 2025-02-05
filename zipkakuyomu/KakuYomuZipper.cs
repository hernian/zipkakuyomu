using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Diagnostics;

namespace zipkakuyomu
{
    internal class KakuYomuZipper(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;
        public async Task ZipKakuYomuAsync()
        {
            var parser = new HtmlParser();
            var resp = await _httpClient.GetAsync("https://kakuyomu.jp/works/16817330648636109045");
            resp.EnsureSuccessStatusCode();
            var strHtml = await resp.Content.ReadAsStringAsync();
            var doc = parser.ParseDocument(strHtml);
            var elemMain = doc.QuerySelector("main");
            Debug.WriteLine(elemMain);
        }
    }
}
