using System.Reflection;
using Dfe.FindInformationAcademiesTrusts.Models;
using static Dfe.FindInformationAcademiesTrusts.Models.Links;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Models
{
    public class LinksTests
    {
        private static List<LinkItem> RegisteredLinks()
        {
            var fieldInfo = typeof(Links).GetField("_links", BindingFlags.NonPublic | BindingFlags.Static);
            return (List<LinkItem>)fieldInfo?.GetValue(null)!;
        }

        private static List<LinkItem> LinkItemsIn(Type type) =>
            type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetValue(null))
                .OfType<LinkItem>()
                .ToList();

        [Fact]
        public void AddLinkItem_ShouldAddNewLinkItem()
        {
            // Act
            var link = AddLinkItem("/newpage", "Go back");

            // Assert
            Assert.IsType<LinkItem>(link);
            Assert.Equal("/newpage", link.Page);
            Assert.Equal("Go back", link.BackText);
        }

        [Fact]
        public void AddLinkItem_WithoutBackText_ShouldDefaultBackTextToBack()
        {
            // Act
            var link = AddLinkItem("/newpage");

            // Assert
            Assert.IsType<LinkItem>(link);
            Assert.Equal("/newpage", link.Page);
            Assert.Equal("Back", link.BackText);
        }

        [Fact]
        public void AddLinkItem_ShouldRegisterTheLinkItem()
        {
            // Act
            var link = AddLinkItem("/registeredpage");

            // Assert
            Assert.Contains(link, RegisteredLinks());
        }

        [Fact]
        public void AddLinkItem_CalledTwiceForTheSamePage_ShouldReturnDifferentInstances()
        {
            // Act
            var first = AddLinkItem("/samepage");
            var second = AddLinkItem("/samepage");

            // Assert
            Assert.NotSame(first, second);
        }

        [Fact]
        public void LinkItem_ShouldHaveDefaultBackText()
        {
            // Arrange & Act
            var link = new LinkItem();

            // Assert
            Assert.Equal("Back", link.BackText);
            Assert.Null(link.Page);
        }

        [Fact]
        public void LinkItem_ShouldSetAndGetValues()
        {
            // Arrange & Act
            var link = new LinkItem { Page = "/somepage", BackText = "Return" };

            // Assert
            Assert.Equal("/somepage", link.Page);
            Assert.Equal("Return", link.BackText);
        }

        [Theory]
        [InlineData("/watchlist/index")]
        [InlineData("/watchlist/schools")]
        [InlineData("/watchlist/trusts")]
        [InlineData("/watchlist/selectschool")]
        [InlineData("/watchlist/confirmschool")]
        [InlineData("/watchlist/confirmtrust")]
        [InlineData("/watchlist/removeschool")]
        [InlineData("/watchlist/removetrust")]
        [InlineData("/watchlist/searchforaschool")]
        [InlineData("/watchlist/searchforatrust")]
        public void Watchlist_Items_ShouldHaveCorrectValues(string expectedPage)
        {
            // Arrange & Act
            var expectedBackText = "Back";

            var foundItem = LinkItemsIn(typeof(Watchlist)).FirstOrDefault(item => item.Page == expectedPage);

            // Assert
            Assert.NotNull(foundItem);
            Assert.Equal(expectedPage, foundItem.Page);
            Assert.Equal(expectedBackText, foundItem.BackText);
        }

        [Fact]
        public void Watchlist_ShouldOnlyContainTheExpectedLinkItems()
        {
            // Arrange & Act
            var items = LinkItemsIn(typeof(Watchlist));

            // Assert
            Assert.Equal(10, items.Count);
        }

        [Fact]
        public void Watchlist_Index_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var linkItem = Watchlist.Index;

            // Assert
            Assert.NotNull(linkItem);
            Assert.Equal("/watchlist/index", linkItem.Page);
            Assert.Equal("Back", linkItem.BackText);
        }

        [Fact]
        public void Watchlist_SchoolsWatchlist_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var linkItem = Watchlist.SchoolsWatchlist;

            // Assert
            Assert.NotNull(linkItem);
            Assert.Equal("/watchlist/schools", linkItem.Page);
            Assert.Equal("Back", linkItem.BackText);
        }

        [Fact]
        public void Watchlist_TrustsWatchlist_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var linkItem = Watchlist.TrustsWatchlist;

            // Assert
            Assert.NotNull(linkItem);
            Assert.Equal("/watchlist/trusts", linkItem.Page);
            Assert.Equal("Back", linkItem.BackText);
        }

        [Fact]
        public void Schools_SchoolDetails_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var linkItem = Schools.SchoolDetails;

            // Assert
            Assert.NotNull(linkItem);
            Assert.Equal("/Schools/Overview/Details", linkItem.Page);
            Assert.Equal("Back", linkItem.BackText);
        }

        [Fact]
        public void Trusts_TrustDetails_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var linkItem = Trusts.TrustDetails;

            // Assert
            Assert.NotNull(linkItem);
            Assert.Equal("/Trusts/Overview/TrustDetails", linkItem.Page);
            Assert.Equal("Back", linkItem.BackText);
        }

        [Fact]
        public void NestedLinkItems_ShouldBeRegistered()
        {
            // Arrange & Act
            var nestedItems = LinkItemsIn(typeof(Watchlist))
                .Concat(LinkItemsIn(typeof(Schools)))
                .Concat(LinkItemsIn(typeof(Trusts)))
                .ToList();

            var registered = RegisteredLinks();

            // Assert
            Assert.All(nestedItems, item => Assert.Contains(item, registered));
        }

        [Fact]
        public void NestedLinkItems_ShouldAllHaveUniquePages()
        {
            // Arrange & Act
            var pages = LinkItemsIn(typeof(Watchlist))
                .Concat(LinkItemsIn(typeof(Schools)))
                .Concat(LinkItemsIn(typeof(Trusts)))
                .Select(item => item.Page)
                .ToList();

            // Assert
            Assert.Equal(pages.Count, pages.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        }
    }
}
