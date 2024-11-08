using Rise.Shared.Enums;
namespace Rise.Client.Utils.Navigation;


public class NavigationLink
{
    public string Url { get; set; }
    public string Name { get; set; }
    public bool Authenticated { get; set; }
    public RolesEnum? Role { get; set; }

    public NavigationLink(string url, string name, bool authenticated, RolesEnum? role = null)
    {
        Url = url;
        Name = name;
        Authenticated = authenticated;
        Role = role;
    }
}

public class NavigationLinks
{
    private const string INFORMATION = "/informatie";
    private const string ACTUA = "/actua";
    private const string SPONSORS = "/meter-peter";
    private const string DOCUMENTS = "/documenten";
    private const string USERS = "Users";
    private const string AUTH_USERS = "authusers";
    private const string MAKEBOOKING = "MakeBookingView";
    private const string MYBOOKINGS = "mybookings";

    private const RolesEnum USER = RolesEnum.User;
    private const RolesEnum ADMIN = RolesEnum.Admin;
    private const RolesEnum GODPARENT = RolesEnum.Godparent;

    public static List<NavigationLink> GetNavigationLinks()
    {
        return new List<NavigationLink>
        {
            new (USERS, "Users", true, ADMIN),
            new (AUTH_USERS, "Auth users", true, ADMIN),
            new (INFORMATION, "Information", false),
            new (ACTUA, "Actua", false),
            new (SPONSORS, "Sponsors", false),
            new (DOCUMENTS, "Documents", false),
            new (MAKEBOOKING, "MakeBooking", true, USER),
            new (MYBOOKINGS, "MyBookings", true, USER)
        };
    }
}
