﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionOptions
    {
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public List<ExceptionMapItem> Mappings { get; } = new List<ExceptionMapItem>();
        [Required] public string DbException { get; set; }

        [Required] public string DbConcurrencyException { get; set; }

        [Required] public string InternalServerIssue { get; set; }

        public bool TryFindMapping(DbException dbException, out ExceptionMapItem mapping)
        {
            mapping = null;

            var words = new HashSet<string>(Regex.Split(dbException.ToStringFormat(), @"\W"));

            var mappingItem = Mappings.FirstOrDefault(a => a.Keywords.IsProperSubsetOf(words));
            if (mappingItem == null)
            {
                return false;
            }

            mapping = mappingItem;

            return true;
        }
    }
}