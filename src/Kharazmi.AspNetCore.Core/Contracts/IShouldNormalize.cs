﻿
namespace Kharazmi.AspNetCore.Core.Contracts
{
    public interface IShouldNormalize
    {
        /// <summary>
        /// This method is called before method execution (after validation if exists).
        /// </summary>
        void Normalize();
    }
}
