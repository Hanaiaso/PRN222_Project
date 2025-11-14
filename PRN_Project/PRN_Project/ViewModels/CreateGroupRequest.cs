namespace PRN_Project.ViewModels
{
    public class CreateGroupRequest
    {
        // Tên thuộc tính phải khớp với tên bạn gửi trong JSON body từ JavaScript
        public string GroupName { get; set; }
        public List<string> MemberEmails { get; set; }
    }
}
