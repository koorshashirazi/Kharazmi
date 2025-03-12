using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Application.Models;
using Kharazmi.AspNetCore.Core.Application.Services;
using Kharazmi.AspNetCore.Core.Authorization;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Mapping;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web.API
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCrudService"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TModel> : CrudControllerBase<TKey, TModel, TModel,
            FilteredPagedQueryModel>
        where TCrudService : class, ICrudService<TKey, TModel>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected override Task<IPagedQueryResult<TModel>> ReadPagedListAsync(FilteredPagedQueryModel query)
        {
            return Service.ReadPagedListAsync(query);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id)
        {
            return Service.FindAsync(id);
        }

        protected override Task<Result> EditAsync(TModel model)
        {
            return Service.EditAsync(model);
        }

        protected override Task<Result> CreateAsync(TModel model)
        {
            return Service.CreateAsync(model);
        }

        protected override Task<Result> DeleteAsync(TModel model)
        {
            return Service.DeleteAsync(model);
        }

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await Service.FindListAsync(ids).ConfigureAwait(false);
            return await Service.DeleteAsync(models).ConfigureAwait(false);
        }
    }

    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TReadModel, TModel> : CrudControllerBase<TKey, TReadModel, TModel,
            FilteredPagedQueryModel>
        where TCrudService : class, ICrudService<TKey, TReadModel, TModel>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(FilteredPagedQueryModel query)
        {
            return Service.ReadPagedListAsync(query);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id)
        {
            return Service.FindAsync(id);
        }

        protected override Task<Result> EditAsync(TModel model)
        {
            return Service.EditAsync(model);
        }

        protected override Task<Result> CreateAsync(TModel model)
        {
            return Service.CreateAsync(model);
        }

        protected override Task<Result> DeleteAsync(TModel model)
        {
            return Service.DeleteAsync(model);
        }

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await Service.FindListAsync(ids).ConfigureAwait(false);
            return await Service.DeleteAsync(models).ConfigureAwait(false);
        }
    }

    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TReadModel, TModel, TFilteredPagedQueryModel> :
            CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TCrudService : class, ICrudService<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel query)
        {
            return Service.ReadPagedListAsync(query);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id)
        {
            return Service.FindAsync(id);
        }

        protected override Task<Result> EditAsync(TModel model)
        {
            return Service.EditAsync(model);
        }

        protected override Task<Result> CreateAsync(TModel model)
        {
            return Service.CreateAsync(model);
        }

        protected override Task<Result> DeleteAsync(TModel model)
        {
            return Service.DeleteAsync(model);
        }

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await Service.FindListAsync(ids).ConfigureAwait(false);
            return await Service.DeleteAsync(models).ConfigureAwait(false);
        }
    }

    [ApiController]
    [Produces("application/json")]
    public abstract class
        CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedQueryModel> : ControllerBase
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel, new()
        where TKey : IEquatable<TKey>
    {
        private IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        protected abstract string CreatePermissionName { get; }
        protected abstract string EditPermissionName { get; }
        protected abstract string ViewPermissionName { get; }
        protected abstract string DeletePermissionName { get; }

        protected abstract Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel query);
        protected abstract Task<Maybe<TModel>> FindAsync(TKey id);
        protected abstract Task<Result> EditAsync(TModel model);
        protected abstract Task<Result> CreateAsync(TModel model);
        protected abstract Task<Result> DeleteAsync(TModel model);
        protected abstract Task<Result> DeleteAsync(IEnumerable<TKey> ids);

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Get(TFilteredPagedQueryModel query)
        {
            if (!await HasPermission(ViewPermissionName).ConfigureAwait(false)) return Forbid();

            var result = await ReadPagedListAsync(query ?? Factory<TFilteredPagedQueryModel>.CreateInstance()).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<TModel>> Get([BindRequired] TKey id)
        {
            if (!await HasPermission(EditPermissionName).ConfigureAwait(false)) return Forbid();

            var model = await FindAsync(id).ConfigureAwait(false);

            return model.HasValue ? (ActionResult) Ok(model.Value) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<ActionResult<TModel>> Post(TModel model)
        {
            if (!await HasPermission(CreatePermissionName).ConfigureAwait(false)) return Forbid();

            var result = await CreateAsync(model).ConfigureAwait(false);
            if (!result.Failed) return Created("", model);

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Put([BindRequired] TKey id, TModel model)
        {
            if (!model.Id.Equals(id)) return BadRequest();

            if (!await HasPermission(EditPermissionName).ConfigureAwait(false)) return Forbid();

            model.Id = id;

            var result = await EditAsync(model).ConfigureAwait(false);
            if (!result.Failed) return Ok(model);

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete([BindRequired] TKey id)
        {
            if (!await HasPermission(DeletePermissionName).ConfigureAwait(false)) return Forbid();

            var model = await FindAsync(id).ConfigureAwait(false);
            if (!model.HasValue) return NotFound();

            var result = await DeleteAsync(model.Value).ConfigureAwait(false);
            if (!result.Failed) return NoContent();

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpPost("[action]")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Delete(IEnumerable<TKey> ids)
        {
            if (!await HasPermission(DeletePermissionName).ConfigureAwait(false))
            {
                return Forbid();
            }

            var result = await DeleteAsync(ids).ConfigureAwait(false);

            if (!result.Failed)
            {
                return NoContent();
            }

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }


        private async Task<bool> HasPermission(string permissionName)
        {
            var policyName = PermissionConstant.PolicyPrefix + permissionName;
            return (await AuthorizationService.AuthorizeAsync(User, policyName).ConfigureAwait(false)).Succeeded;
        }
    }
}