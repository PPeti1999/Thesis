using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthyAPI.Data;
using HealthyAPI.DTOs.Profile;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;

namespace HealthyAPI.Tests
{
  public class TestFixture : IDisposable
  {
    public Context Context { get; private set; }
    public static string TestUserId = "27aca460-6814-41e1-8544-3dfa81a086c3";
    public static string OtherUserId = "other-user-id-12345";
    public static string YesterdayNoteId = "yesterday-note-id";
    public static string TodayNoteId = "today-note-id";
    public static string Food1Id = "food1";
    public static string Food2Id = "food2";
    public static string Recipe1Id = "rec1";
    public static string MealType1Id = "1";
    public static string MealType2Id = "2";

    public TestFixture()
    {
      var options = new DbContextOptionsBuilder<Context>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      Context = new Context(options);
      SeedDatabase();
    }

    private void SeedDatabase()
    {
      var testUser = new User
      {
        Id = TestUserId,
        FirstName = "pásztori",
        LastName = "péter",
        Age = 25,
        Height = 180,
        Weight = 92,
        GoalWeight = 90,
        TargetCalorie = 2300,
        TargeProtein = 180,
        TargetCarb = 300,
        TargetFat = 70,
        ActivityMultiplier = 1.55f,
        IsFemale = false,
        GoalType = 2,
        UserName = "pasztoripeti@gmail.com",
        Email = "pasztoripeti@gmail.com",
        EmailConfirmed = true
      };
      Context.Users.Add(testUser);

      Context.Food.AddRange(
          new Food { FoodID = Food1Id, Title = "Csirkemell", Protein = 31, Fat = 3, Carb = 0, Calorie = 165, Gram = 100 },
          new Food { FoodID = Food2Id, Title = "Rizs", Protein = 2, Fat = 0, Carb = 28, Calorie = 130, Gram = 100 }
      );

      Context.Recipe.Add(new Recipe { RecipeID = Recipe1Id, Title = "Csirkerizs", SumProtein = 33, SumCarb = 28, SumFat = 3, SumCalorie = 295 });
      Context.RecipeFoods.AddRange(
          new RecipeFoods { RecipeFoodID = "rf1", RecipeID = Recipe1Id, FoodID = Food1Id, Quantity = 100 },
          new RecipeFoods { RecipeFoodID = "rf2", RecipeID = Recipe1Id, FoodID = Food2Id, Quantity = 100 }
      );

      Context.MealTypes.AddRange(
          new MealTypes { MealTypeID = MealType1Id, Name = "Reggeli" },
          new MealTypes { MealTypeID = MealType2Id, Name = "Ebéd" }
      );

      var yesterday = DateTime.Today.AddDays(-1);
      Context.DailyNote.Add(new DailyNote
      {
        DailyNoteID = YesterdayNoteId,
        UserID = TestUserId,
        DailyWeight = 93,
        CreatedAt = yesterday,
        DailyTargetCalorie = 2300,
        DailyTargetProtein = 180,
        DailyTargetCarb = 300,
        DailyTargetFat = 70
      });

      Context.DailyNote.Add(new DailyNote
      {
        DailyNoteID = TodayNoteId,
        UserID = TestUserId,
        DailyWeight = 92,
        CreatedAt = DateTime.Today,
        DailyTargetCalorie = 2300,
        DailyTargetProtein = 180,
        DailyTargetCarb = 300,
        DailyTargetFat = 70
      });

      Context.SaveChanges();
    }

    public Context CreateContext()
    {
      var options = new DbContextOptionsBuilder<Context>()
         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;

      var context = new Context(options);
      SeedDatabaseForContext(context);
      return context;
    }

    private void SeedDatabaseForContext(Context context)
    {
      var testUser = new User
      {
        Id = TestUserId,
        FirstName = "pásztori",
        LastName = "péter",
        Age = 25,
        Height = 180,
        Weight = 92,
        GoalWeight = 90,
        TargetCalorie = 2300,
        TargeProtein = 180,
        TargetCarb = 300,
        TargetFat = 70,
        ActivityMultiplier = 1.55f,
        IsFemale = false,
        GoalType = 2,
        UserName = "pasztoripeti@gmail.com",
        Email = "pasztoripeti@gmail.com",
        EmailConfirmed = true
      };
      context.Users.Add(testUser);
      context.Users.Add(new User { Id = OtherUserId, FirstName = "Másik", LastName = "Felhasználó", Weight = 70, Height = 170, Age = 30, IsFemale = true, ActivityMultiplier = 1.2f, GoalType = 0, TargeProtein = 140, TargetCarb = 200, TargetFat = 60, TargetCalorie = 2000 });

      context.Food.AddRange(
          new Food { FoodID = Food1Id, Title = "Csirkemell", Protein = 31, Fat = 3, Carb = 0, Calorie = 165, Gram = 100 },
          new Food { FoodID = Food2Id, Title = "Rizs", Protein = 2, Fat = 0, Carb = 28, Calorie = 130, Gram = 100 }
      );
      context.Recipe.Add(new Recipe { RecipeID = Recipe1Id, Title = "Csirkerizs", SumProtein = 33, SumCarb = 28, SumFat = 3, SumCalorie = 295 });
      context.RecipeFoods.AddRange(
          new RecipeFoods { RecipeFoodID = "rf1", RecipeID = Recipe1Id, FoodID = Food1Id, Quantity = 100 },
          new RecipeFoods { RecipeFoodID = "rf2", RecipeID = Recipe1Id, FoodID = Food2Id, Quantity = 100 }
      );
      context.MealTypes.AddRange(
          new MealTypes { MealTypeID = MealType1Id, Name = "Reggeli" },
          new MealTypes { MealTypeID = MealType2Id, Name = "Ebéd" }
      );
      var yesterday = DateTime.Today.AddDays(-1);
      context.DailyNote.Add(new DailyNote
      {
        DailyNoteID = YesterdayNoteId,
        UserID = TestUserId,
        DailyWeight = 93,
        CreatedAt = yesterday,
        DailyTargetCalorie = 2300,
        DailyTargetProtein = 180,
        DailyTargetCarb = 300,
        DailyTargetFat = 70
      });
      context.DailyNote.Add(new DailyNote
      {
        DailyNoteID = TodayNoteId,
        UserID = TestUserId,
        DailyWeight = 92,
        CreatedAt = DateTime.Today,
        DailyTargetCalorie = 2300,
        DailyTargetProtein = 180,
        DailyTargetCarb = 300,
        DailyTargetFat = 70
      });
      context.SaveChanges();
    }

    public void Dispose()
    {
      Context.Database.EnsureDeleted();
      Context.Dispose();
    }
  }

  public class UserProfileServiceTests : IClassFixture<TestFixture>
  {
    private readonly TestFixture _fixture;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public UserProfileServiceTests(TestFixture fixture)
    {
      _fixture = fixture;

      var userStoreMock = new Mock<IUserStore<User>>();
      _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

      var contextAccessorMock = new Mock<IHttpContextAccessor>();
      var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
      _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

      _configurationMock = new Mock<IConfiguration>();
    }

    private UserProfileService CreateService(Context context)
    {
      return new UserProfileService(
          context,
          null,
          _signInManagerMock.Object,
          _userManagerMock.Object,
          null,
          _configurationMock.Object
      );
    }

    [Fact]
    public async Task GetCurrentUserProfile_ShouldReturnProfile_WhenUserExists()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);

      var profile = await service.GetCurrentUserProfile(TestFixture.TestUserId);

      profile.Should().NotBeNull();
      profile.Id.Should().Be(TestFixture.TestUserId);
      profile.FirstName.Should().Be("pásztori");
    }

    [Fact]
    public async Task GetCurrentUserProfile_ShouldReturnNull_WhenUserDoesNotExist()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);

      var profile = await service.GetCurrentUserProfile("nem-letezo-id");

      profile.Should().BeNull();
    }

    [Fact]
    public async Task UpdateProfile_ShouldReturnNull_WhenUserDoesNotExist()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto();

      var result = await service.UpdateProfile("nem-letezo-id", dto);

      result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateProfile_ShouldUpdateBasicInfo()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto
      {
        FirstName = "Frissített",
        LastName = "Név",
        Age = 30,
        Height = 181,
        Weight = 90,
        GoalWeight = 85,
        BodyFat = 18,
        IsFemale = false,
        GoalType = 2,
        ActivityMultiplier = 1.725f
      };

      var result = await service.UpdateProfile(TestFixture.TestUserId, dto);
      var userInDb = await context.Users.FindAsync(TestFixture.TestUserId);

      result.Should().NotBeNull();
      result.FirstName.Should().Be("Frissített");

      userInDb.LastName.Should().Be("Név");
      userInDb.Age.Should().Be(30);
      userInDb.Height.Should().Be(181);
      userInDb.Weight.Should().Be(90);
      userInDb.GoalWeight.Should().Be(85);
      userInDb.BodyFat.Should().Be(18);
      userInDb.ActivityMultiplier.Should().Be(1.725f);
    }

    [Fact]
    public async Task UpdateProfile_ShouldCalculateTDEE_Male_MaintainWeight()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto
      {
        Weight = 90,
        Height = 180,
        Age = 30,
        IsFemale = false,
        GoalType = 0,
        ActivityMultiplier = 1.55f
      };

      var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

      result.Should().NotBeNull();
      result.TargetCalorie.Should().Be(2913);
    }

    [Fact]
    public async Task UpdateProfile_ShouldCalculateTDEE_Female_MaintainWeight()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto
      {
        Weight = 70,
        Height = 170,
        Age = 30,
        IsFemale = true,
        GoalType = 0,
        ActivityMultiplier = 1.2f
      };

      var result = await service.UpdateProfile(TestFixture.OtherUserId, dto);

      result.Should().NotBeNull();
      result.TargetCalorie.Should().Be(1741);
    }

    [Fact]
    public async Task UpdateProfile_ShouldCalculateTDEE_Male_GainWeight()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto
      {
        Weight = 90,
        Height = 180,
        Age = 30,
        IsFemale = false,
        GoalType = 1,
        ActivityMultiplier = 1.55f
      };

      var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

      result.Should().NotBeNull();
      result.TargetCalorie.Should().Be(3413);
    }

    [Fact]
    public async Task UpdateProfile_ShouldCalculateTDEE_Male_LoseWeight()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto
      {
        Weight = 90,
        Height = 180,
        Age = 30,
        IsFemale = false,
        GoalType = 2,
        ActivityMultiplier = 1.55f
      };

      var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

      result.Should().NotBeNull();
      result.TargetCalorie.Should().Be(2413);
    }

    [Fact]
    public async Task UpdateProfile_ShouldUpdateMacrosCorrectly()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);
      var dto = new UpdateUserProfileDto
      {
        Weight = 90,
        Height = 180,
        Age = 30,
        IsFemale = false,
        GoalType = 2,
        ActivityMultiplier = 1.55f
      };

      var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

      result.Should().NotBeNull();
      result.TargetProtein.Should().Be(180);
      result.TargetFat.Should().Be(90);
      result.TargetCarb.Should().Be(221f);
    }

    [Fact]
    public async Task UpdateProfile_ShouldUpdateTodayDailyNote_IfExists()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);

      var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
      todayNote.DailyTargetCalorie.Should().Be(2300);

      var dto = new UpdateUserProfileDto
      {
        Weight = 90,
        Height = 180,
        Age = 30,
        IsFemale = false,
        GoalType = 2,
        ActivityMultiplier = 1.55f
      };

      await service.UpdateProfile(TestFixture.TestUserId, dto);

      var updatedNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
      updatedNote.Should().NotBeNull();
      updatedNote.DailyTargetCalorie.Should().Be(2413);
      updatedNote.DailyTargetProtein.Should().Be(180);
      updatedNote.DailyTargetCarb.Should().Be(221f);
      updatedNote.DailyTargetFat.Should().Be(90);
    }

    [Fact]
    public async Task UpdateProfile_ShouldNotFail_WhenTodayDailyNoteDoesNotExist()
    {
      using var context = _fixture.CreateContext();
      var service = CreateService(context);

      var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
      context.DailyNote.Remove(todayNote);
      await context.SaveChangesAsync();

      var dto = new UpdateUserProfileDto
      {
        Weight = 90,
        Height = 180,
        Age = 30,
        IsFemale = false,
        GoalType = 2,
        ActivityMultiplier = 1.55f
      };

      Func<Task> action = async () => await service.UpdateProfile(TestFixture.TestUserId, dto);

      await action.Should().NotThrowAsync();
      var user = await context.Users.FindAsync(TestFixture.TestUserId);
      user.TargetCalorie.Should().Be(2413);
    }
  }

  public class DailyNoteServiceTests : IClassFixture<TestFixture>
  {
    private readonly TestFixture _fixture;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    public DailyNoteServiceTests(TestFixture fixture)
    {
      _fixture = fixture;

      _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new[] { new Claim(ClaimTypes.NameIdentifier, TestFixture.TestUserId) };
      var identity = new ClaimsIdentity(claims);
      var principal = new ClaimsPrincipal(identity);
      _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });
    }

    [Fact]
    public async Task GetTodayNote_ShouldReturnNote_WhenNoteExists()
    {
      using var context = _fixture.CreateContext();
      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var note = await service.GetTodayNote(TestFixture.TestUserId);

      note.Should().NotBeNull();
      note.DailyNoteID.Should().Be(TestFixture.TodayNoteId);
      note.CreatedAt.Date.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task CreateDailyNote_ShouldCreateAndReturnNote_WhenNoteDoesNotExist()
    {
      using var context = _fixture.CreateContext();
      var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
      context.DailyNote.Remove(todayNote);
      await context.SaveChangesAsync();

      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var note = await service.CreateDailyNote(TestFixture.TestUserId);

      note.Should().NotBeNull();
      note.CreatedAt.Date.Should().Be(DateTime.Today);
      note.UserID.Should().Be(TestFixture.TestUserId);
      note.DailyTargetCalorie.Should().Be(2300);
    }

    [Fact]
    public async Task CreateDailyNote_ShouldUseYesterdayWeight_WhenCreatingNewNote()
    {
      using var context = _fixture.CreateContext();
      var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
      context.DailyNote.Remove(todayNote);
      await context.SaveChangesAsync();

      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var newNote = await service.CreateDailyNote(TestFixture.TestUserId);

      newNote.Should().NotBeNull();
      newNote.DailyWeight.Should().Be(93);
    }

    [Fact]
    public async Task CreateDailyNote_ShouldUseUserWeight_WhenCreatingFirstNote()
    {
      using var context = _fixture.CreateContext();
      var notes = context.DailyNote.Where(d => d.UserID == TestFixture.TestUserId);
      context.DailyNote.RemoveRange(notes);
      await context.SaveChangesAsync();

      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);
      var user = await context.Users.FindAsync(TestFixture.TestUserId);
      user.Weight = 88;
      await context.SaveChangesAsync();

      var newNote = await service.CreateDailyNote(TestFixture.TestUserId);

      newNote.Should().NotBeNull();
      newNote.DailyWeight.Should().Be(88);
    }

    [Fact]
    public async Task CreateDailyNote_ShouldCreateMealEntries_WhenCreatingNewNote()
    {
      using var context = _fixture.CreateContext();
      var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
      context.DailyNote.Remove(todayNote);
      await context.SaveChangesAsync();

      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var newNote = await service.CreateDailyNote(TestFixture.TestUserId);
      var mealEntries = await context.MealEntries.Where(me => me.DailyNoteID == newNote.DailyNoteID).ToListAsync();

      newNote.Should().NotBeNull();
      mealEntries.Count.Should().Be(2);
      mealEntries.Should().Contain(me => me.MealTypeID == TestFixture.MealType1Id);
      mealEntries.Should().Contain(me => me.MealTypeID == TestFixture.MealType2Id);
    }

    [Fact]
    public async Task GetPreviousNote_ShouldReturnYesterdayNote()
    {
      using var context = _fixture.CreateContext();
      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var prevNote = await service.GetPreviousNote(TestFixture.TestUserId, DateTime.Today);

      prevNote.Should().NotBeNull();
      prevNote.DailyNoteID.Should().Be(TestFixture.YesterdayNoteId);
    }

    [Fact]
    public async Task GetPreviousNote_ShouldReturnNull_WhenNoPreviousNoteExists()
    {
      using var context = _fixture.CreateContext();
      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var prevNote = await service.GetPreviousNote(TestFixture.TestUserId, DateTime.Today.AddDays(-1));

      prevNote.Should().BeNull();
    }

    [Fact]
    public async Task UpdateWeight_ShouldUpdateWeight_WhenNoteExists()
    {
      using var context = _fixture.CreateContext();
      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var updatedNote = await service.UpdateWeight(TestFixture.TodayNoteId, 91);
      var noteInDb = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);

      updatedNote.Should().NotBeNull();
      updatedNote.DailyWeight.Should().Be(91);
      noteInDb.DailyWeight.Should().Be(91);
    }

    [Fact]
    public async Task UpdateWeight_ShouldReturnNull_WhenNoteDoesNotExist()
    {
      using var context = _fixture.CreateContext();
      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var updatedNote = await service.UpdateWeight("nem-letezo-id", 90);

      updatedNote.Should().BeNull();
    }

    [Fact]
    public async Task GetAllDailyNotesForGraph_ShouldReturnOnlyNotesWithWeight()
    {
      using var context = _fixture.CreateContext();
      context.DailyNote.Add(new DailyNote
      {
        DailyNoteID = "note-with-zero-weight",
        UserID = TestFixture.TestUserId,
        DailyWeight = 0,
        CreatedAt = DateTime.Today.AddDays(1)
      });
      await context.SaveChangesAsync();

      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var notes = await service.GetAllDailyNotesForGraph();

      notes.Count.Should().Be(2);
      notes.Should().NotContain(n => n.DailyNoteID == "note-with-zero-weight");
    }

    [Fact]
    public async Task GetAllDailyNotesForGraph_ShouldReturnSortedByDate()
    {
      using var context = _fixture.CreateContext();
      var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

      var notes = await service.GetAllDailyNotesForGraph();

      notes.Count.Should().Be(2);
      notes[0].DailyNoteID.Should().Be(TestFixture.YesterdayNoteId);
      notes[1].DailyNoteID.Should().Be(TestFixture.TodayNoteId);
    }
  }
}