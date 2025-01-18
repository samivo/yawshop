using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace YawShop.Services.PaytrailService.Models;

public class CallbackModel
{
    [FromQuery(Name = "checkout-account")]
    public required long Account { get; set; }

    [FromQuery(Name = "checkout-algorithm")]
    public required string Algorithm { get; set; }

    [FromQuery(Name = "checkout-amount")]
    public required long Amount { get; set; }

    [FromQuery(Name = "checkout-settlement-reference")]
    public string? SettlementReference { get; set; }

    [FromQuery(Name = "checkout-stamp")]
    public required string Stamp { get; set; }

    [FromQuery(Name = "checkout-reference")]
    public required string Reference { get; set; }

    [FromQuery(Name = "checkout-transaction-id")]
    public string TransactionId { get; set; } = "";

    [FromQuery(Name = "checkout-status")]
    public required string Status { get; set; }

    [FromQuery(Name = "checkout-provider")]
    public required string Provider { get; set; }

    [FromQuery(Name = "signature")]
    public required string Signature { get; set; }
}

public enum PaytrailPaymentStatuses
{
    New,
    Ok,
    Fail,
    Pending,
    Delayed
}
