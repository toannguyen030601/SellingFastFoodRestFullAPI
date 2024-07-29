namespace ASM_api.Models
{
    public class Feedback
    {
        public int FeedbackID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public int CustomerID { get; set; } // Khóa ngoại đến Customer (nếu cần)
        public Customer? Customer { get; set; } // Tham chiếu đến Customer (nếu cần)
    }
}
