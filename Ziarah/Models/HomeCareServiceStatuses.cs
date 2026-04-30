namespace Ziarah.Models;

/// <summary>
/// قيم حقل Status في جدول HomeCareServices لطلبات الزيارة المنزلية.
/// </summary>
public static class HomeCareServiceStatuses
{
    /// <summary>قيد الانتظار — الطلب مسجل ولم يُعالج بعد.</summary>
    public const int Pending = 1;

    // للتوسع لاحقاً على المنصة، مثال:
    // public const int Confirmed = 2;
    // public const int InProgress = 3;
    // public const int Completed = 4;
    // public const int Cancelled = 5;
}
