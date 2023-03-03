namespace SchoolWebRegister.DAL.Repositories
{
    public interface IImageRepository
    {
        Task<byte[]> LoadImage(long imageId);
    }
}
