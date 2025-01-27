using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wikrypted.Models;

namespace wikrypted;

public static class WikipediaArticleFetcher
{
    private const string RandomArticleUrl = "https://en.wikipedia.org/wiki/Special:Random";
    private static readonly HttpClient client = new HttpClient();
    private const string localJsonFilePath = "articles.json";

    public static async Task<string> GetRandomArticleUrl()
    {
        HttpResponseMessage response = await client.GetAsync(RandomArticleUrl);
        response.EnsureSuccessStatusCode();
        
        return response.RequestMessage.RequestUri.ToString();
    }

    public static async Task<WikipediaArticle> GetRandomArticleAsync()
    {
        string articleUrl = await GetRandomArticleUrl();

        // Check if the article URL already exists in the local JSON file
        var articles = LoadArticlesFromFile();
        var existingArticle = articles.FirstOrDefault(a => a.Url == articleUrl);

        if (existingArticle != null)
        {
            return existingArticle;
        }

        // Download the article if it does not exist locally
        string apiUrl = $"https://en.wikipedia.org/api/rest_v1/page/summary/{articleUrl.Split('/').Last()}";
        string jsonResponse = await client.GetStringAsync(apiUrl);
        JObject articleData = JObject.Parse(jsonResponse);

        string title = articleData["title"].ToString();
        string description = articleData["extract"].ToString();

        var newArticle = new WikipediaArticle { Url = articleUrl, Title = title, Description = description };

        // Save the new article to the local JSON file
        articles.Add(newArticle);
        SaveArticlesToFile(articles);

        return newArticle;
    }
    
    private static List<WikipediaArticle> LoadArticlesFromFile()
    {
        if (!File.Exists(localJsonFilePath))
        {
            return new List<WikipediaArticle>();
        }

        string json = File.ReadAllText(localJsonFilePath);
        return JsonConvert.DeserializeObject<List<WikipediaArticle>>(json);
    }

    private  static void SaveArticlesToFile(List<WikipediaArticle> articles)
    {
        string json = JsonConvert.SerializeObject(articles, Formatting.Indented);
        File.WriteAllText(localJsonFilePath, json);
    }
}