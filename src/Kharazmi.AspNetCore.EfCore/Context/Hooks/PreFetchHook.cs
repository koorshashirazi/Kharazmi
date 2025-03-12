using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EFCore.Context.Hooks
{
    public abstract class PreFetchHook<TEntity> : PreActionHook<TEntity>
    {
        public override EntityState HookState => EntityState.Unchanged;
    }
}