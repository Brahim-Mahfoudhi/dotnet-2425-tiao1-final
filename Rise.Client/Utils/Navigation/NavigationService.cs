using Auth0.ManagementApi.Models;
using Rise.Client.Utils.Navigation;
using Rise.Shared.Enums;

public static class NavigationService
{
    public static List<NavigationLink> GetNavigationLinks() => new()
    {
        new NavigationLink("userspage", "ManageUsers", authenticated: true, role: RolesEnum.Admin),
        new NavigationLink("boats", "Boats", authenticated: true, role: RolesEnum.Admin),
        new NavigationLink("batteries", "Batteries", authenticated: true, role: RolesEnum.Admin),
        new NavigationLink("informatie", "Information", authenticated: false),
        new NavigationLink("actua", "Actua", authenticated: false),
        new NavigationLink("meter-peter", "Sponsors", authenticated: false),
        new NavigationLink("documenten", "Documents", authenticated: false),
        new NavigationLink("MakeBookingView", "MakeBooking", authenticated: true, role: RolesEnum.User),
        new NavigationLink("mybookings", "MyBookings", authenticated: true, role: RolesEnum.User),
    };

    public static List<PageInfo> GetPageInfos() => new()
    {
        new PageInfo("", backgroundImage: "img/Buut_BG.png", renderHeader: false),
        new PageInfo("userspage"),
        new PageInfo("boats"),
        new PageInfo("batteries"),
        new PageInfo("userdetails/{userId}"),
        new PageInfo("informatie"),
        new PageInfo("actua"),
        new PageInfo("meter-peter"),
        new PageInfo("documenten"),
        new PageInfo("MakeBookingView"),
        new PageInfo("mybookings"),
        new PageInfo("notifications"),
        new PageInfo("embedded-login", backgroundImage: "img/Buut_BG3.png"),
        new PageInfo("register", backgroundImage: "img/Buut_BG4.png", pageClass: "signup-page sidebar-collapse")
    };
}