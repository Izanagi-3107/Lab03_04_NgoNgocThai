/*
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 * Bài thực hành: Lab03-04 - Nhóm dự án và nhân sự
 */
using System;

namespace Lab03
{
    // Kỹ sư phần mềm là một loại nhân sự: quan hệ is-a.
    public class SoftwareEngineer : Employee
    {
        public string PrimaryLanguage { get; }
        public double TechnicalAllowance { get; }

        public SoftwareEngineer(string id, string fullName, string primaryLanguage)
            : this(id, fullName, 0, primaryLanguage, 0)
        {
        }

        public SoftwareEngineer(
            string id,
            string fullName,
            double baseSalary,
            string primaryLanguage,
            double technicalAllowance)
            : base(id, fullName, baseSalary)
        {
            PrimaryLanguage = RequireText(primaryLanguage, nameof(primaryLanguage));
            RequireNonNegative(technicalAllowance, nameof(technicalAllowance));
            TechnicalAllowance = technicalAllowance;
        }

        // Overriding: lớp con thay đổi cách tính chi phí của lớp cha.
        public override double CalculateMonthlyCost()
        {
            double cost = base.CalculateMonthlyCost() + TechnicalAllowance;
            if (!double.IsFinite(cost))
            {
                throw new OverflowException("Chi phí kỹ sư vượt phạm vi biểu diễn của double.");
            }

            return cost;
        }

        public override void DisplayInfo()
        {
            ThrowIfDisposed();
            Console.WriteLine(
                $"Kỹ sư {Id} | {FullName} | {PrimaryLanguage} | " +
                $"Lương: {BaseSalary:N0} | Phụ cấp: {TechnicalAllowance:N0} | " +
                $"Chi phí: {CalculateMonthlyCost():N0}");
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                Console.WriteLine($"[Kết thúc sử dụng] SoftwareEngineer {Id}");
            }

            base.Dispose(disposing);
        }
    }
}
