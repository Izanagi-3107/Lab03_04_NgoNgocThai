/*
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 * Bài thực hành: Lab03-04 - Nhóm dự án và nhân sự
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lab03
{
    internal static class Program
    {
        private static int passed;

        private static int Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("vi-VN");

            try
            {
                Console.WriteLine("LAB03-04: NHÓM DỰ ÁN VÀ NHÂN SỰ\n");
                RunRequiredScenario();
                RunBoundaryCases();
                Console.WriteLine($"\nKẾT QUẢ: {passed}/{passed} kiểm tra đạt. Hoàn thành Lab03-04.");
                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"\nFAIL: {exception.Message}");
                return 1;
            }
        }

        private static void RunRequiredScenario()
        {
            // Bước 1: hai Employee dùng hai constructor khác nhau.
            using Employee employee1 = new Employee("E01", "Nguyễn An", 4_500_000);
            using Employee employee2 = new Employee("E02", "Trần Bình");
            Check(employee2.BaseSalary == 0, "Constructor hai tham số: lương mặc định bằng 0");
            employee2.IncreaseSalary(7_000_000, false);
            CheckClose(employee2.BaseSalary, 7_000_000, "Tăng tiền cố định với byPercentage = false");

            // Bước 2: hai SoftwareEngineer dùng hai constructor khác nhau.
            using SoftwareEngineer engineer1 = new SoftwareEngineer("S01", "Lê Chi", "C#");
            using SoftwareEngineer engineer2 = new SoftwareEngineer(
                "S02", "Phạm Dũng", 20_000_000, "Python", 3_000_000);
            Check(engineer1.BaseSalary == 0 && engineer1.TechnicalAllowance == 0,
                "Constructor kỹ sư ngắn: lương và phụ cấp mặc định bằng 0");
            engineer1.IncreaseSalary(15_000_000);

            // Bước 3 và 4: hai hình thức tăng lương.
            employee1.IncreaseSalary(500_000);
            CheckClose(employee1.BaseSalary, 5_000_000, "Tăng lương một số tiền cố định");
            employee2.IncreaseSalary(10, true);
            CheckClose(employee2.BaseSalary, 7_700_000, "Tăng lương 10 phần trăm");

            // Bước 5: nhóm ban đầu chưa có trưởng nhóm.
            using ProjectTeam team1 = new ProjectTeam("P01", "Hệ thống quản lý");
            Check(team1.Leader == null && team1.Members.Count == 0,
                "Nhóm mới chưa có trưởng nhóm và chưa có thành viên");

            // Bước 6, 7 và 8: thêm thường, thêm làm trưởng nhóm, thử thêm trùng.
            Check(team1.AddMember(employee1), "Thêm nhân sự vào nhóm");
            Check(team1.AddMember(engineer2, true) && ReferenceEquals(team1.Leader, engineer2),
                "Thêm kỹ sư và đặt làm trưởng nhóm");
            Check(!team1.AddMember(employee1) && team1.Members.Count == 2,
                "Từ chối thêm trùng nhân sự");

            // Bước 9 và 10: lời gọi đa hình và tổng chi phí, không đếm trưởng nhóm hai lần.
            team1.DisplayTeam();
            CheckClose(team1.CalculateTotalMonthlyCost(), 28_000_000,
                "Tổng chi phí bằng 28.000.000, đã tính phụ cấp kỹ sư");

            // Bước 11: không được xóa trưởng nhóm hiện tại.
            Check(!team1.RemoveMember(engineer2.Id) && team1.Contains(engineer2.Id),
                "Từ chối xóa trưởng nhóm hiện tại");

            // Bước 12: đổi trưởng nhóm, tự thêm người mới; người cũ vẫn là thành viên.
            Check(team1.ChangeLeader(engineer1) && team1.Contains(engineer1.Id)
                && ReferenceEquals(team1.Leader, engineer1),
                "Đổi trưởng nhóm và tự thêm trưởng nhóm mới");
            Check(team1.Contains(engineer2.Id), "Trưởng nhóm cũ vẫn còn trong nhóm");
            Check(team1.RemoveMember(engineer2.Id), "Xóa người từng là trưởng nhóm");
            CheckClose(team1.CalculateTotalMonthlyCost(), 20_000_000,
                "Tổng chi phí sau khi đổi và xóa bằng 20.000.000");

            // Bước 13 và 14: một đối tượng tham gia hai nhóm, kết thúc nhóm thứ hai.
            ProjectTeam team2;
            using (team2 = new ProjectTeam("P02", "Ứng dụng học tập", employee1))
            {
                Check(team1.Contains(employee1.Id) && team2.Contains(employee1.Id)
                    && ReferenceEquals(team1.Members[0], team2.Members[0]),
                    "Hai nhóm tham chiếu tới cùng một đối tượng nhân sự");
                Check(team2.Members.Count == 1 && ReferenceEquals(team2.Leader, employee1),
                    "Constructor có trưởng nhóm tự thêm đúng một thành viên");
                Check(team2.AddMember(employee2), "Thêm nhân sự thứ hai vào nhóm P02");
                team2.DisplayTeam();
            }

            // Bước 15: Dispose nhóm không kết thúc sử dụng các nhân sự.
            Check(team2.IsDisposed && team2.Members.Count == 0 && team2.Leader == null,
                "Kết thúc khối using: nhóm P02 đã dọn các tham chiếu nội bộ");
            Check(!employee1.IsDisposed && !employee2.IsDisposed && team1.Contains(employee1.Id),
                "Nhân sự vẫn tồn tại và vẫn sử dụng được ở nhóm P01");
            employee1.DisplayInfo();
            CheckClose(employee1.CalculateMonthlyCost(), 5_000_000,
                "Nhân sự vẫn tính được chi phí sau khi kết thúc nhóm P02");
        }

        private static void RunBoundaryCases()
        {
            Console.WriteLine("\nKIỂM TRA CÁC TRƯỜNG HỢP BIÊN\n");

            using Employee defaultEmployee = new Employee();
            Check(defaultEmployee.Id == "UNKNOWN"
                && defaultEmployee.FullName == "Unnamed employee"
                && defaultEmployee.BaseSalary == 0,
                "Constructor mặc định đúng cả ba giá trị");

            ExpectException<ArgumentException>(() => new Employee(" ", "An"), "Từ chối mã rỗng");
            ExpectException<ArgumentException>(() => new Employee("E10", " "), "Từ chối tên rỗng");
            ExpectException<ArgumentOutOfRangeException>(() => new Employee("E10", "An", -1),
                "Từ chối lương âm");
            ExpectException<ArgumentOutOfRangeException>(() => new Employee("E10", "An", double.NaN),
                "Từ chối lương NaN");
            ExpectException<ArgumentOutOfRangeException>(() => new Employee("E10", "An", double.PositiveInfinity),
                "Từ chối lương vô hạn");
            ExpectException<ArgumentException>(() => new SoftwareEngineer("S10", "Bình", " "),
                "Từ chối ngôn ngữ rỗng");
            ExpectException<ArgumentOutOfRangeException>(() => new SoftwareEngineer("S10", "Bình", 0, "C#", -1),
                "Từ chối phụ cấp âm");
            ExpectException<ArgumentOutOfRangeException>(() => defaultEmployee.IncreaseSalary(0),
                "Từ chối tăng lương bằng 0");
            ExpectException<ArgumentOutOfRangeException>(() => defaultEmployee.IncreaseSalary(-10, true),
                "Từ chối phần trăm âm");
            ExpectException<ArgumentOutOfRangeException>(() => defaultEmployee.IncreaseSalary(double.PositiveInfinity),
                "Từ chối mức tăng vô hạn");
            Check(defaultEmployee.BaseSalary == 0, "Dữ liệu không đổi sau các thao tác tăng lương sai");
            defaultEmployee.IncreaseSalary(10, true);
            Check(defaultEmployee.BaseSalary == 0, "Tăng 10 phần trăm của lương 0 vẫn bằng 0");

            using Employee employee = new Employee("T01", "Thành viên", 1_000);
            using Employee duplicate = new Employee("T01", "Đối tượng khác", 9_000);
            using Employee replacement = new Employee("T02", "Trưởng nhóm mới", 2_000);
            using ProjectTeam team = new ProjectTeam("T", "Nhóm kiểm tra");

            CheckClose(team.CalculateTotalMonthlyCost(), 0, "Nhóm rỗng có tổng chi phí bằng 0");
            Check(!team.Contains("MISSING") && !team.RemoveMember("MISSING"),
                "Tìm hoặc xóa mã không tồn tại trả về false");
            Check(team.AddMember(employee) && !team.AddMember(duplicate),
                "Hai đối tượng khác nhau nhưng cùng mã vẫn bị coi là trùng");
            Check(team.ChangeLeader(employee) && team.Members.Count == 1,
                "Chọn thành viên có sẵn làm trưởng nhóm không thêm bản sao");
            Check(!team.ChangeLeader(duplicate) && ReferenceEquals(team.Leader, employee),
                "Từ chối đổi trưởng nhóm sang đối tượng khác có mã xung đột");
            Check(team.AddMember(replacement, true) && ReferenceEquals(team.Leader, replacement)
                && team.Contains(employee.Id),
                "AddMember với trưởng nhóm mới giữ người cũ trong danh sách");
            Check(!team.AddMember(employee, true) && ReferenceEquals(team.Leader, replacement),
                "Thêm trùng với makeLeader = true không đổi trạng thái nhóm");
            Check(team.ChangeLeader(employee) && team.Members.Count == 2,
                "ChangeLeader chọn lại thành viên có sẵn");

            employee.IncreaseSalary(500);
            CheckClose(team.CalculateTotalMonthlyCost(), 3_500,
                "Nhóm đọc dữ liệu mới của đối tượng được tham chiếu");
            ExpectException<NotSupportedException>(
                () => ((ICollection<Employee>)team.Members).Clear(),
                "Danh sách chỉ đọc ngăn bên ngoài xóa thành viên trực tiếp");

            // null! chỉ dùng tại đây để cố ý kiểm tra đầu vào null.
            ExpectException<ArgumentNullException>(() => team.AddMember(null!), "Từ chối thành viên null");
            ExpectException<ArgumentException>(() => new ProjectTeam(" ", "Tên"), "Từ chối mã dự án rỗng");
            ExpectException<ArgumentException>(() => team.Contains(" "), "Từ chối mã tìm kiếm rỗng");

            team.Dispose();
            team.Dispose();
            Check(team.IsDisposed && !employee.IsDisposed && !replacement.IsDisposed,
                "Dispose nhóm nhiều lần an toàn và không Dispose nhân sự");
            ExpectException<ObjectDisposedException>(() => team.AddMember(employee),
                "Nhóm đã kết thúc sử dụng không nhận thêm thành viên");

            using Employee veryLarge = new Employee("BIG", "Lương lớn", double.MaxValue);
            ExpectException<OverflowException>(() => veryLarge.IncreaseSalary(double.MaxValue),
                "Phát hiện tràn số khi tăng lương");
            Check(veryLarge.BaseSalary == double.MaxValue, "Tràn số không làm hỏng lương đang lưu");

            using Employee disposedEmployee = new Employee("CLOSED", "Đã kết thúc");
            disposedEmployee.Dispose();
            using ProjectTeam activeTeam = new ProjectTeam("ACTIVE", "Nhóm đang hoạt động");
            ExpectException<ObjectDisposedException>(() => activeTeam.AddMember(disposedEmployee),
                "Không phân công nhân sự đã kết thúc sử dụng");
        }

        private static void Check(bool condition, string description)
        {
            if (!condition)
            {
                throw new InvalidOperationException(description);
            }

            passed++;
            Console.WriteLine($"PASS: {description}");
        }

        private static void CheckClose(double actual, double expected, string description)
        {
            // double có sai số biểu diễn, vì vậy so sánh với dung sai nhỏ.
            Check(double.IsFinite(actual) && Math.Abs(actual - expected) < 0.005, description);
        }

        private static void ExpectException<TException>(Action action, string description)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                Check(true, description);
                return;
            }

            throw new InvalidOperationException($"{description}: chưa phát sinh lỗi như mong đợi.");
        }
    }
}
