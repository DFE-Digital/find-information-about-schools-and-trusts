namespace Dfe.FindInformationAcademiesTrusts.Models;

public static class Links
{
    private static readonly List<LinkItem> _links = [];
    
    public static LinkItem AddLinkItem(string page, string backText = "Back")
    {
        LinkItem item = new() { Page = page, BackText = backText };
        _links.Add(item);
        return item;
    }
    
    public static class Watchlist
    {
        public static readonly LinkItem Index = AddLinkItem(backText: "Back", page: "/watchlist/index");
        public static readonly LinkItem SchoolsWatchlist = AddLinkItem(backText: "Back", page: "/watchlist/schools");
        public static readonly LinkItem TrustsWatchlist = AddLinkItem(backText: "Back", page: "/watchlist/trusts");
        public static readonly LinkItem SelectSchool = AddLinkItem(backText: "Back", page: "/watchlist/selectschool");
        public static readonly LinkItem ConfirmSchool = AddLinkItem(backText: "Back", page: "/watchlist/confirmschool");
        public static readonly LinkItem ConfirmTrust = AddLinkItem(backText: "Back", page: "/watchlist/confirmtrust");
        public static readonly LinkItem RemoveSchool = AddLinkItem(backText: "Back", page: "/watchlist/removeschool");
        public static readonly LinkItem RemoveTrust = AddLinkItem(backText: "Back", page: "/watchlist/removetrust");
        public static readonly LinkItem SearchForASchool = AddLinkItem(backText: "Back", page: "/watchlist/searchforaschool");
        public static readonly LinkItem SearchForATrust = AddLinkItem(backText: "Back", page: "/watchlist/searchforatrust");

    }
    
    public static class Schools
    {
        public static readonly LinkItem SchoolDetails = AddLinkItem(backText: "Back", page: "/Schools/Overview/Details");
    }
    
    public static class Trusts
    {
        public static readonly LinkItem TrustDetails = AddLinkItem(backText: "Back", page: "/Trusts/Overview/TrustDetails");
    }
    
}


public class LinkItem
{
    public string Page { get; set; } = null!;
    public string BackText { get; set; } = "Back";
}