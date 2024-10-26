namespace Rise.Client.Utils.Navigation;


public class NavigationLink
{
    public string Url { get; set; }
    public string Name { get; set; }
    public bool Authenticated { get; set; }

    public NavigationLink(string url, string name, bool authenticated)
    {
        Url = url;
        Name = name;
        Authenticated = authenticated;
    }
}

public class NavigationLinks
{
    private const string INFORMATION = "/informatie";
    private const string ACTUA = "/actua";
    private const string GOD_PARENT = "/meter-peter";
    private const string DOCUMENTS = "/documenten";
    private const string USERS = "Users";
    private const string AUTH_USERS = "authusers";
    private const string MAKEBOOKING = "MakeBookingView";


    public static List<NavigationLink> GetNavigationLinks()
    {
        return new List<NavigationLink>
        {
            new (USERS, "Users", true),
            new (AUTH_USERS, "Auth users", true),
            new (INFORMATION, "Information", false),
            new (ACTUA, "Actua", false),
            new (GOD_PARENT, "GodParents", false),
            new (DOCUMENTS, "Documents", false),
            new (MAKEBOOKING, "MakeBooking", true)
        };
    }
}
