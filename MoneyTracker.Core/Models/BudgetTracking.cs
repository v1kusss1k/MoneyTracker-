// MoneyTracker.Core/Models/BudgetTracking.cs
namespace MoneyTracker.Core.Models
{
    public class BudgetTracking
    {
        public Budget Budget { get; set; }
        public decimal Spent { get; set; } // Потрачено в этом месяце
        public decimal Remaining => Budget.MonthlyLimit - Spent;
        public decimal Percentage => Budget.MonthlyLimit > 0 ?
            (Spent / Budget.MonthlyLimit) * 100 : 0;
        public bool IsExceeded => Spent > Budget.MonthlyLimit;
        public bool IsWarning => Percentage >= 80 && !IsExceeded;

        public string Status
        {
            get
            {
                if (IsExceeded) return "Превышен";
                if (IsWarning) return "Близко к лимиту";
                return "В норме";
            }
        }

        public string ProgressText => $"{Spent:N0}₽ / {Budget.MonthlyLimit:N0}₽";
    }
}