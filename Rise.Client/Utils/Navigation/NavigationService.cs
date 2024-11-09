using Rise.Client.Utils.Navigation;
using Rise.Shared.Enums;

public static class NavigationService
{
    public static List<NavigationLink> GetNavigationLinks() => new()
    {
        new NavigationLink("Users", "Users", authenticated: true, role: RolesEnum.Admin),
        new NavigationLink("authusers", "Auth Users", authenticated: true, role: RolesEnum.Admin),
        new NavigationLink("informatie", "Information", authenticated: false),
        new NavigationLink("actua", "Actua", authenticated: false),
        new NavigationLink("meter-peter", "Sponsors", authenticated: false),
        new NavigationLink("documenten", "Documents", authenticated: false),
        new NavigationLink("MakeBookingView", "MakeBooking", authenticated: true, role: RolesEnum.User),
        new NavigationLink("mybookings", "My Bookings", authenticated: true, role: RolesEnum.User)
    };

    public static List<PageInfo> GetPageInfos() => new()
    {
        new PageInfo("", backgroundImage: "img/buut_bg.png", renderHeader: false),
        new PageInfo("Users"),
        new PageInfo("authusers"),
        new PageInfo("informatie"),
        new PageInfo("actua"),
        new PageInfo("meter-peter"),
        new PageInfo("documenten"),
        new PageInfo("MakeBookingView"),
        new PageInfo("mybookings"),
        new PageInfo("embedded-login", backgroundImage: "img/buut_BG3.png"),
        new PageInfo("register", backgroundImage: "img/buut_BG4.png", pageClass: "signup-page")
    };
}