using FluentAssertions;
using HealthyAPI.Data;
using HealthyAPI.DTOs.MealFoods;
using HealthyAPI.DTOs.MealRecipe;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HealthyAPI.Tests
{
    // Ezek a tesztek a meglévő TestFixture-t használják
    public class MealFoodsServiceIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public MealFoodsServiceIntegrationTests(TestFixture fixture) => _fixture = fixture;

        private static Context NewCtx() =>
            new Context(new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        [Fact]
        public async Task CreateMealFoods_Should_UpdateEntrySums_And_CallDailyNote()
        {
            using var context = _fixture.CreateContext();

            // Készítünk egy üres MealEntry-t a mai naplóhoz
            var entry = new MealEntries { MealEntryID = "ME_F1", DailyNoteID = TestFixture.TodayNoteId };
            context.MealEntries.Add(entry);
            await context.SaveChangesAsync();

            // Használjuk a seedelt "Csirkemell" ételt (Food1Id)
            var chicken = await context.Food.FindAsync(TestFixture.Food1Id);
            chicken.Should().NotBeNull();

            // Mock DailyNoteService: ellenőrizzük, hogy UpdateMealNutritionAsync meghívódik a TODAY note-tal
            var dn = new Mock<IDailyNoteService>(MockBehavior.Strict);
            dn.Setup(x => x.UpdateMealNutritionAsync(TestFixture.TodayNoteId))
              .Returns(Task.CompletedTask)
              .Verifiable();

            var sut = new MealFoodsService(context, dn.Object);

            // 200 g csirke
            var created = await sut.CreateMealFoods(new MealFoodCreateDto
            {
                MealEntryID = entry.MealEntryID,
                FoodID = TestFixture.Food1Id,
                Quantity = 200
            });

            var refreshed = await context.MealEntries.FindAsync(entry.MealEntryID);
            refreshed!.SumProtein.Should().BeApproximately(62f, 0.001f); // 31 * 2.0
            refreshed.SumFat.Should().BeApproximately(6f, 1.5f); // nálad Fat=3 (nem 4), ezért ~6 (3*2) – toleranciával
            refreshed.SumCalorie.Should().Be(330);               // 165 * 2

            dn.Verify();
        }

        [Fact]
        public async Task UpdateMealFoods_Should_RescaleEntrySums()
        {
            using var context = _fixture.CreateContext();

            var entry = new MealEntries { MealEntryID = "ME_F2", DailyNoteID = TestFixture.TodayNoteId };
            context.MealEntries.Add(entry);
            await context.SaveChangesAsync();

            var rice = await context.Food.FindAsync(TestFixture.Food2Id);
            rice.Should().NotBeNull(); // Rizs: P=2, C=28, F=0, Cal=130 per 100 g

            var dn = new Mock<IDailyNoteService>();
            dn.Setup(x => x.UpdateMealNutritionAsync(TestFixture.TodayNoteId))
              .Returns(Task.CompletedTask);

            var sut = new MealFoodsService(context, dn.Object);

            // 100 g → C=28
            var mf = await sut.CreateMealFoods(new MealFoodCreateDto
            {
                MealEntryID = entry.MealEntryID,
                FoodID = TestFixture.Food2Id,
                Quantity = 100
            });
            (await context.MealEntries.FindAsync(entry.MealEntryID))!
                .SumCarb.Should().BeApproximately(28f, 0.001f);

            // 150 g → C=42
            await sut.UpdateMealFoods(mf.MealFoodID, new MealFoodCreateDto
            {
                MealEntryID = entry.MealEntryID,
                FoodID = TestFixture.Food2Id,
                Quantity = 150
            });

            (await context.MealEntries.FindAsync(entry.MealEntryID))!
                .SumCarb.Should().BeApproximately(42f, 0.001f);
        }

        [Fact]
        public async Task DeleteMealFoods_Should_DecreaseEntrySums()
        {
            using var context = _fixture.CreateContext();

            var entry = new MealEntries { MealEntryID = "ME_F3", DailyNoteID = TestFixture.TodayNoteId };
            context.MealEntries.Add(entry);
            await context.SaveChangesAsync();

            var dn = new Mock<IDailyNoteService>();
            dn.Setup(x => x.UpdateMealNutritionAsync(TestFixture.TodayNoteId))
              .Returns(Task.CompletedTask);

            var sut = new MealFoodsService(context, dn.Object);

            // 100 g csirke + 100 g rizs
            var a = await sut.CreateMealFoods(new MealFoodCreateDto { MealEntryID = entry.MealEntryID, FoodID = TestFixture.Food1Id, Quantity = 100 });
            var b = await sut.CreateMealFoods(new MealFoodCreateDto { MealEntryID = entry.MealEntryID, FoodID = TestFixture.Food2Id, Quantity = 100 });

            var before = await context.MealEntries.FindAsync(entry.MealEntryID);
            before!.SumProtein.Should().BeGreaterThan(0);
            before.SumCarb.Should().BeGreaterThan(0);

            await sut.DeleteMealFoods(a.MealFoodID);

            // Várakozás: csak a rizs marad (100 g)
            var after = await context.MealEntries.FindAsync(entry.MealEntryID);
            after!.SumProtein.Should().BeApproximately(2f, 0.001f);   // Rizs 2 g / 100 g
            after.SumCarb.Should().BeApproximately(28f, 0.001f);   // Rizs 28 g / 100 g
            after.SumFat.Should().BeApproximately(0f, 0.001f);
        }
    }

    public class MealRecipesServiceIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public MealRecipesServiceIntegrationTests(TestFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task CreateRecipe_Should_ScaleByQuantity_And_CallDailyNote()
        {
            using var context = _fixture.CreateContext();

            var entry = new MealEntries { MealEntryID = "ME_R1", DailyNoteID = TestFixture.TodayNoteId };
            context.MealEntries.Add(entry);
            await context.SaveChangesAsync();

            // Seedben van egy "Csirkerizs" recept (Recipe1Id) Sum* mezőkkel
            var recipe = await context.Recipe.FindAsync(TestFixture.Recipe1Id);
            recipe.Should().NotBeNull();

            var dn = new Mock<IDailyNoteService>(MockBehavior.Strict);
            dn.Setup(x => x.UpdateMealNutritionAsync(TestFixture.TodayNoteId))
              .Returns(Task.CompletedTask).Verifiable();

            var sut = new MealRecipesService(context, dn.Object);

            // 1.5 adag
            await sut.Create(new MealRecipeCreateDto
            {
                MealEntryID = entry.MealEntryID,
                RecipeID = TestFixture.Recipe1Id,
                Quantity = 1.5f
            });

            var e = await context.MealEntries.FindAsync(entry.MealEntryID);
            e!.SumProtein.Should().BeApproximately(33f * 1.5f, 0.001f);
            e.SumCarb.Should().BeApproximately(28f * 1.5f, 0.001f);
            e.SumFat.Should().BeApproximately(3f * 1.5f, 0.001f);

            dn.Verify();
        }

        [Fact]
        public async Task UpdateRecipe_Should_RescaleEntrySums()
        {
            using var context = _fixture.CreateContext();

            var entry = new MealEntries { MealEntryID = "ME_R2", DailyNoteID = TestFixture.TodayNoteId };
            context.MealEntries.Add(entry);
            await context.SaveChangesAsync();

            var dn = new Mock<IDailyNoteService>();
            dn.Setup(x => x.UpdateMealNutritionAsync(TestFixture.TodayNoteId))
              .Returns(Task.CompletedTask);

            var sut = new MealRecipesService(context, dn.Object);

            // 1.0 adag
            var created = await sut.Create(new MealRecipeCreateDto
            {
                MealEntryID = entry.MealEntryID,
                RecipeID = TestFixture.Recipe1Id,
                Quantity = 1.0f
            });

            var baseEntry = await context.MealEntries.FindAsync(entry.MealEntryID);
            baseEntry!.SumProtein.Should().BeApproximately(33f, 0.001f);

            // 2.5 adagra növeljük
            await sut.Update(created.MealRecipeID, new MealRecipeCreateDto
            {
                MealEntryID = entry.MealEntryID,
                RecipeID = TestFixture.Recipe1Id,
                Quantity = 2.5f
            });

            var after = await context.MealEntries.FindAsync(entry.MealEntryID);
            after!.SumProtein.Should().BeApproximately(33f * 2.5f, 0.001f);
            after.SumCarb.Should().BeApproximately(28f * 2.5f, 0.001f);
            after.SumFat.Should().BeApproximately(3f * 2.5f, 0.001f);
        }

        [Fact]
        public async Task DeleteRecipe_Should_DecreaseEntrySums()
        {
            using var context = _fixture.CreateContext();

            var entry = new MealEntries { MealEntryID = "ME_R3", DailyNoteID = TestFixture.TodayNoteId };
            context.MealEntries.Add(entry);
            await context.SaveChangesAsync();

            var dn = new Moq.Mock<IDailyNoteService>();
            dn.Setup(x => x.UpdateMealNutritionAsync(TestFixture.TodayNoteId))
              .Returns(Task.CompletedTask);

            var sut = new MealRecipesService(context, dn.Object);

            // 1 db recept: 1.0 adag
            var created = await sut.Create(new MealRecipeCreateDto
            {
                MealEntryID = entry.MealEntryID,
                RecipeID = TestFixture.Recipe1Id, // "Csirkerizs" – SumProtein=33, SumCarb=28, SumFat=3
                Quantity = 1.0f
            });

            var before = await context.MealEntries.FindAsync(entry.MealEntryID);
            before!.SumProtein.Should().BeApproximately(33f, 0.001f);
            before.SumCarb.Should().BeApproximately(28f, 0.001f);
            before.SumFat.Should().BeApproximately(3f, 0.001f);

            // Törlés
            await sut.Delete(created.MealRecipeID);

            var after = await context.MealEntries.FindAsync(entry.MealEntryID);
            after!.SumProtein.Should().BeApproximately(0f, 0.001f);
            after.SumCarb.Should().BeApproximately(0f, 0.001f);
            after.SumFat.Should().BeApproximately(0f, 0.001f);
        }

    }

    public class DailyNoteRecalcEndToEndTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;
        public DailyNoteRecalcEndToEndTests(TestFixture f) => _fixture = f;

        [Fact]
        public async Task AfterFoodsAndRecipes_Modifications_DailyNote_UpdateMealNutrition_Should_AggregateActuals()
        {
            using var context = _fixture.CreateContext();

            var dnId = TestFixture.TodayNoteId;

            // Készítünk két belső étkezést
            var me1 = new MealEntries { MealEntryID = "ME_E2E_1", DailyNoteID = dnId };
            var me2 = new MealEntries { MealEntryID = "ME_E2E_2", DailyNoteID = dnId };
            context.MealEntries.AddRange(me1, me2);
            await context.SaveChangesAsync();

            // me1: 200 g csirke
            context.MealFoods.Add(new MealFoods { MealEntryID = me1.MealEntryID, FoodID = TestFixture.Food1Id, Quantity = 200 });
            // me2: 1.5 adag „Csirkerizs”
            context.MealRecipes.Add(new MealRecipes { MealEntryID = me2.MealEntryID, RecipeID = TestFixture.Recipe1Id, Quantity = 1.5f });
            await context.SaveChangesAsync();

            var dnService = new DailyNoteService(context, new HttpContextAccessor());

            await dnService.UpdateMealNutritionAsync(dnId);

            var dn = await context.DailyNote.FindAsync(dnId);
            dn!.ActualSumProtein.Should().BeApproximately(62f + 33f * 1.5f, 0.01f);
            dn.ActualSumCarb.Should().BeApproximately(0f + 28f * 1.5f, 0.01f);
            dn.ActualSumFat.Should().BeApproximately(6f + 3f * 1.5f, 0.5f); // csirke zsírja nálad 3/100g → 6
            dn.ActualCalorie.Should().BeGreaterThan(0);
        }
    }
}
