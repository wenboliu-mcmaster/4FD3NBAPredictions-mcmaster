/*  Filename: UserServiceTests.cs
    Author: Emily Ramanna
    Description: Demonstration of how the UserService class can be unit tested
*/
using _4FD3NBAPredictions.Models;
using _4FD3NBAPredictions.Repositories;
using _4FD3NBAPredictions.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;

namespace _4FD3NBAPredictionsTesting
{
    [TestClass]
    public class UserServiceTests
    {

        [TestMethod]
        public async Task VerifyReadinessToSetNicknameAsync_UserNull_ReturnFalse()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = null;
            var identity = new Mock<IEnumerable<Claim>>();

            //ACT
            bool result = await userService.VerifyReadinessToSetNicknameAsync(user, identity.Object);

            //ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task VerifyReadinessToSetNicknameAsync_UserId0_ReturnFalse()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = "as", UserId = 0};
            var identity = new Mock<IEnumerable<Claim>>();

            //ACT
            bool result = await userService.VerifyReadinessToSetNicknameAsync(user, identity.Object);

            //ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task VerifyReadinessToSetNicknameAsync_UserNicknameNull_ReturnFalse()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            var identity = new Mock<IEnumerable<Claim>>();

            //ACT
            bool result = await userService.VerifyReadinessToSetNicknameAsync(user, identity.Object);

            //ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task VerifyReadinessToSetNicknameAsync_UserDoesNotExist_ReturnFalse()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            userRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns((Task<User>)null);
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = "as", UserId = 20 };
            var identity = new Mock<IEnumerable<Claim>>();

            //ACT
            bool result = await userService.VerifyReadinessToSetNicknameAsync(user, identity.Object);

            //ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task VerifyReadinessToSetNicknameAsync_UserAlreadyHasANickname_ExceptionThrown()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            User repoUser = new User { Email = "a@s.com", Nickname = "as", UserId = 20 };
            userRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns((Task.FromResult(repoUser)));
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = "as", UserId = 20 };
            var identity = new Mock<IEnumerable<Claim>>();

            //ACT
            bool result = await userService.VerifyReadinessToSetNicknameAsync(user, identity.Object);

            //ASSERT
            //taken care of by ExpectedException
        }

        [TestMethod]
        public async Task VerifyReadinessToSetNicknameAsync_SuccessfulUpdate_ReturnTrue()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            User repoUser = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            userRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns((Task.FromResult(repoUser)));
            
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = "as", UserId = 20 };
            var identity = new Mock<IEnumerable<Claim>>();

            userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns((Task.FromResult(user)));

            //ACT
            bool result = await userService.VerifyReadinessToSetNicknameAsync(user, identity.Object);

            //ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateNickname_NicknameTooShort_ExceptionThrown()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
           
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            string nickname = "as";

            //ACT
            User result = await userService.UpdateNickname(user, nickname);

            //ASSERT
            //taken care of by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateNickname_NicknameTooLong_ExceptionThrown()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();

            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            string nickname = "abcdefghijklmnopqrstuvwxyzabcdefghi";

            //ACT
            User result = await userService.UpdateNickname(user, nickname);

            //ASSERT
            //taken care of by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateNickname_NicknameNotAlphanumeric_ExceptionThrown()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();

            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            string nickname = "hi.there";

            //ACT
            User result = await userService.UpdateNickname(user, nickname);

            //ASSERT
            //taken care of by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public async Task UpdateNickname_NicknameAlreadyTaken_ExceptionThrown()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Throws<DbUpdateException>();
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            string nickname = "as3";

            //ACT
            User result = await userService.UpdateNickname(user, nickname);

            //ASSERT
            //taken care of by ExpectedException
        }

        [TestMethod]
        public async Task UpdateNickname_SuccessfulUpdate_ReturnsUser()
        {
            //ARRANGE
            var userRepo = new Mock<UserRepository>();
            User repoUser = new User { Email = "a@s.com", Nickname = "as3", UserId = 20 };
            userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns((Task.FromResult(repoUser)));
            var config = new Mock<IConfiguration>();

            var userService = new UserService(userRepo, config);

            User user = new User { Email = "a@s.com", Nickname = null, UserId = 20 };
            string nickname = "as3";

            //ACT
            User result = await userService.UpdateNickname(user, nickname);

            //ASSERT
            Assert.AreEqual(repoUser.Nickname, nickname);
        }
    }
}
