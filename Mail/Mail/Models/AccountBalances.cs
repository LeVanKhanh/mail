namespace Mail.Models
{
    public class AccountBalance
    {
        public string Branch { get; set; } // Chi nhánh                             0
        public string AccountId { get; set; } // Tài khoản                          1
        public string OldAccountId { get; set; } // Tài khoản cũ                    2
        public string AccountName1 { get; set; } // Tên tài khoản 1                 3
        public string CustomerCode { get; set; } // Mã khách hàng                   4
        public string ShortName { get; set; } // Tên gợi nhớ                        5
        public string CustomerName1 { get; set; } // Tên khách hàng 1               6
        public string CustomerName2 { get; set; } // Tên khách hàng 2               7
        public string ProductCode { get; set; } // Mã Sản phẩm                      8
        public string ProductName { get; set; } // Tên Sản phẩm                     9
        public string Currency { get; set; } // Tiền tệ                             10
        public string AvailableBalance { get; set; } // Số dư khả dụng              11
        public string ActualBalance { get; set; } // Số dư thực tế                  12
        public string CustomerOfficer { get; set; } // Cán bộ quản lý khách hàng    13
        public string Email { get; set; }
    }
}
