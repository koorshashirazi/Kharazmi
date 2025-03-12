using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Kharazmi.AspNetCore.Core.Cryptography;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Kharazmi.AspNetCore.Security.Cryptography
{
    internal class XmlRepository : IXmlRepository
    {
        private readonly IProtectionStore _repository;

        public XmlRepository(IProtectionStore repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return _repository.ReadElements();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            _repository.StoreElement(element, friendlyName);
        }
    }
}