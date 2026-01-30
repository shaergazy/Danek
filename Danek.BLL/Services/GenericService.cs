using AutoMapper;
using Danek.BLL.Services.Contracts;
using Danek.DAL.Repositories.Contracts;
using Microsoft.Extensions.Logging;

namespace Danek.BLL.Services
{
    public class GenericService<TListDto, TAddDto, TEditDto, TGetDto, TEntity, TKey> : IGenericService<TListDto, TAddDto, TEditDto, TGetDto, TEntity, TKey>
    where TAddDto : class
    where TEditDto : class
    where TListDto : class
    where TGetDto : class
        where TEntity : class
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<TEntity, TKey> _unitOfWork;
        private readonly ILogger<GenericService<TListDto, TAddDto, TEditDto, TGetDto, TEntity, TKey>> _logger;

        public GenericService(IMapper mapper, IUnitOfWork<TEntity, TKey> unitOfWork, ILogger<GenericService<TListDto, TAddDto, TEditDto, TGetDto, TEntity, TKey>> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public virtual IEnumerable<TListDto> GetAll()
        {
            var entities = _unitOfWork.Repository.GetAll();
            _logger.LogInformation("Fetched all entities.");
            return _mapper.Map<IEnumerable<TListDto>>(entities);
        }

        public virtual async Task<TGetDto> GetByIdAsync(TKey id)
        {
            var entity = await _unitOfWork.Repository.GetByIdAsync(id);
            _logger.LogInformation("Fetched entity with ID: {Id}", id);
            return _mapper.Map<TGetDto>(entity);
        }

        public async Task<TEntity> CreateAsync(TAddDto dto)
        {
            var entity = await BuildEntityForCreate(dto);
            await _unitOfWork.Repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Created entity with ID: {Id}", entity.GetType().GetProperty("Id").GetValue(entity, null));
            return entity;
        }

        public async Task UpdateAsync(TEditDto dto)
        {
            var entity = await BuildEntityForUpdate(dto);
            await _unitOfWork.Repository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Updated entity with ID: {Id}", entity.GetType().GetProperty("Id").GetValue(entity, null));
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            var entity = await BuildEntityForDelete(id);
            await _unitOfWork.Repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Deleted entity with ID: {Id}", id);
        }

        public virtual async Task DeleteAsync(TEntity obj)
        {
            await _unitOfWork.Repository.Remove(obj);
            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task<TEntity> BuildEntityForDelete(TKey id)
        {
            var entity = await _unitOfWork.Repository.GetByIdAsync(id);
            return entity;
        }
        public virtual async Task<TEntity> BuildEntityForCreate(TAddDto dto)
        {
            return _mapper.Map<TEntity>(dto);
        }

        public virtual async Task<TEntity> BuildEntityForUpdate(TEditDto dto)
        {
            return _mapper.Map<TEntity>(dto);
        }
    }
}
