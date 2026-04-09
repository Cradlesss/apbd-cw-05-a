using System;

namespace LegacyRenewalApp;

public class RenewalFeeCalculator
{
    public FeeResult Calculate(
        Customer customer,
        string normalizedPlanCode,
        string normalizedPaymentMethod,
        bool includePremiumSupport,
        decimal subtotalAfterDiscount)
    {
        var supportFee = 0m;
        var paymentFee = 0m;
        var taxRate = 0.20m;
        var notes = "";

        if (includePremiumSupport)
        {
            supportFee = normalizedPlanCode switch
            {
                "START" => 250m,
                "PRO" => 400m,
                "ENTERPRISE" => 700m,
                _ => supportFee
            };
            notes += "premium support included; ";
        }

        paymentFee = subtotalAfterDiscount + supportFee;
        switch (normalizedPaymentMethod)
        {
            case "CARD":
                paymentFee *= 0.02m;
                notes += "card payment fee; ";
                break;
            case "BANK_TRANSFER":
                paymentFee *= 0.01m;
                notes += "bank transfer fee; ";
                break;
            case "PAYPAL":
                paymentFee *= 0.035m;
                notes += "paypal payment fee; ";
                break;
            case "INVOICE":
                paymentFee = 0m;
                notes += "invoice payment; ";
                break;
            default:
                throw new ArgumentException("Unsupported payment method");
        }

        taxRate = customer.Country switch
        {
            "Poland" => 0.23m,
            "Germany" => 0.19m,
            "Czech Republic" => 0.21m,
            "Norway" => 0.25m,
            _ => taxRate
        };
        
        var res = new FeeResult();
        res.SupportFee = supportFee;
        res.PaymentFee = paymentFee;
        res.TaxRate = taxRate;
        res.Notes = notes;
        
        return res;
    }
}