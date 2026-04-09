namespace LegacyRenewalApp;

public class BillingService
{
    public void SaveInvoice(RenewalInvoice invoice)
    {
        LegacyBillingGateway.SaveInvoice(invoice);
    }

    public void SendInvoiceEmail(Customer customer, RenewalInvoice invoice, string normalizedPlanCode)
    {
        if (string.IsNullOrWhiteSpace(customer.Email)) return;
        
        var subject = "Subscription renewal invoice";
        var body =
            $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
            $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

        LegacyBillingGateway.SendEmail(customer.Email, subject, body);
    }
}