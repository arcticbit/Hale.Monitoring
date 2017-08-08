﻿using Hale.Core.Models.Messages;

namespace Hale.Core.Model.Interfaces
{
    public interface IAuthService
    {
        bool Authorize(string userName, string password);
        void ChangePassword(string userName, string newPassword);

    }
}
