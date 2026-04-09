namespace LegacyRenewalApp;

public class RenewalDiscountCalculator
{
    public DiscountResult Calculate(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount,
        bool useLoyaltyPoints)
    {
        var discountAmount = 0m;
        var notes = "";

        switch (customer.Segment)
        {
            case "Silver":
                discountAmount += baseAmount * 0.05m;
                notes += "silver discount; ";
                break;
            case "Gold":
                discountAmount += baseAmount * 0.10m;
                notes += "gold discount; ";
                break;
            case "Platinum":
                discountAmount += baseAmount * 0.15m;
                notes += "platinum discount; ";
                break;
            case "Education":
                if (plan.IsEducationEligible)
                {
                    discountAmount += baseAmount * 0.20m;
                    notes += "education discount; ";
                }
                break;
        }

        switch (customer.YearsWithCompany)
        {
            case >= 5:
                discountAmount += baseAmount * 0.07m;
                notes += "long-term loyalty discount; ";
                break;
            case >= 2:
                discountAmount += baseAmount * 0.03m;
                notes += "basic loyalty discount; ";
                break;
        }
        
        switch (seatCount)
        {
            case >= 50:
                discountAmount += baseAmount * 0.12m;
                notes += "large team discount; ";
                break;
            case >= 20:
                discountAmount += baseAmount * 0.08m;
                notes += "medium team discount; ";
                break;
            case >= 10:
                discountAmount += baseAmount * 0.04m;
                notes += "small team discount; ";
                break;
        }

        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            var ptsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            discountAmount += ptsToUse;
            notes += $"loyalty points used: {ptsToUse}; ";
        }
        
        var subtotalAfterDsc = baseAmount - discountAmount;

        if (subtotalAfterDsc < 300m)
        {
            subtotalAfterDsc = 300m;
            notes += "minimum discounted subtotal applied; ";
        }

        var res = new DiscountResult();
        res.DiscountAmount = discountAmount;
        res.SubtotalAfterDiscount = subtotalAfterDsc;
        res.Notes = notes;
        
        return res;
    }
}