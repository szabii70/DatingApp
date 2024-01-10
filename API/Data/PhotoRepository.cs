using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public PhotoRepository(DataContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }
        public async Task<IEnumerable<PhotoDto>> GetUnapprovedPhotos()
        {
            var result = await _context.Photos
                .IgnoreQueryFilters()
                .Where(x => x.IsApproved == false)
                .ProjectTo<PhotoDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            
            return result;
        }

        public async Task<Photo> GetPhotoById(int id)
        {
            var result = await _context.Photos
                .IgnoreQueryFilters()
                .Where(x => x.Id == id)
                .Include(x => x.AppUser)
                .SingleOrDefaultAsync();

            return result;
        }

        public async void RemovePhoto(int id)
        {
            var photo = await GetPhotoById(id);

            if (photo == null) throw new Exception("Photo not found");

            _context.Photos.Remove(photo);
        }
    }
}
