namespace wikrypted.Models;

public class WikipediaArticle
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        string titleOutline = new string('=', Title.Length);
        return $"{Title}\n{titleOutline}\n{Description}";
    }
}