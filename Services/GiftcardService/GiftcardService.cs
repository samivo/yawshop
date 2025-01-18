using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YawShop.Attributes;
using YawShop.Services.EmailService;
using YawShop.Services.GiftcardService.Models;
using YawShop.Utilities;

namespace YawShop.Services.GiftcardService;

public class GiftcardService : IGiftcardService
{

    private readonly ILogger<GiftcardService> _logger;
    private readonly IEmailer _emailer;
    private readonly ApplicationDbContext _context;

    public GiftcardService(ILogger<GiftcardService> logger, IEmailer emailer, ApplicationDbContext context)
    {
        _logger = logger;
        _emailer = emailer;
        _context = context;
    }

    public async Task<List<GiftcardModel>> FindAsNoTrackingAsync(Expression<Func<GiftcardModel, bool>> predicate)
    {

        try
        {
            var giftcards = await _context.Giftcards
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();

            return giftcards;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to find giftcards: {ex}", ex.ToString());
            throw;
        }

    }

    public async Task<List<GiftcardModel>> FindAsync(Expression<Func<GiftcardModel, bool>> predicate)
    {

        try
        {
            var giftcards = await _context.Giftcards
            .Where(predicate)
            .ToListAsync();

            return giftcards;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to find giftcards: {ex}", ex.ToString());
            throw;
        }

    }

    public async Task CreateAsync(GiftcardModel giftcardModel)
    {
        try
        {
            if (await _context.Giftcards.AnyAsync(g => g.Code == giftcardModel.Code))
            {
                throw new InvalidOperationException("Duplicate giftcard code in database. Operation failed!");
            }

            _context.Giftcards.Add(giftcardModel);
            await _context.SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Creating giftcard failed {ex}", ex.ToString());
            throw;
        }
    }

    public async Task UpdateAsync(string giftcardCode, GiftcardModel UpdatedGiftcard)
    {
        try
        {
            var oldGiftcard = await _context.Giftcards.SingleAsync(g => g.Code == giftcardCode);

            PropertyCopy.CopyWithoutAttribute(UpdatedGiftcard, oldGiftcard, typeof(NoApiUpdateAttribute));

            await _context.SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Updating giftcard failed {ex}", ex.ToString());
            throw;
        }
    }

    public async Task DeleteAsync(string giftcardCode)
    {
        try
        {
            var giftcard = await _context.Giftcards.SingleAsync(g => g.Code == giftcardCode);

            await _context.SaveChangesAsync();

            return;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Giftcard set used failed {ex}", ex.ToString());
            throw;
        }
    }

    public async Task SendGiftcardEmailAsync(string giftcardCode)
    {

        try
        {
            var giftcard = await _context.Giftcards.SingleAsync(g => g.Code == giftcardCode);
            var owner = await _context.Clients.SingleAsync(c => c.Id == giftcard.OwnerClientId);

            if (owner.Email == null)
            {
                throw new InvalidOperationException("Failed to send email to owner. Client info probably anonymized!");
            }

            var message = new EmailMessage();
            message.To.Add(owner.Email);
            message.Subject = giftcard.Name;
            message.Body = GiftcardEmail.GetEmailBody(giftcardCode);

            await _emailer.SendMailAsync(message);

        }
        catch (Exception ex)
        {
            _logger.LogWarning("Giftcard email failed. {ex}", ex.ToString());
            throw;
        }
    }

}