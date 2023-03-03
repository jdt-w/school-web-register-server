namespace SchoolWebRegister.Services.Courses
{
    public interface IQuizService
    {
        Task<bool> IsQuizActive(int quizId);
        Task<uint> GetExpiredTime(int quizId);
        Task<ushort> GetQuestionsCount(int quizId);
    }
}
