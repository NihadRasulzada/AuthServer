using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    /// <summary>
    /// Provides generic CRUD operations for entities and DTOs.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TDto">The type of the Data Transfer Object (DTO).</typeparam>
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericService{TEntity, TDto}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="genericRepository">The generic repository.</param>
        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        #region AddAsync
        /// <summary>
        /// Adds a new entity asynchronously.
        /// </summary>
        /// <param name="entity">The DTO representing the entity to add.</param>
        /// <returns>A response containing the added DTO and status code.</returns>
        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            try
            {
                var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
                await _genericRepository.AddAsync(newEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
                return Response<TDto>.Success(newDto, 200);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                return Response<TDto>.Fail("An error occurred while adding the entity.", 500, true);
            }
        }
        #endregion

        #region GetAllAsync
        /// <summary>
        /// Retrieves all entities asynchronously.
        /// </summary>
        /// <returns>A response containing the list of DTOs and status code.</returns>
        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            try
            {
                var entities = await _genericRepository.GetAllAsync().ConfigureAwait(false);
                var dtoList = ObjectMapper.Mapper.Map<List<TDto>>(entities);
                return Response<IEnumerable<TDto>>.Success(dtoList, 200);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                return Response<IEnumerable<TDto>>.Fail("An error occurred while retrieving entities.", 500, true);
            }
        }
        #endregion

        #region GetByIdAsync
        /// <summary>
        /// Retrieves an entity by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>A response containing the DTO and status code.</returns>
        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _genericRepository.GetByIdAsync(id).ConfigureAwait(false);
                if (entity == null)
                {
                    return Response<TDto>.Fail("Id not found", 404, true);
                }
                var dto = ObjectMapper.Mapper.Map<TDto>(entity);
                return Response<TDto>.Success(dto, 200);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                return Response<TDto>.Fail("An error occurred while retrieving the entity.", 500, true);
            }
        }
        #endregion

        #region Remove
        /// <summary>
        /// Removes an entity by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>A response indicating success or failure.</returns>
        public async Task<Response<NoDataDto>> Remove(int id)
        {
            try
            {
                var entity = await _genericRepository.GetByIdAsync(id).ConfigureAwait(false);
                if (entity == null)
                {
                    return Response<NoDataDto>.Fail("Id not found", 404, true);
                }
                _genericRepository.Remove(entity);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                return Response<NoDataDto>.Success(204);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                return Response<NoDataDto>.Fail("An error occurred while removing the entity.", 500, true);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates an existing entity asynchronously.
        /// </summary>
        /// <param name="entity">The DTO representing the updated entity.</param>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <returns>A response indicating success or failure.</returns>
        public async Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            try
            {
                var existingEntity = await _genericRepository.GetByIdAsync(id).ConfigureAwait(false);
                if (existingEntity == null)
                {
                    return Response<NoDataDto>.Fail("Id not found", 404, true);
                }
                var updatedEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
                _genericRepository.Update(updatedEntity);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                return Response<NoDataDto>.Success(204);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                return Response<NoDataDto>.Fail("An error occurred while updating the entity.", 500, true);
            }
        }
        #endregion

        #region Where
        /// <summary>
        /// Retrieves entities that match the specified predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The predicate to filter the entities.</param>
        /// <returns>A response containing the list of DTOs and status code.</returns>
        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var entities = _genericRepository.Where(predicate);
                var entityList = await entities.ToListAsync().ConfigureAwait(false);
                var dtoList = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(entityList);
                return Response<IEnumerable<TDto>>.Success(dtoList, 200);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                return Response<IEnumerable<TDto>>.Fail("An error occurred while retrieving entities.", 500, true);
            }
        }
        #endregion
    }
}
