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
            RenewalValidator validator = new RenewalValidator();
            validator.ValidateInput(customerId, planCode, seatCount, paymentMethod);
            
            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();
            
            CustomerRepository customerRepository = new CustomerRepository();
            SubscriptionPlanRepository planRepository = new SubscriptionPlanRepository();
            
            Customer customer = customerRepository.GetById(customerId);
            SubscriptionPlan plan = planRepository.GetByCode(normalizedPlanCode);
            
            validator.ValidateCustomer(customer);
            
            var baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;

            RenewalDiscountCalculator discountCalculator = new RenewalDiscountCalculator();
            DiscountResult discountResult = discountCalculator.Calculate(customer, plan, seatCount, baseAmount, useLoyaltyPoints);
            
            RenewalFeeCalculator feeCalculator = new RenewalFeeCalculator();
            FeeResult feeResult = feeCalculator.Calculate(
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
            
            RenewalInvoiceFactory invoiceFactory = new RenewalInvoiceFactory();
            RenewalInvoice invoice = invoiceFactory.Create(
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
            
            BillingService billingService = new BillingService();
            billingService.SaveInvoice(invoice);
            billingService.SendInvoiceEmail(customer, invoice, normalizedPlanCode);
                
            return invoice;
        }
    }
}
