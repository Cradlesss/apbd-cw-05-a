using System;

namespace LegacyRenewalApp;

public class RenewalInvoiceFactory
{
    public RenewalInvoice Create(
        Customer customer,
        string normalizedPlanCode,
        string normalizedPaymentMethod,
        int seatCount,
        decimal baseAmount,
        decimal discountAmount,
        decimal supportFee,
        decimal paymentFee,
        decimal taxAmount,
        decimal finalAmount,
        string notes)
    {
        RenewalInvoice invoice = new RenewalInvoice();
        invoice.InvoiceNumber = "INV-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + customer.Id + "-" + normalizedPlanCode;
        invoice.CustomerName = customer.FullName;
        invoice.PlanCode = normalizedPlanCode;
        invoice.PaymentMethod = normalizedPaymentMethod;
        invoice.SeatCount = seatCount;
        invoice.BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero);
        invoice.DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero);
        invoice.SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero);
        invoice.PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero);
        invoice.TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
        invoice.FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero);
        invoice.Notes = notes.Trim();
        invoice.GeneratedAt = DateTime.UtcNow;
        
        return invoice;
    }
}