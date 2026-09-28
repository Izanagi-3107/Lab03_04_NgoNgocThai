/*
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 * Bài thực hành: Lab03-04 - Nhóm dự án và nhân sự
 */
using System;
using System.Collections.Generic;

namespace Lab03
{
    // Kết tập: nhóm giữ các tham chiếu, không sở hữu vòng đời của nhân sự.
    public sealed class ProjectTeam : IDisposable
    {
        private readonly List<Employee> members = new List<Employee>();

        public string ProjectCode { get; }
        public string ProjectName { get; }
        public Employee? Leader { get; private set; }
        public bool IsDisposed { get; private set; }

        // Bọc danh sách để bên ngoài không thêm/xóa và bỏ qua quy tắc của nhóm.
        public IReadOnlyList<Employee> Members => members.AsReadOnly();

        public ProjectTeam(string projectCode, string projectName)
        {
            ProjectCode = RequireText(projectCode, nameof(projectCode));
            ProjectName = RequireText(projectName, nameof(projectName));
        }

        public ProjectTeam(string projectCode, string projectName, Employee leader)
            : this(projectCode, projectName)
        {
            AddMember(leader, true);
        }

        public bool AddMember(Employee employee)
        {
            return AddMember(employee, false);
        }

        public bool AddMember(Employee employee, bool makeLeader)
        {
            ThrowIfDisposed();
            ValidateEmployee(employee);

            // Thêm trùng bị từ chối hoàn toàn, không đổi trưởng nhóm.
            // Để chọn một thành viên đã có làm trưởng nhóm, dùng ChangeLeader.
            if (FindMember(employee.Id) != null)
            {
                return false;
            }

            members.Add(employee);
            if (makeLeader)
            {
                Leader = employee;
            }

            return true;
        }

        public bool Contains(string employeeId)
        {
            ThrowIfDisposed();
            string id = RequireText(employeeId, nameof(employeeId));
            return FindMember(id) != null;
        }

        public bool RemoveMember(string employeeId)
        {
            ThrowIfDisposed();
            string id = RequireText(employeeId, nameof(employeeId));

            if (Leader != null && Leader.Id == id)
            {
                return false;
            }

            Employee? employee = FindMember(id);
            return employee != null && members.Remove(employee);
        }

        public bool ChangeLeader(Employee employee)
        {
            ThrowIfDisposed();
            ValidateEmployee(employee);

            Employee? existing = FindMember(employee.Id);
            if (existing != null && !ReferenceEquals(existing, employee))
            {
                // Hai đối tượng khác nhau nhưng trùng mã là dữ liệu xung đột.
                return false;
            }

            if (existing == null)
            {
                members.Add(employee);
            }

            Leader = employee;
            return true;
        }

        public double CalculateTotalMonthlyCost()
        {
            ThrowIfDisposed();
            double total = 0;

            foreach (Employee employee in members)
            {
                // Gọi đa hình: kỹ sư tự tính cả phụ cấp.
                total += employee.CalculateMonthlyCost();
                if (!double.IsFinite(total))
                {
                    throw new OverflowException("Tổng chi phí vượt phạm vi biểu diễn của double.");
                }
            }

            return total;
        }

        public void DisplayTeam()
        {
            ThrowIfDisposed();
            Console.WriteLine($"\nDự án {ProjectCode}: {ProjectName}");
            Console.WriteLine($"Trưởng nhóm: {Leader?.FullName ?? "Chưa có"}");

            foreach (Employee employee in members)
            {
                string role = ReferenceEquals(employee, Leader) ? "Trưởng nhóm" : "Thành viên";
                Console.Write($"[{role}] ");
                employee.DisplayInfo();
            }

            Console.WriteLine($"Tổng chi phí: {CalculateTotalMonthlyCost():N0} VNĐ/tháng\n");
        }

        private Employee? FindMember(string employeeId)
        {
            foreach (Employee employee in members)
            {
                if (string.Equals(employee.Id, employeeId, StringComparison.Ordinal))
                {
                    return employee;
                }
            }

            return null;
        }

        private static void ValidateEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            if (employee.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(employee));
            }
        }

        private static string RequireText(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Thông tin không được để trống.", parameterName);
            }

            return value.Trim();
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(ProjectTeam));
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            // Chỉ bỏ tham chiếu nội bộ. Không gọi Dispose trên bất kỳ Employee nào.
            Leader = null;
            members.Clear();
            IsDisposed = true;
            Console.WriteLine($"[Kết thúc sử dụng] ProjectTeam {ProjectCode}; nhân sự vẫn độc lập.");
        }
    }
}
