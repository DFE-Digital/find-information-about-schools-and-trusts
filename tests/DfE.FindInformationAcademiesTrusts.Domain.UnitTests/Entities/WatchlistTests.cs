using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;

namespace DfE.FindInformationAcademiesTrusts.Domain.UnitTests.Entities
{
    public class WatchlistTests
    {
        [Fact]
        public void Constructor_WithEstablishmentParameters_SetsAllConstructorFields()
        {
            var id = new WatchlistId(Guid.NewGuid());
            const string establishmentId = "100001";
            const string user = "user@example.com";

            var watchlist = new Watchlist(id, establishmentId, null, false, user);

            Assert.Equal(id, watchlist.Id);
            Assert.Equal(establishmentId, watchlist.EstablishmentId);
            Assert.Null(watchlist.TrustId);
            Assert.False(watchlist.IsTrust);
            Assert.Equal(user, watchlist.User);
        }

        [Fact]
        public void Constructor_WithTrustParameters_SetsAllConstructorFields()
        {
            var id = new WatchlistId(Guid.NewGuid());
            const string trustId = "TR00001";
            const string user = "user@example.com";

            var watchlist = new Watchlist(id, null, trustId, true, user);

            Assert.Equal(id, watchlist.Id);
            Assert.Null(watchlist.EstablishmentId);
            Assert.Equal(trustId, watchlist.TrustId);
            Assert.True(watchlist.IsTrust);
            Assert.Equal(user, watchlist.User);
        }

        [Fact]
        public void Constructor_DoesNotSetReadableId_ToSupportDatabaseGeneratedValue()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "user@test.gov.uk");

            Assert.Equal(0, watchlist.ReadableId);
        }

        [Fact]
        public void Constructor_CreatedByDefaultsToEmptyString()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "user@test.gov.uk");

            Assert.Equal(string.Empty, watchlist.CreatedBy);
        }

        [Fact]
        public void Constructor_AuditDatesAndLastModifiedFieldsDefaultToUnset()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "user@test.gov.uk");

            Assert.Equal(default, watchlist.CreatedOn);
            Assert.Null(watchlist.LastModifiedOn);
            Assert.Null(watchlist.LastModifiedBy);
        }

        [Fact]
        public void Constructor_HasNoDomainEvents()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "user@test.gov.uk");

            Assert.Empty(watchlist.DomainEvents);
        }

        [Fact]
        public void Properties_WhenAssigned_PersistAuditAndIdentityFields()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "original@test.gov.uk");
            var newId = new WatchlistId(Guid.NewGuid());
            var createdOn = DateTime.UtcNow;
            var lastModifiedOn = createdOn.AddHours(2);

            watchlist.Id = newId;
            watchlist.EstablishmentId = "100002";
            watchlist.TrustId = "TR00002";
            watchlist.IsTrust = true;
            watchlist.User = "updated@test.gov.uk";
            watchlist.CreatedOn = createdOn;
            watchlist.CreatedBy = "creator@test.gov.uk";
            watchlist.LastModifiedOn = lastModifiedOn;
            watchlist.LastModifiedBy = "modifier@test.gov.uk";

            Assert.Equal(newId, watchlist.Id);
            Assert.Equal("100002", watchlist.EstablishmentId);
            Assert.Equal("TR00002", watchlist.TrustId);
            Assert.True(watchlist.IsTrust);
            Assert.Equal("updated@test.gov.uk", watchlist.User);
            Assert.Equal(createdOn, watchlist.CreatedOn);
            Assert.Equal("creator@test.gov.uk", watchlist.CreatedBy);
            Assert.Equal(lastModifiedOn, watchlist.LastModifiedOn);
            Assert.Equal("modifier@test.gov.uk", watchlist.LastModifiedBy);
        }

        [Fact]
        public void User_CanBeSetToNull()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "user@test.gov.uk");

            watchlist.User = null;

            Assert.Null(watchlist.User);
        }

        [Fact]
        public void EstablishmentIdAndTrustId_CanBeSetToNull()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", "TR00001", false, "user@test.gov.uk");

            watchlist.EstablishmentId = null;
            watchlist.TrustId = null;

            Assert.Null(watchlist.EstablishmentId);
            Assert.Null(watchlist.TrustId);
        }

        [Fact]
        public void LastModifiedOnAndLastModifiedBy_CanBeSetToNull()
        {
            var watchlist = new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, "user@test.gov.uk")
            {
                LastModifiedOn = DateTime.UtcNow,
                LastModifiedBy = "someone"
            };

            watchlist.LastModifiedOn = null;
            watchlist.LastModifiedBy = null;

            Assert.Null(watchlist.LastModifiedOn);
            Assert.Null(watchlist.LastModifiedBy);
        }
    }
}
