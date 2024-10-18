using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PROG6212.Models;

namespace PROG6212.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Static list to hold claims (use a database in production)
        public static List<Claim> _claims = new List<Claim>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, IFormFile SupportingDocument)
        {
            if (SupportingDocument != null)
            {
                // Check the file size (e.g., 5MB limit)
                if (SupportingDocument.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("SupportingDocument", "The file size must not exceed 5MB.");
                    return View(claim);
                }

                // Check the file type
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(SupportingDocument.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("SupportingDocument", "Invalid file type. Only .pdf, .docx, and .xlsx are allowed.");
                    return View(claim);
                }

                // Save the file securely
                var filePath = Path.Combine("wwwroot/uploads", SupportingDocument.FileName); // Adjust file storage location
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    SupportingDocument.CopyTo(stream);
                }

                // Store the file name in the claim
                claim.SupportingDocument = SupportingDocument.FileName;
            }

            claim.Status = "Pending";
            claim.ClaimID = _claims.Count + 1;
            _claims.Add(claim);

            return RedirectToAction("ViewClaimStatus");
        }

        public IActionResult ApproveRejectClaims()
        {
            return View(_claims.Where(c => c.Status == "Pending").ToList());
        }

        [HttpPost]
        public IActionResult ApproveClaim(int claimId)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimID == claimId);
            if (claim != null)
            {
                claim.Status = "Approved";
            }
            return RedirectToAction("ApproveRejectClaims");
        }

        [HttpPost]
        public IActionResult RejectClaim(int claimId)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimID == claimId);
            if (claim != null)
            {
                claim.Status = "Rejected";
            }
            return RedirectToAction("ApproveRejectClaims");
        }

        public IActionResult ViewClaimStatus()
        {
            return View(_claims); // Pass the claims to the view
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
