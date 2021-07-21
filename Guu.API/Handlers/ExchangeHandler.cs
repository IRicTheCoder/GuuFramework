using System.Collections.Generic;
using Guu.Game;

namespace Guu.API
{
    /// <summary>
    /// Serves as a handler for exchanges after being registered, mostly to sort them
    /// and identify them. Make a child of this class to create your own handlers
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ExchangeHandler : IHandler<ExchangeHandler>
    {
        // TODO: Finish this
        
        //+ HANDLING
        /// <inheritdoc />
        public virtual ExchangeHandler Setup()
        {
            APIHandler.HandleRegistration += RegistryHandle;
            APIHandler.HandleItems += Handle;
            APIHandler.ClearMemory += ClearMemory;
            
            return this;
        }
        
        /// <inheritdoc />
        public virtual void RegistryHandle() { }

        /// <inheritdoc />
        public virtual void Handle() { }

        /// <inheritdoc />
        public virtual void ClearMemory() { }
        
        //+ LOCK CHECK
        

        //+ VERIFICATION
        
    }
}