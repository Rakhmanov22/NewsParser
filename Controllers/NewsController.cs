using Microsoft.AspNetCore.Mvc;
using NewsParser.Interfaces;

namespace NewsParser.Controllers
{
    [Route("api")]
    public class NewsController : ControllerBase
    {
        private readonly INewsRepository _newsRepository;

        public NewsController(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        [HttpGet("posts")]
        public IActionResult GetNewsByDateRange()
        {
            var news = _newsRepository.GetNewsByDateRange();
            return Ok(news);
        }

        [HttpGet("topten")]
        public IActionResult GetTopTenWords()
        {
            var topTenWords = _newsRepository.GetTopTenWords();
            return Ok(topTenWords);
        }

        [HttpGet("search")]
        public IActionResult SearchNewsByText(string text)
        {
            var matchingNews = _newsRepository.SearchNewsByText(text);
            return Ok(matchingNews);
        }
    }
}