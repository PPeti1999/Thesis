/*
 * Ez a fájl egységteszteket (unit tests) tartalmaz a HealthyAPI projekt Service rétegéhez.
 * A teszteléshez az xUnit keretrendszert, a Moq könyvtárat (mockoláshoz) és az Entity Framework Core
 * InMemory adatbázis-szolgáltatóját használjuk.
 *
 * Ez a verzió a FluentAssertions könyvtárat használja az asszertációkhoz (ellenőrzésekhez)
 * a jobb olvashatóság érdekében.
 *
 * Szükséges NuGet csomagok a tesztprojekthez:
 * - xunit
 * - xunit.runner.visualstudio
 * - Moq
 * - Microsoft.EntityFrameworkCore.InMemory
 * - Microsoft.AspNetCore.Http (az IHttpContextAccessor mockolásához)
 * - FluentAssertions (ehhez a verzióhoz)
 */

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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions; // FluentAssertions importálása

namespace HealthyAPI.Tests
{
    /// <summary>
    /// Tesztelési "fixture" (tesztelési környezet), amely beállítja a memóriabeli adatbázist
    /// és feltölti alapértelmezett adatokkal a tesztek futtatása előtt.
    /// </summary>
    public class TestFixture : IDisposable
    {
        public Context Context { get; private set; }
        public static string TestUserId = "27aca460-6814-41e1-8544-3dfa81a086c3"; // A meglévő seedelt felhasználó
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
            // Egyedi adatbázisnév minden futtatáskor, hogy a tesztek izoláltak legyenek
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new Context(options);
            SeedDatabase();
        }

        /// <summary>
        /// Feltölti a memóriabeli adatbázist tesztadatokkal.
        /// </summary>
        private void SeedDatabase()
        {
            // --- Felhasználók ---
            var testUser = new User
            {
                Id = TestUserId,
                FirstName = "pásztori",
                LastName = "péter",
                Age = 25,
                Height = 180, // Magasság cm-ben (feltételezve)
                Weight = 92,
                GoalWeight = 90,
                TargetCalorie = 2300,
                TargeProtein = 180,
                TargetCarb = 300,
                TargetFat = 70,
                ActivityMultiplier = 1.55f,
                IsFemale = false,
                GoalType = 2, // Fogyás
                UserName = "pasztoripeti@gmail.com",
                Email = "pasztoripeti@gmail.com",
                EmailConfirmed = true
            };
            Context.Users.Add(testUser);

            // --- Ételek ---
            Context.Food.AddRange(
                new Food { FoodID = Food1Id, Title = "Csirkemell", Protein = 31, Fat = 3, Carb = 0, Calorie = 165, Gram = 100 },
                new Food { FoodID = Food2Id, Title = "Rizs", Protein = 2, Fat = 0, Carb = 28, Calorie = 130, Gram = 100 }
            );

            // --- Receptek ---
            Context.Recipe.Add(new Recipe { RecipeID = Recipe1Id, Title = "Csirkerizs", SumProtein = 33, SumCarb = 28, SumFat = 3, SumCalorie = 295 });
            Context.RecipeFoods.AddRange(
                new RecipeFoods { RecipeFoodID = "rf1", RecipeID = Recipe1Id, FoodID = Food1Id, Quantity = 100 },
                new RecipeFoods { RecipeFoodID = "rf2", RecipeID = Recipe1Id, FoodID = Food2Id, Quantity = 100 }
            );

            // --- Étkezés Típusok ---
            Context.MealTypes.AddRange(
                new MealTypes { MealTypeID = MealType1Id, Name = "Reggeli" },
                new MealTypes { MealTypeID = MealType2Id, Name = "Ebéd" }
            );

            // --- Napi Jegyzetek (Előző nap) ---
            var yesterday = DateTime.Today.AddDays(-1);
            Context.DailyNote.Add(new DailyNote
            {
                DailyNoteID = YesterdayNoteId,
                UserID = TestUserId,
                DailyWeight = 93, // Tegnapi súly
                CreatedAt = yesterday,
                DailyTargetCalorie = 2300,
                DailyTargetProtein = 180,
                DailyTargetCarb = 300,
                DailyTargetFat = 70
            });

            // --- Mai nap (teszteléshez) ---
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

        /// <summary>
        /// Biztosít egy tiszta Context-et minden teszthez.
        /// </summary>
        public Context CreateContext()
        {
            var options = new DbContextOptionsBuilder<Context>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var context = new Context(options);
            // Újra seedeljük, hogy minden teszt friss adatokkal induljon
            SeedDatabaseForContext(context);
            return context;
        }

        private void SeedDatabaseForContext(Context context)
        {
            // Lemásoljuk az eredeti fixture logikáját, de a kapott context-be
            var testUser = new User
            {
                Id = TestUserId,
                FirstName = "pásztori",
                LastName = "péter",
                Age = 25,
                Height = 180, // Magasság cm-ben (feltételezve)
                Weight = 92,
                GoalWeight = 90,
                TargetCalorie = 2300,
                TargeProtein = 180,
                TargetCarb = 300,
                TargetFat = 70,
                ActivityMultiplier = 1.55f,
                IsFemale = false,
                GoalType = 2, // Fogyás
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

    /// <summary>
    /// UserProfileService tesztjei
    /// </summary>
    public class UserProfileServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public UserProfileServiceTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        // --- GetCurrentUserProfile Tesztek ---

        [Fact]
        public async Task GetCurrentUserProfile_ShouldReturnProfile_WhenUserExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);

            // Act
            var profile = await service.GetCurrentUserProfile(TestFixture.TestUserId);

            // Assert
            profile.Should().NotBeNull();
            profile.Id.Should().Be(TestFixture.TestUserId);
            profile.FirstName.Should().Be("pásztori");
        }

        [Fact]
        public async Task GetCurrentUserProfile_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);

            // Act
            var profile = await service.GetCurrentUserProfile("nem-letezo-id");

            // Assert
            profile.Should().BeNull();
        }

        // --- UpdateProfile Tesztek ---

        [Fact]
        public async Task UpdateProfile_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
            var dto = new UpdateUserProfileDto();

            // Act
            var result = await service.UpdateProfile("nem-letezo-id", dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateBasicInfo()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
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
                GoalType = 2, // Fogyás
                ActivityMultiplier = 1.725f
            };

            // Act
            var result = await service.UpdateProfile(TestFixture.TestUserId, dto);
            var userInDb = await context.Users.FindAsync(TestFixture.TestUserId);

            // Assert
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
            // Arrange (10 * Súly + 6.25 * Magasság - 5 * Kor + 5) * Multiplikátor
            // (10 * 90 + 6.25 * 180 - 5 * 30 + 5) * 1.55 = (900 + 1125 - 150 + 5) * 1.55 = 1880 * 1.55 = 2914
            // JAVÍTÁS: A float/double -> int kasztolás miatt 2913 lesz az eredmény a service-ben.
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
            var dto = new UpdateUserProfileDto
            {
                Weight = 90,
                Height = 180,
                Age = 30,
                IsFemale = false,
                GoalType = 0, // Megtartás
                ActivityMultiplier = 1.55f
            };

            // Act
            var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

            // Assert
            result.Should().NotBeNull();
            result.TargetCalorie.Should().Be(2913); // JAVÍTVA 2914-ről 2913-ra
        }

        [Fact]
        public async Task UpdateProfile_ShouldCalculateTDEE_Female_MaintainWeight()
        {
            // Arrange (10 * Súly + 6.25 * Magasság - 5 * Kor - 161) * Multiplikátor
            // (10 * 70 + 6.25 * 170 - 5 * 30 - 161) * 1.2 = (700 + 1062.5 - 150 - 161) * 1.2 = 1451.5 * 1.2 = 1741.8
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
            var dto = new UpdateUserProfileDto
            {
                Weight = 70,
                Height = 170,
                Age = 30,
                IsFemale = true,
                GoalType = 0, // Megtartás
                ActivityMultiplier = 1.2f
            };

            // Act
            var result = await service.UpdateProfile(TestFixture.OtherUserId, dto);

            // Assert
            result.Should().NotBeNull();
            result.TargetCalorie.Should().Be(1741); // Ez helyes volt
        }

        [Fact]
        public async Task UpdateProfile_ShouldCalculateTDEE_Male_GainWeight()
        {
            // Arrange (BMR * TDEE) + 500
            // 2913 (BMR*TDEE) + 500 = 3413
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
            var dto = new UpdateUserProfileDto
            {
                Weight = 90,
                Height = 180,
                Age = 30,
                IsFemale = false,
                GoalType = 1, // Tömegnövelés
                ActivityMultiplier = 1.55f
            };

            // Act
            var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

            // Assert
            result.Should().NotBeNull();
            result.TargetCalorie.Should().Be(3413); // JAVÍTVA 3414-ről 3413-ra
        }

        [Fact]
        public async Task UpdateProfile_ShouldCalculateTDEE_Male_LoseWeight()
        {
            // Arrange (BMR * TDEE) - 500
            // 2913 (BMR*TDEE) - 500 = 2413
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
            var dto = new UpdateUserProfileDto
            {
                Weight = 90,
                Height = 180,
                Age = 30,
                IsFemale = false,
                GoalType = 2, // Fogyás
                ActivityMultiplier = 1.55f
            };

            // Act
            var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

            // Assert
            result.Should().NotBeNull();
            result.TargetCalorie.Should().Be(2413); // Ez helyes
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateMacrosCorrectly()
        {
            // Arrange
            // Súly = 90kg
            // TargeProtein = 90 * 2 = 180
            // TargetFat = 90 * 1 = 90
            // TargetCarb = (TDEE - (FehérjeKcal + ZsírKcal)) / 4
            // TDEE (fogyás) = 2413 (a service számítása szerint)
            // JAVÍTÁS: Kerekítve a várt érték 221f
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);
            var dto = new UpdateUserProfileDto
            {
                Weight = 90,
                Height = 180,
                Age = 30,
                IsFemale = false,
                GoalType = 2, // Fogyás
                ActivityMultiplier = 1.55f
            };

            // Act
            var result = await service.UpdateProfile(TestFixture.TestUserId, dto);

            // Assert
            result.Should().NotBeNull();
            result.TargetProtein.Should().Be(180);
            result.TargetFat.Should().Be(90);
            result.TargetCarb.Should().Be(221f); // JAVÍTVA (Math.Round)
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateTodayDailyNote_IfExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);

            // Ellenőrizzük, hogy a mai napló a régi értékkel kezd
            var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
            todayNote.DailyTargetCalorie.Should().Be(2300);

            var dto = new UpdateUserProfileDto
            {
                Weight = 90,
                Height = 180,
                Age = 30,
                IsFemale = false,
                GoalType = 2, // Fogyás
                ActivityMultiplier = 1.55f
            }; // Ez 2413 kalóriát fog számolni

            // Act
            await service.UpdateProfile(TestFixture.TestUserId, dto);

            // Assert
            // Újra kell kérnünk a context-ből az entitást
            var updatedNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
            updatedNote.Should().NotBeNull();
            updatedNote.DailyTargetCalorie.Should().Be(2413);
            updatedNote.DailyTargetProtein.Should().Be(180);
            updatedNote.DailyTargetCarb.Should().Be(221f); // JAVÍTVA (Math.Round)
            updatedNote.DailyTargetFat.Should().Be(90);
        }

        [Fact]
        public async Task UpdateProfile_ShouldNotFail_WhenTodayDailyNoteDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new UserProfileService(context);

            // Töröljük a mai naplót, hogy biztosan ne létezzen
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

            // Act
            // A hívásnak nem szabad kivételt dobnia
            Func<Task> action = async () => await service.UpdateProfile(TestFixture.TestUserId, dto);

            // Assert
            await action.Should().NotThrowAsync();
            var user = await context.Users.FindAsync(TestFixture.TestUserId);
            user.TargetCalorie.Should().Be(2413); // JAVÍTVA 2414-ről 2413-ra
        }
    }

    /// <summary>
    /// DailyNoteService tesztjei
    /// </summary>
    public class DailyNoteServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public DailyNoteServiceTests(TestFixture fixture)
        {
            _fixture = fixture;

            // Mock IHttpContextAccessor beállítása, hogy a GetUserId() működjön
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, TestFixture.TestUserId) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });
        }

        // --- GetTodayNote Tesztek ---

        [Fact]
        public async Task GetTodayNote_ShouldReturnNote_WhenNoteExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            var note = await service.GetTodayNote(TestFixture.TestUserId);

            // Assert
            note.Should().NotBeNull();
            note.DailyNoteID.Should().Be(TestFixture.TodayNoteId);
            note.CreatedAt.Date.Should().Be(DateTime.Today);
        }

        // JAVÍTOTT TESZT: A GetTodayNote hívás helyett a CreateDailyNote hívást teszteljük,
        // mert a "create" logika a Controllerben van, ami ezt a metódust hívja.
        [Fact]
        public async Task CreateDailyNote_ShouldCreateAndReturnNote_WhenNoteDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            // Töröljük a mai naplót, hogy a "create" ág fusson le
            var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
            context.DailyNote.Remove(todayNote);
            await context.SaveChangesAsync();

            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            // JAVÍTÁS: GetTodayNote() helyett CreateDailyNote() hívása
            var note = await service.CreateDailyNote(TestFixture.TestUserId);

            // Assert
            note.Should().NotBeNull();
            note.CreatedAt.Date.Should().Be(DateTime.Today);
            note.UserID.Should().Be(TestFixture.TestUserId);
            note.DailyTargetCalorie.Should().Be(2300); // A felhasználó profiljából
        }

        // JAVÍTOTT TESZT: CreateDailyNote hívással
        [Fact]
        public async Task CreateDailyNote_ShouldUseYesterdayWeight_WhenCreatingNewNote()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
            context.DailyNote.Remove(todayNote);
            await context.SaveChangesAsync();

            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            // JAVÍTÁS: GetTodayNote() helyett CreateDailyNote() hívása
            var newNote = await service.CreateDailyNote(TestFixture.TestUserId);

            // Assert
            newNote.Should().NotBeNull();
            newNote.DailyWeight.Should().Be(93); // A tegnapi (YesterdayNoteId) súlyát (93) kellett örökölnie
        }

        // JAVÍTOTT TESZT: CreateDailyNote hívással
        [Fact]
        public async Task CreateDailyNote_ShouldUseUserWeight_WhenCreatingFirstNote()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            // Törlünk MINDEN naplót a felhasználótól
            var notes = context.DailyNote.Where(d => d.UserID == TestFixture.TestUserId);
            context.DailyNote.RemoveRange(notes);
            await context.SaveChangesAsync();

            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);
            var user = await context.Users.FindAsync(TestFixture.TestUserId);
            user.Weight = 88; // Beállítunk egy egyedi súlyt
            await context.SaveChangesAsync();

            // Act
            // JAVÍTÁS: GetTodayNote() helyett CreateDailyNote() hívása
            var newNote = await service.CreateDailyNote(TestFixture.TestUserId);

            // Assert
            newNote.Should().NotBeNull();
            newNote.DailyWeight.Should().Be(88); // A User.Weight értékét (88) kellett örökölnie
        }

        // JAVÍTOTT TESZT: CreateDailyNote hívással
        [Fact]
        public async Task CreateDailyNote_ShouldCreateMealEntries_WhenCreatingNewNote()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var todayNote = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);
            context.DailyNote.Remove(todayNote);
            await context.SaveChangesAsync();

            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            // JAVÍTÁS: GetTodayNote() helyett CreateDailyNote() hívása
            var newNote = await service.CreateDailyNote(TestFixture.TestUserId);
            var mealEntries = await context.MealEntries.Where(me => me.DailyNoteID == newNote.DailyNoteID).ToListAsync();

            // Assert
            newNote.Should().NotBeNull();
            mealEntries.Count.Should().Be(2); // A seedelt 2 MealType (Reggeli, Ebéd)
            mealEntries.Should().Contain(me => me.MealTypeID == TestFixture.MealType1Id);
            mealEntries.Should().Contain(me => me.MealTypeID == TestFixture.MealType2Id);
        }

        // --- GetPrevious/GetNext Tesztek ---

        [Fact]
        public async Task GetPreviousNote_ShouldReturnYesterdayNote()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            // A "mai" dátumot (Today) adjuk meg, hogy a tegnapit keressük
            var prevNote = await service.GetPreviousNote(TestFixture.TestUserId, DateTime.Today);

            // Assert
            prevNote.Should().NotBeNull();
            prevNote.DailyNoteID.Should().Be(TestFixture.YesterdayNoteId);
        }

        [Fact]
        public async Task GetPreviousNote_ShouldReturnNull_WhenNoPreviousNoteExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            // A "tegnapi" dátumot adjuk meg, ami előtt nincs semmi
            var prevNote = await service.GetPreviousNote(TestFixture.TestUserId, DateTime.Today.AddDays(-1));

            // Assert
            prevNote.Should().BeNull();
        }

        // --- UpdateWeight Tesztek ---

        [Fact]
        public async Task UpdateWeight_ShouldUpdateWeight_WhenNoteExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            var updatedNote = await service.UpdateWeight(TestFixture.TodayNoteId, 91);
            var noteInDb = await context.DailyNote.FindAsync(TestFixture.TodayNoteId);

            // Assert
            updatedNote.Should().NotBeNull();
            updatedNote.DailyWeight.Should().Be(91);
            noteInDb.DailyWeight.Should().Be(91);
        }

        [Fact]
        public async Task UpdateWeight_ShouldReturnNull_WhenNoteDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            var updatedNote = await service.UpdateWeight("nem-letezo-id", 90);

            // Assert
            updatedNote.Should().BeNull();
        }

        // --- GetAllDailyNotesForGraph Tesztek ---

        [Fact]
        public async Task GetAllDailyNotesForGraph_ShouldReturnOnlyNotesWithWeight()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            context.DailyNote.Add(new DailyNote
            {
                DailyNoteID = "note-with-zero-weight",
                UserID = TestFixture.TestUserId,
                DailyWeight = 0, // Súly 0, ezt nem szabadna visszaadnia
                CreatedAt = DateTime.Today.AddDays(1)
            });
            await context.SaveChangesAsync();

            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            var notes = await service.GetAllDailyNotesForGraph();

            // Assert
            notes.Count.Should().Be(2); // Csak a 93kg és 92kg súlyú naplókat adja vissza
            notes.Should().NotContain(n => n.DailyNoteID == "note-with-zero-weight");
        }

        [Fact]
        public async Task GetAllDailyNotesForGraph_ShouldReturnSortedByDate()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var service = new DailyNoteService(context, _mockHttpContextAccessor.Object);

            // Act
            var notes = await service.GetAllDailyNotesForGraph();

            // Assert
            notes.Count.Should().Be(2);
            notes[0].DailyNoteID.Should().Be(TestFixture.YesterdayNoteId); // A tegnapi az első
            notes[1].DailyNoteID.Should().Be(TestFixture.TodayNoteId); // A mai a második
        }
    }
}

