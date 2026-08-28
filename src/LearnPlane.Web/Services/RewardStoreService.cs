using System.Data;
using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Services;

public sealed class RewardStoreService(
    IDbContextFactory<LearnPlaneDbContext> dbFactory,
    PointBalanceCalculator balanceCalculator)
{
    public async Task<int> GetBalanceAsync(string userId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await GetBalanceAsync(db, userId);
    }

    public async Task AddToCartAsync(string userId, int rewardItemId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var itemExists = await db.RewardItems.AnyAsync(x => x.Id == rewardItemId && x.IsActive);
        if (!itemExists) throw new InvalidOperationException("Belønningen er ikke tilgjengelig.");

        var cartItem = await db.CartItems.SingleOrDefaultAsync(x => x.UserId == userId && x.RewardItemId == rewardItemId);
        if (cartItem is null)
        {
            db.CartItems.Add(new CartItem { UserId = userId, RewardItemId = rewardItemId });
        }
        else if (cartItem.Quantity < 99)
        {
            cartItem.Quantity++;
        }

        await db.SaveChangesAsync();
    }

    public async Task SetQuantityAsync(string userId, int cartItemId, int quantity)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var cartItem = await db.CartItems.SingleOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId)
            ?? throw new InvalidOperationException("Varen finnes ikke i handlekurven.");
        if (quantity <= 0)
            db.CartItems.Remove(cartItem);
        else
            cartItem.Quantity = Math.Min(quantity, 99);
        await db.SaveChangesAsync();
    }

    public async Task<CheckoutResult> CheckoutAsync(string userId)
    {
        await using var strategyContext = await dbFactory.CreateDbContextAsync();
        var strategy = strategyContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var cart = await db.CartItems
                .Include(x => x.RewardItem)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Id)
                .ToListAsync();

            if (cart.Count == 0)
                return CheckoutResult.Failed("Handlekurven er tom.");
            if (cart.Any(x => !x.RewardItem.IsActive))
                return CheckoutResult.Failed("En vare i handlekurven er ikke lenger tilgjengelig. Fjern den og prøv igjen.");

            var total = cart.Sum(x => checked(x.RewardItem.PricePoints * x.Quantity));
            var balance = await GetBalanceAsync(db, userId);
            if (!balanceCalculator.CanAfford(balance, total))
                return CheckoutResult.Failed($"Du mangler {total - balance} poeng for å gjennomføre kjøpet.", balance);

            var purchase = new RewardPurchase
            {
                UserId = userId,
                TotalPoints = total,
                Lines = cart.Select(x => new RewardPurchaseLine
                {
                    RewardItemId = x.RewardItemId,
                    ItemName = x.RewardItem.Name,
                    ItemDescription = x.RewardItem.Description,
                    ImageUrl = x.RewardItem.ImageUrl,
                    UnitPricePoints = x.RewardItem.PricePoints,
                    Quantity = x.Quantity
                }).ToList()
            };
            db.RewardPurchases.Add(purchase);
            db.CartItems.RemoveRange(cart);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return CheckoutResult.Succeeded(purchase.Id, total, balance - total);
        });
    }

    private async Task<int> GetBalanceAsync(LearnPlaneDbContext db, string userId)
    {
        var earned = await db.QuizAttempts.Where(x => x.UserId == userId).SumAsync(x => (int?)x.PointsAwarded) ?? 0;
        var spent = await db.RewardPurchases.Where(x => x.UserId == userId).SumAsync(x => (int?)x.TotalPoints) ?? 0;
        return balanceCalculator.Calculate(earned, spent);
    }
}

public sealed record CheckoutResult(bool Success, string Message, int? PurchaseId, int TotalPoints, int RemainingBalance)
{
    public static CheckoutResult Failed(string message, int balance = 0) => new(false, message, null, 0, balance);
    public static CheckoutResult Succeeded(int purchaseId, int total, int balance) =>
        new(true, "Kjøpet er gjennomført!", purchaseId, total, balance);
}
