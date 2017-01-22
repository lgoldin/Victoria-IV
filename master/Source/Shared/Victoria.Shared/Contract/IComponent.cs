using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared.Contract
{
    public interface IComponent
    {
        /// <summary>
        /// Initialize component
        /// </summary>
        void Initialize();

        /// <summary>
        /// Uninitialize Component
        /// </summary>
        void UnInitialize();
    }
}
