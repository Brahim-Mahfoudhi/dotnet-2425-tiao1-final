using Rise.Shared.Enums;

namespace Rise.Client.Utils.Navigation;

public class NavigationLink
{
    public string Url { get; }
    public string Name { get; }
    public bool Authenticated { get; }
    public RolesEnum? Role { get; }

    public NavigationLink(string url, string name, bool authenticated, RolesEnum? role = null)
    {
        Url = url;
        Name = name;
        Authenticated = authenticated;
        Role = role;
    }
}

public class PageInfo
{
    public string Url { get; }
    public string BackgroundImage { get; }
    public string PageClass { get; }
    public bool RenderHeader { get; }

    public PageInfo(string url, string backgroundImage = "img/buut_bg.png", string pageClass = "", bool renderHeader = true)
    {
        Url = url;
        BackgroundImage = backgroundImage;
        PageClass = pageClass;
        RenderHeader = renderHeader;
    }
}