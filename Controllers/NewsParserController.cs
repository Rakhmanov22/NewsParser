using NewsParser.Interfaces;
using NewsParser.Models;
using System;
using System.Text.RegularExpressions;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace NewsParser.Controllers
{
    [Authorize]
    public class NewsParserController : ControllerBase
    {
        private readonly INewsParseRepository _newsParseRepository;
        private readonly ILogger<NewsParserController> _logger;

        public NewsParserController(INewsParseRepository newsParseRepository, ILogger<NewsParserController> logger)
        {
            _newsParseRepository = newsParseRepository;
            _logger = logger;   
        }

        [HttpPost("parse")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ParseNewsAsync()
        {
            try
            {
                var username = User.Identity.Name;
                var url = "https://zk.kz/";
                string htmlContent = await GetHtmlContentAsync(url);
                if (!string.IsNullOrEmpty(htmlContent))
                {
                    // Здесь вы можете использовать htmlContent для дальнейшего парсинга
                    ParseAndSaveNews(htmlContent);
                }
                _logger.LogInformation("Add parse news in the database by user {Username} at {Timestamp}", username, DateTime.UtcNow);
                return Ok("News parsed and saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parse news in the database by user {Username} at {Timestamp}", ex.Message, DateTime.UtcNow);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<string> GetHtmlContentAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Бросит исключение, если запрос завершится неудачно

                return await response.Content.ReadAsStringAsync();
            }
        }

        public void ParseAndSaveNews(string htmlContent)
        {
            // В этом проекте используем простой код для демонстрации
            var posts = new List<Post>();
            var matches = Regex.Matches(htmlContent, @"<div class=""project-list_inner"">(?<content>.*?)<div class=""project-list_txt"">\s*<p>(?<text>.*?)</p>\s*</div>", RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                var dateMatch = ParseDateContent(match.ToString());
                var nameMatch = ParseTitle(match.ToString());
                var txtMatch = ParseContent(match.ToString());
                posts.Add(new Post { Date = DateTime.ParseExact(dateMatch, "d MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU")), Content = txtMatch, Title = nameMatch });
            }
            _newsParseRepository.AddNews(posts);
        }

        private string ParseTitle(string htmlContent)
        {
            // извлечения заголовка с использованием регулярного выражения
            var match = Regex.Match(htmlContent, @"<div class=""project-list_name"">\s*<a href=""(?<url>.*?)"">(?<name>.*?)</a>\s*</div>");
            return match.Success ? match.Groups[2].Value : "No Title";
        }

        private string ParseContent(string htmlContent)
        {
            // извлечения содержимого с использованием регулярного выражения
            var match = Regex.Match(htmlContent, @"<div class=""project-list_txt"">\s*<p>(?<text>.*?)</p>\s*</div>");
            return match.Success ? match.Groups[1].Value : "No Content";
        }

        private string ParseDateContent(string htmlContent)
        {
            // извлечения даты с использованием регулярного выражения
            var match = Regex.Match(htmlContent, @"<div class=""project-list_date"">(?<date>.*?)</div>");
            return match.Success ? match.Groups[1].Value : "No Content";
        }
    }
}
