﻿using Goliath.Enums;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <summary>
    /// A repository that manages the timeouts for unauthorized users when they try to request
    /// emails/two factor tokens, etc.
    /// </summary>
    public interface IUnauthorizedTimeoutsRepository
    {
        /// <summary>
        /// <para>
        /// Finds the user in the table through their <paramref name="userId" /> and if they are not
        /// found it creates a new row for them.
        /// </para>
        /// <para>
        /// Then this method replaces the column entry that corresponds with the <paramref
        /// name="requestType" /> with the current UTC time stamp.
        /// </para>
        /// </summary>
        /// <param name="userId"> The Id of the <see cref="Models.ApplicationUser" /> </param>
        /// <param name="requestType"> The type of request that is being made. </param>
        /// <returns> </returns>
        Task UpdateRequestAsync(string userId, UnauthorizedRequest requestType);

        /// <param name="userId"> The ID of the user. </param>
        /// <returns>
        /// Returns whether or not the user can be sent a new two factor code when they first load a
        /// two factor page.
        /// </returns>
        Task<bool> CanRequestTwoFactorSmsAsync(string userId);

        /// <param name="userId"> The ID of the user. </param>
        /// <returns> Returns whether or not the user can request a new two factor code. </returns>
        Task<bool> CanRequestResendTwoFactorSmsAsync(string userId);
    }
}