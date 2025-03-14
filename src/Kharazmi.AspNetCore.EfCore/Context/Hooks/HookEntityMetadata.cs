﻿using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Kharazmi.AspNetCore.EFCore.Context.Hooks
{
    public class HookEntityMetadata
    {
        public HookEntityMetadata(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
    }
}