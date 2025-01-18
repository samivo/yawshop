using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using YawShop.Services.ClientService.Models;
using YawShop.Services.ProductService.Models;

namespace YawShop.Services.ClientService;

public class ClientService : IClientService
{

    private readonly ILogger<ClientService> _logger;
    private readonly ApplicationDbContext _context;

    public ClientService(ILogger<ClientService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task CreateAsync(ClientModel client)
    {
        try
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to create new client: {ex}", ex.ToString());
            throw;
        }
    }

    public async Task<List<ClientModel>> GetAllAsync()
    {
        try
        {
            var clients = await _context.Clients.Include(client => client.AdditionalInfo).ToListAsync();
            return clients;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to read all clients: {err}", ex.ToString());
            throw;
        }
    }

    public async Task<List<ClientModel>> GetAsync(Expression<Func<ClientModel, bool>> predicate)
    {
        try
        {
            var client = await _context.Clients.Include(client => client.AdditionalInfo).Where(predicate).ToListAsync();
            return client;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to read all clients: {err}", ex.ToString());
            throw;
        }
    }

    public void ValidateAndSanitizeClientInput(ClientModel client)
    {

        // Validate and sanitize First Name
        if (string.IsNullOrWhiteSpace(client.FirstName) || client.FirstName.Length > 50)
        {
            throw new InvalidOperationException("First name is required and must not exceed 50 characters.");
        }
        if (!Regex.IsMatch(client.FirstName, @"^[a-zA-ZäöåÄÖÅ\s\-']+$"))
        {
            throw new InvalidOperationException("First name contains invalid characters.");
        }
        client.FirstName = HttpUtility.HtmlEncode(client.FirstName);

        // Validate and sanitize Last Name
        if (string.IsNullOrWhiteSpace(client.LastName) || client.LastName.Length > 50)
        {
            throw new InvalidOperationException("Last name is required and must not exceed 50 characters.");
        }
        if (!Regex.IsMatch(client.LastName, @"^[a-zA-ZäöåÄÖÅ\s\-']+$"))
        {
            throw new InvalidOperationException("Last name contains invalid characters.");
        }
        client.LastName = HttpUtility.HtmlEncode(client.LastName);

        // Validate and sanitize Email
        if (string.IsNullOrWhiteSpace(client.Email) || client.Email.Length > 254)
        {
            throw new InvalidOperationException("Email is required and must not exceed 254 characters.");
        }
        if (!Regex.IsMatch(client.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new InvalidOperationException("Email is not in a valid format.");
        }
        client.Email = HttpUtility.HtmlEncode(client.Email);


        if (client.AdditionalInfo == null)
        {
            return; // No additional info to check
        }

        var sanitizer = new HtmlSanitizer();

        foreach (var additionalField in client.AdditionalInfo)
        {
            if (additionalField.FieldType == CustomerFieldType.Text)
            {
                if (additionalField.FieldValue == null)
                {
                    continue; // No further validation needed
                }

                additionalField.FieldValue = additionalField.FieldValue.Trim();

                additionalField.FieldValue = sanitizer.Sanitize(additionalField.FieldValue);

                if (additionalField.FieldValue.Length > 500)
                {
                    throw new InvalidOperationException($"Clients additional text field lenght is limited to 500 characters.");
                }

                if (Regex.IsMatch(additionalField.FieldValue, @"<script|on\w+=", RegexOptions.IgnoreCase))
                {
                    throw new InvalidOperationException("Potentially dangerous input detected.");
                }

            }

        }

    }

    public void ValidateAdditionalFields(ClientModel client, List<ProductSpesificClientFields> fieldsFromAllProductsInCart)
    {
        try
        {
            //Products may contain duplicate fields -> list distinct fields.
            var productFields = fieldsFromAllProductsInCart.DistinctBy(t => t.FieldName);

            //Products does not require any fields
            if (!productFields.Any())
            {
                if (client.AdditionalInfo != null && client.AdditionalInfo.Count > 0)
                {
                    throw new InvalidOperationException("Products in cart does not require any additional fields, but client has some anyway?");
                }

                return;
            }

            if (client.AdditionalInfo == null)
            {
                //Product required fields but client has none.
                throw new InvalidOperationException("Client additional datafields missing. Products requires fields and client has none.");
            }

            //Check that all product fields is present in client info
            var clientFieldNames = client.AdditionalInfo.Select(field => field.FieldName);

            if (!productFields.All(field => clientFieldNames.Contains(field.FieldName)))
            {
                throw new InvalidOperationException("Client is missing some additional data fields that is specified in product model.");
            }

            //Check types
            var typeValidators = new Dictionary<CustomerFieldType, Func<string, bool>>
            {
                { CustomerFieldType.Integer, value => int.TryParse(value, out _) },
                { CustomerFieldType.Decimal, value => decimal.TryParse(value, out _) },
                { CustomerFieldType.DateTime, value => DateTime.TryParse(value, out _) },
                { CustomerFieldType.Boolean, value => bool.TryParse(value, out _) },
                { CustomerFieldType.Agreement, value => bool.TryParse(value, out _) }
            };

            foreach (var clientInfo in client.AdditionalInfo)
            {
                var productField = productFields.SingleOrDefault(field => field.FieldName == clientInfo.FieldName) ?? throw new InvalidOperationException("Client has a datafield that aint specified in the product.");

                if (string.IsNullOrEmpty(clientInfo.FieldValue))
                {
                    if (productField.IsRequired)
                    {
                        throw new InvalidOperationException($"Client additional field {clientInfo.FieldName} is required but the value is null or empty.");
                    }

                    return; // Not required and empty, so no further validation needed
                }

                if (typeValidators.TryGetValue(productField.FieldType, out var validator))
                {
                    if (!validator(clientInfo.FieldValue))
                    {
                        throw new InvalidOperationException($"Client additional datafield validation error: {clientInfo.FieldName}: {clientInfo.FieldValue} is not type of {productField.FieldType}.");
                    }
                }
                else if (productField.FieldType != CustomerFieldType.Text)
                {
                    throw new InvalidOperationException($"Unsupported field type: {productField.FieldType}");
                }

            }


            return;

        }
        catch (Exception)
        {
            throw;
        }
    }


}
