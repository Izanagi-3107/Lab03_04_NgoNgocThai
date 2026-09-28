/*
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 * Bài thực hành: Lab03-04 - Nhóm dự án và nhân sự
 */
using System;

namespace Lab03
{
    // Nhân sự tự bảo vệ các ràng buộc của dữ liệu mà mình quản lý.
    public class Employee : IDisposable
    {
        public string Id { get; }
        public string FullName { get; }
        public double BaseSalary { get; private set; }
        public bool IsDisposed { get; private set; }

        // Constructor chaining: cả ba constructor dùng chung phần kiểm tra dữ liệu.
        public Employee() : this("UNKNOWN", "Unnamed employee", 0)
        {
        }

        public Employee(string id, string fullName) : this(id, fullName, 0)
        {
        }

        public Employee(string id, string fullName, double baseSalary)
        {
            Id = RequireText(id, nameof(id));
            FullName = RequireText(fullName, nameof(fullName));
            RequireNonNegative(baseSalary, nameof(baseSalary));
            BaseSalary = baseSalary;
        }

        // Overloading: cùng tên, khác danh sách tham số.
        public void IncreaseSalary(double amount)
        {
            IncreaseSalary(amount, false);
        }

        public void IncreaseSalary(double value, bool byPercentage)
        {
            ThrowIfDisposed();

            if (!double.IsFinite(value) || value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value), "Giá trị tăng phải là số hữu hạn và lớn hơn 0.");
            }

            double increase = byPercentage ? BaseSalary * (value / 100.0) : value;
            double newSalary = BaseSalary + increase;

            if (!double.IsFinite(newSalary))
            {
                throw new OverflowException("Lương mới vượt phạm vi biểu diễn của double.");
            }

            // Chỉ cập nhật sau khi mọi kiểm tra đều thành công.
            BaseSalary = newSalary;
        }

        public virtual double CalculateMonthlyCost()
        {
            ThrowIfDisposed();
            return BaseSalary;
        }

        public virtual void DisplayInfo()
        {
            ThrowIfDisposed();
            Console.WriteLine(
                $"Nhân sự {Id} | {FullName} | Lương: {BaseSalary:N0} | " +
                $"Chi phí: {CalculateMonthlyCost():N0}");
        }

        protected static string RequireText(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Thông tin không được để trống.", parameterName);
            }

            return value.Trim();
        }

        protected static void RequireNonNegative(double value, string parameterName)
        {
            if (!double.IsFinite(value) || value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName, "Giá trị phải là số hữu hạn và không âm.");
            }
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        // Dùng cho minh họa vòng đời trong bản chuyển sang C#.
        // Dispose là kết thúc sử dụng có chủ đích, KHÔNG phải giải phóng bộ nhớ ngay.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            if (disposing)
            {
                Console.WriteLine($"[Kết thúc sử dụng] Employee {Id}");
            }
        }
    }
}
