using System.Collections.Generic;

namespace Guu.API
{
    /// <summary>
    /// Serves as a handler for food after being registered, mostly to sort them
    /// and identify them. Make a child of this class to create your own handlers
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    [System.Obsolete("NOT IMPLEMENTED YET")]
    public class FoodHandler : IHandler<FoodHandler>
    {
        //+ GROUP LISTING
        internal static readonly HashSet<Identifiable.Id> NONTARRGOLD_SLIMES_GROUP = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        internal static readonly HashSet<Identifiable.Id> MEAT_GROUP = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        internal static readonly HashSet<Identifiable.Id> FRUIT_GROUP = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        internal static readonly HashSet<Identifiable.Id> GINGER_GROUP = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        internal static readonly HashSet<Identifiable.Id> PLORTS_GROUP = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        internal static readonly HashSet<Identifiable.Id> VEGGIES_GROUP = new HashSet<Identifiable.Id>(Identifiable.idComparer);

        //+ HANDLING
        internal FoodHandler InternalSetup()
        {
            foreach (Identifiable.Id id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.VEGGIES)) VEGGIES_GROUP.Add(id);
            foreach (Identifiable.Id id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.FRUIT)) FRUIT_GROUP.Add(id);
            foreach (Identifiable.Id id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.MEAT)) MEAT_GROUP.Add(id);
            foreach (Identifiable.Id id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.NONTARRGOLD_SLIMES)) NONTARRGOLD_SLIMES_GROUP.Add(id);
            foreach (Identifiable.Id id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.PLORTS)) PLORTS_GROUP.Add(id);
            foreach (Identifiable.Id id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.GINGER)) GINGER_GROUP.Add(id);

            Setup();
            return this;
        }
        
        /// <inheritdoc />
        public virtual FoodHandler Setup()
        {
            APIHandler.HandleRegistration += RegistryHandle;
            APIHandler.HandleItems += Handle;
            APIHandler.ClearMemory += ClearMemory;
            
            return this;
        }
        
        /// <inheritdoc />
        public void RegistryHandle() { }

        /// <inheritdoc />
        public virtual void Handle() { }

        /// <inheritdoc />
        public virtual void ClearMemory() { }
        
        //+ VERIFICATION
        
    }
}