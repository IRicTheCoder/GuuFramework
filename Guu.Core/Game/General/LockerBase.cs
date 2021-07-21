using System;

namespace Guu.Game.General
{
	/// <summary>The base for a locker, used to simplify the process</summary>
	public abstract class LockerBase<T> where T : Delegate
	{
		/// <summary>The checker method for the locker</summary>
		protected readonly T lockerCheck;

		/// <summary>The unlock state</summary>
		protected bool unlocked;
		
		/// <summary>Creates a new locker</summary>
		protected LockerBase(T check)
		{
			lockerCheck = check;
			unlocked = false;
		}
	}
}