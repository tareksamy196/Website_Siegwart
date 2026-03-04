using AutoMapper;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Dtos.Admin.TeamMember;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Service for managing team members
    /// </summary>
    public class TeamMemberService : ITeamMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<TeamMemberService> _logger;

        public TeamMemberService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService,
            ILogger<TeamMemberService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _logger = logger;
        }

        public async Task<int> CreateAsync(CreateTeamMemberDto input)
        {
            _logger.LogInformation("Creating team member: {NameEn}", input.NameEn);

            try
            {
                string? savedImagePath = null;
                if (input.ImageFile != null && input.ImageFile.Length > 0)
                    savedImagePath = await _attachmentService.UploadAsync(input.ImageFile, "uploads/team");

                var entity = _mapper.Map<TeamMember>(input);
                entity.ImageUrl = savedImagePath;

                await _unitOfWork.TeamMemberRepository.AddAsync(entity);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Team member created successfully: {Id} - {NameEn}", entity.Id, entity.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team member: {NameEn}", input.NameEn);
                throw;
            }
        }

        public async Task<TeamMember?> GetEntityByIdAsync(int id)
        {
            _logger.LogDebug("Getting team member entity by ID: {Id}", id);

            try
            {
                var entity = await _unitOfWork.TeamMemberRepository.GetByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                {
                    _logger.LogDebug("Team member not found: {Id}", id);
                    return null;
                }

                _logger.LogDebug("Team member entity retrieved: {Id} - {NameEn}", id, entity.NameEn);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team member entity: {Id}", id);
                throw;
            }
        }

        public async Task<int> UpdateAsync(UpdateTeamMemberDto input)
        {
            _logger.LogInformation("Updating team member: {Id} - {NameEn}", input.Id, input.NameEn);

            try
            {
                var entity = await _unitOfWork.TeamMemberRepository.GetByIdAsync(input.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Team member not found: {Id}", input.Id);
                    throw new KeyNotFoundException($"Team member with ID {input.Id} not found.");
                }

                _mapper.Map(input, entity);

                if (input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(entity.ImageUrl))
                        _attachmentService.Delete(entity.ImageUrl);

                    string? savedImagePath = await _attachmentService.UploadAsync(input.ImageFile, "uploads/team");
                    entity.ImageUrl = savedImagePath;
                }

                await _unitOfWork.TeamMemberRepository.UpdateAsync(entity);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Team member updated successfully: {Id} - {NameEn}", entity.Id, entity.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team member: {Id}", input.Id);
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting team member: {Id}", id);

            try
            {
                var entity = await _unitOfWork.TeamMemberRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Team member not found: {Id}", id);
                    throw new KeyNotFoundException($"Team member with ID {id} not found.");
                }

                if (!string.IsNullOrEmpty(entity.ImageUrl))
                    _attachmentService.Delete(entity.ImageUrl);

                await _unitOfWork.TeamMemberRepository.RemoveAsync(entity);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Team member deleted successfully: {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team member: {Id}", id);
                throw;
            }
        }

        public async Task<List<AdminTeamMemberDto>> GetAllAsync()
        {
            _logger.LogDebug("Getting all team members");

            try
            {
                var items = (await _unitOfWork.TeamMemberRepository.GetAllAsync())
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Order)
                    .ToList();

                var result = _mapper.Map<List<AdminTeamMemberDto>>(items);

                _logger.LogDebug("Retrieved {Count} team members", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all team members");
                throw;
            }
        }

        public async Task<UpdateTeamMemberDto?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting team member for edit: {Id}", id);

            try
            {
                var entity = await _unitOfWork.TeamMemberRepository.GetByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                {
                    _logger.LogDebug("Team member not found: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<UpdateTeamMemberDto>(entity);

                _logger.LogDebug("Team member retrieved for edit: {Id} - {NameEn}", id, result.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team member by ID: {Id}", id);
                throw;
            }
        }
    }
}