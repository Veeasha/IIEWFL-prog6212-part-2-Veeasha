using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PROG6212.Controllers;
using PROG6212.Models;
using System.Collections.Generic;

namespace PROG6212.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private Mock<ILogger<HomeController>> _loggerMock;

        [TestInitialize]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object);
        }

        [TestMethod]
        public void Index_Returns_ViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void SubmitClaim_Get_Returns_ViewResult()
        {
            // Act
            var result = _controller.SubmitClaim();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void SubmitClaim_Post_ValidClaim_RedirectsToViewClaimStatus()
        {
            // Arrange
            var claim = new Claim
            {
                Lecturer = "Veeasha Packirisamy",
                HoursWorked = 5,
                HourlyRate = 100,
                Status = "Pending"
            };

            // Act
            var result = _controller.SubmitClaim(claim, null);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("ViewClaimStatus", redirectResult.ActionName);
        }

        [TestMethod]
        public void SubmitClaim_Post_InvalidFile_ReturnsViewWithErrors()
        {
            // Arrange
            var claim = new Claim();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6MB, exceeding the 5MB limit
            mockFile.Setup(f => f.FileName).Returns("file.pdf");

            // Act
            var result = _controller.SubmitClaim(claim, mockFile.Object);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey("SupportingDocument"));
        }

        [TestMethod]
        public void ApproveRejectClaims_Returns_ViewWithPendingClaims()
        {
            // Arrange
            var pendingClaim = new Claim { ClaimID = 1, Status = "Pending" };
            HomeController._claims.Add(pendingClaim); // Adding a pending claim to the static list

            // Act
            var result = _controller.ApproveRejectClaims();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<Claim>;
            Assert.AreEqual(1, model.Count); // Should contain the pending claim
        }

        [TestMethod]
        public void ApproveClaim_ChangesClaimStatusToApproved()
        {
            // Arrange
            var claim = new Claim { ClaimID = 1, Status = "Pending" };
            HomeController._claims.Add(claim);

            // Act
            var result = _controller.ApproveClaim(1);

            // Assert
            Assert.AreEqual("Approved", claim.Status);
        }

        [TestMethod]
        public void RejectClaim_ChangesClaimStatusToRejected()
        {
            // Arrange
            HomeController._claims.Clear(); // Ensure the list is empty before test
            var claim = new Claim { ClaimID = 1, Status = "Pending" };
            HomeController._claims.Add(claim);

            // Act
            var result = _controller.RejectClaim(1);

            // Assert
            Assert.AreEqual("Rejected", claim.Status);
        }


        [TestMethod]
        public void ViewClaimStatus_Returns_ViewWithAllClaims()
        {
            // Arrange
            HomeController._claims.Clear(); // Ensure the list is empty before test
            var claim1 = new Claim { ClaimID = 1, Status = "Pending" };
            var claim2 = new Claim { ClaimID = 2, Status = "Approved" };
            HomeController._claims.Add(claim1);
            HomeController._claims.Add(claim2);

            // Act
            var result = _controller.ViewClaimStatus();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<Claim>;
            Assert.AreEqual(2, model.Count); // Ensure the correct number of claims is passed to the view
        }


      
    }
}
