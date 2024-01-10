using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
    {
        _userManager = userManager;
        _uow = uow;
        _mapper = mapper;
        _photoService = photoService;
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUserWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username,[FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(',').ToArray();

        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if(!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if(!result.Succeeded) return BadRequest("Failed to remove roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-for-approval")]
    public async Task<ActionResult> GetPhotosForApproval()
    {
        var result = await _uow.PhotoRepository.GetUnapprovedPhotos();
        
        return Ok(result);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("approve-photo/{photoId}")]
    public async Task<IActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
        
        if (photo == null) return NotFound("There is no such photo with this id");
        
        if (photo.IsApproved) return BadRequest("The photo is already approved");

        photo.IsApproved = true;

        var user = await GetUserByPhotoIdAsync(photo.Id);

        if (user == null) return NotFound("There is no user for this photo");
        
        var mainPhoto = user.Photos.SingleOrDefault(p => p.IsMain == true);
        ;
        if ( mainPhoto == null) photo.IsMain = true;
        
        //if (await _uow.Complete()) return Ok("Successful approval"); //this is an alternative soulution
        if (await _uow.Complete()) return Ok( new
        {
            Id = photo.Id,
            Url = photo.Url,
            IsMain = photo.IsMain,
            PublicId = photo.PublicId,
            AppUserId = photo.AppUserId,
            IsApproved = photo.IsApproved,
        });
        //if (await _uow.Complete()) return Ok(_mapper.Map<Photo>(photo)); //this does not work for some reason, there is an object cycle

        return BadRequest("Failed to approve");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpDelete("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(this.User.GetUsername());

        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        _uow.PhotoRepository.RemovePhoto(photoId);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Problem deleting photo");
    }

    private async Task<AppUser> GetUserByPhotoIdAsync(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
        return await _uow.UserRepository.GetUserByIdAsync(photo.AppUserId);
    }
}
