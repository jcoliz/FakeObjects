using System;
using System.Collections;
using System.Collections.Generic;

namespace jcoliz.FakeObjects
{
    /// <summary>
    /// Public interface for the fluid composer of fake objects operations
    /// </summary>
    public interface IFakeObjects<T>: IEnumerable<T> where T : class, new()
    {
        #region Accessors

        /// <summary>
        /// Pick one particular series
        /// </summary>
        /// <param name="index">Which series</param>
        /// <returns>A list of all items in the chosen series</returns>
        IList<T> Group(int index);

        /// <summary>
        /// Pick many series
        /// </summary>
        /// <param name="index">What series range</param>
        /// <returns>All items in the chosen series</returns>
        IEnumerable<T> Groups(Range index);

        /// <summary>
        /// Total number of items
        /// </summary>
        int Count { get; }
        
        #endregion

        #region Actions

        /// <summary>
        /// Add another group of fake objects to this one
        /// </summary>
        /// <param name="count">How many objects</param>
        /// <param name="func">What changes to make on them</param>
        IFakeObjects<T> Add(int count, Action<T> func = null);

        /// <summary>
        /// Save all objects created sofar to the chosen target
        /// </summary>
        /// <param name="target">Where to save them</param>
        IFakeObjects<T> SaveTo(IFakeObjectsSaveTarget target);

        #endregion
    }

    /// <summary>
    /// A target where fake objects could be saved to
    /// </summary>
    /// <remarks>
    /// Allows an object to be passed into SaveTo()
    /// </remarks>
    public interface IFakeObjectsSaveTarget
    {
        void AddRange(IEnumerable objects);
    }
}