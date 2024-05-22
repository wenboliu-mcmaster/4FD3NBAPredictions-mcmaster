/*  Filename: PoolServiceTests.cs
    Author: Emily Ramanna
    Description: Demonstration of how the PoolService class can be unit tested
*/

using _4FD3NBAPredictions.Models;
using _4FD3NBAPredictions.Repositories;
using _4FD3NBAPredictions.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _4FD3NBAPredictionsTesting
{
    [TestClass]
    public class PoolServiceTests
    {
        [TestMethod]
        public void AcceptOrDismissInvite_PoolMembershipIsNull_ReturnNoInvite()
        {
            //ARRANGE
            var poolRepo = new Mock<PoolRepository>();
            var poolService = new PoolService(poolRepo.Object);

            //ACT
            string result = poolService.AcceptOrDismissInvite(null, false);

            //ASSERT
            Assert.IsTrue(result.Contains("not invited"));
        }

        [TestMethod]
        public void AcceptOrDismissInvite_NotInvited_ReturnNoInvite()
        {
            //ARRANGE
            var poolRepo = new Mock<PoolRepository>();
            var poolService = new PoolService(poolRepo.Object);
            var poolMembership = new PoolMembership { StatusFlag = (int)_4FD3NBAPredictions.Models.Enums.MembershipStatusEnum.Accepted };
            //ACT
            string result = poolService.AcceptOrDismissInvite(poolMembership, false);

            //ASSERT
            Assert.IsTrue(result.Contains("not invited"));
        }

        [TestMethod]
        public void AcceptOrDismissInvite_SuccessfullyAccepted_ReturnSuccessString()
        {
            //ARRANGE
            var poolRepo = new Mock<PoolRepository>();
            var poolService = new PoolService(poolRepo.Object);
            var poolMembership = new PoolMembership { StatusFlag = (int)_4FD3NBAPredictions.Models.Enums.MembershipStatusEnum.Invited };

            //ACT
            string result = poolService.AcceptOrDismissInvite(poolMembership, true);

            //ASSERT
            Assert.IsTrue(result.Contains("Accepted invite"));
        }


        [TestMethod]
        public void AcceptOrDismissInvite_SuccessfullyDismissed_ReturnSuccessString()
        {
            //ARRANGE
            var poolRepo = new Mock<PoolRepository>();
            var poolService = new PoolService(poolRepo.Object);
            var poolMembership = new PoolMembership { StatusFlag = (int)_4FD3NBAPredictions.Models.Enums.MembershipStatusEnum.Invited };

            //ACT
            string result = poolService.AcceptOrDismissInvite(poolMembership, false);

            //ASSERT
            Assert.IsTrue(result.Contains("Dismissed invite"));
        }


    }
}
