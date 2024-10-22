namespace Rise.Client.Utils.Navigation;


public class NavigationLink
{
    public string Url { get; set; }
    public string Name { get; set; }

    public NavigationLink(string url, string name)
    {
        Url = url;
        Name = name;
    }
}

public class NavigationLinks
{
    private const string INFORMATION = "/informatie";
    private const string ACTUA = "/actua";
    private const string GOD_PARENT = "/meter-peter";
    private const string DOCUMENTS = "/documenten";

    public static List<NavigationLink> GetNavigationLinks()
    {
        return new List<NavigationLink>
        {
            new (INFORMATION, "Informatie"),
            new (ACTUA, "Actua"),
            new (GOD_PARENT, "Meter en Peter"),
            new (DOCUMENTS, "Documenten")
        };
    }
}
