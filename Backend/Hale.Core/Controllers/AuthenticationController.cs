﻿using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Hale.Core.Models.Messages;
using Microsoft.Owin.Security;
using NLog;
using System.Linq;
using Hale.Core.Data.Contexts;
using Hale.Core.Model.Models;

namespace Hale.Core.Controllers
{
    /// <summary>
    /// TODO: Add text here
    /// </summary>
    [RoutePrefix("api/v1/authentication")]
    public class AuthenticationController : ApiController
    {

        #region Constructors and declarations
        private readonly Logger      _log;
        private readonly HaleDBContext _db;

        internal AuthenticationController() : this(new HaleDBContext()) { }

        /// <summary>
        /// TODO: Add text here
        /// </summary>
        /// <param name="context"></param>
        public AuthenticationController(HaleDBContext context)
        {
            _log = LogManager.GetCurrentClassLogger();
            _db = context;
        }
        #endregion
        #region Private Methods

        private void CreateUserClaims(LoginAttemptDTO attempt)
        {
            Request
                .GetOwinContext()
                .Authentication
                .SignIn(
                    new AuthenticationProperties()
                    {
                        IsPersistent = attempt.Persistent
                    },
                    new ClaimsIdentity(
                        new[] {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, attempt.Username)
                        }, "HaleCoreAuth")
                    );
        }

        #endregion

        internal string _currentUsername => Request
            .GetOwinContext()
            .Authentication
            .User
            .Identities
            .First()
            .Claims
            .First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            .Value;

        /// <summary>
        /// Creates a claim on successful sign in.
        /// </summary>
        /// <param name="attempt"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] LoginAttemptDTO attempt)
        {
            var user = _db.Accounts.FirstOrDefault(x => x.UserName == attempt.Username);
            if (user == null)
                return Unauthorized();

            var passwordAccepted = BCrypt.Net.BCrypt.Verify(attempt.Password, user.Password);

            if (!passwordAccepted)
                return Unauthorized();

            else {
                CreateUserClaims(attempt);
                return Ok();
            }
        }

        /// <summary>
        /// TODO: Add text here
        /// </summary>
        /// <returns></returns>
        [Route()]
        [HttpGet]
        [Authorize(Roles = "User")]
        public IHttpActionResult Status()
        {
            return Ok();
        }

        /// <summary>
        /// TODO: Add text here
        /// </summary>
        /// <returns></returns>
        [Route()]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult Logout()
        {
            var context = Request.GetOwinContext();
            context.Authentication.SignOut();
            return Ok();
        }

        [Route("change-password")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult ChangePassword(PasswordDTO passwordChange)
        {
            var user = _db.Accounts.Single(x => x.UserName == _currentUsername);
            var passwordAccepted = BCrypt.Net.BCrypt.Verify(passwordChange.oldPassword, user.Password);

            if (!passwordAccepted)
                return Unauthorized();

            var newPassword = BCrypt.Net.BCrypt.HashPassword(passwordChange.newPassword, 5);

            user.Password = newPassword;
            _db.SaveChanges();

            return Ok();
        }

    }
}
