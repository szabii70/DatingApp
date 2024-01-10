using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhotoById(int id);
        Task<IEnumerable<PhotoDto>> GetUnapprovedPhotos();
        void RemovePhoto(int id);
    }
}