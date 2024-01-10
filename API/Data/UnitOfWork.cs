using API.Data;
using API.Interfaces;
using AutoMapper;

namespace API;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UnitOfWork(DataContext context, IMapper mapper, IPhotoService photoService)
    {
        _context = context;
        _mapper = mapper;
        _photoService = photoService;
    }
    public IUserRepository UserRepository => new UserRepository(_context, _mapper);

    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

    public ILikeRepository LikesRepository => new LikesRepository(_context);

    public IPhotoRepository PhotoRepository => new PhotoRepository(_context, _mapper, _photoService);

    public async Task<bool> Complete()
    {
        var result = await _context.SaveChangesAsync() > 0;
        ;
        return result;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
