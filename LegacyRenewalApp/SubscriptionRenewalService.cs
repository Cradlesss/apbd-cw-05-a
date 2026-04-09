using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            var validator = new RenewalValidator();
            validator.ValidateInput(customerId, planCode, seatCount, paymentMethod);
            
            var normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            var normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();
            
            var customerRepository = new CustomerRepository();
            var planRepository = new SubscriptionPlanRepository();
            
            var customer = customerRepository.GetById(customerId);
            var plan = planRepository.GetByCode(normalizedPlanCode);
            
            validator.ValidateCustomer(customer);
            
            var baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;

            var discountCalculator = new RenewalDiscountCalculator();
            var discountResult = discountCalculator.Calculate(customer, plan, seatCount, baseAmount, useLoyaltyPoints);
            
            var feeCalculator = new RenewalFeeCalculator();
            var feeResult = feeCalculator.Calculate(
                customer,
                normalizedPlanCode,
                normalizedPaymentMethod,
                includePremiumSupport,
                discountResult.SubtotalAfterDiscount);
            
            var taxBase = discountResult.SubtotalAfterDiscount + feeResult.SupportFee + feeResult.PaymentFee;
            var taxAmount = taxBase * feeResult.TaxRate;
            var finalAmount = taxBase + taxAmount;
            
            var notes = discountResult.Notes + feeResult.Notes;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }
            
            var invoiceFactory = new RenewalInvoiceFactory();
            var invoice = invoiceFactory.Create(
                customer,
                normalizedPlanCode,
                normalizedPaymentMethod,
                seatCount,
                baseAmount,
                discountResult.DiscountAmount,
                feeResult.SupportFee,
                feeResult.PaymentFee,
                taxAmount,
                finalAmount,
                notes);
            
            var billingService = new BillingService();
            billingService.SaveInvoice(invoice);
            billingService.SendInvoiceEmail(customer, invoice, normalizedPlanCode);
                
            return invoice;
        }
    }
}
