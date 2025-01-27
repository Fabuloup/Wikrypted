// See https://aka.ms/new-console-template for more information

using wikrypted;
using wikrypted.Models;

Console.OutputEncoding = System.Text.Encoding.Unicode;

bool exit = false;

while (!exit)
{
    Console.Clear();
    Console.WriteLine("Starting a new game...");
    Console.WriteLine("\n\n");
    WikipediaArticle article = await WikipediaArticleFetcher.GetRandomArticleAsync();
    Gameplay.DecodeGameplay(article);
    
    Console.ReadLine();
}
