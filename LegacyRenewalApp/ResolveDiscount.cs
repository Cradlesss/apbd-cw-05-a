namespace LegacyRenewalApp;

public class ResolveDiscount(decimal discountAmount, string notes)
{
    public static ResolveDiscount Resolve(decimal baseAmount, 
                                  Customer customer,
                                  int seatCount,
                                  bool isEducationEligible,
                                  bool usesLoyaltyPts
    )
    {
        var segment = customer.Segment.ToLower().Trim();
        var discountAmount = 0m;
        var notes = string.Empty;
        
        switch (segment)
        {
            case "silver":
                discountAmount += baseAmount * 0.05m;
                notes += "silver discount; ";
                break;
            case "gold":
                discountAmount += baseAmount * 0.10m;
                notes += "gold discount; ";
                break;
            case "platinum":
                discountAmount += baseAmount * 0.15m;
                notes += "platinum discount; ";
                break;
            case "education" when isEducationEligible:
                discountAmount += baseAmount * 0.20m;
                notes += "education discount; ";
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

        if (usesLoyaltyPts && customer.LoyaltyPoints > 0)
        {
            var ptsToUse =  customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            discountAmount += ptsToUse;
            notes += $"loyalty points used: {ptsToUse}; ";
        }

        return new ResolveDiscount(discountAmount, notes);
    }

    public string GetNotes()
    {
        return notes;
    }

    public decimal GetDiscountAmount()
    {
        return discountAmount;
    }
}