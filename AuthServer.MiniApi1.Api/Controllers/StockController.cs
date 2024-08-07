﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace AuthServer.MiniApi1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [Authorize(Roles = "Admin", Policy = "BakuPolicy")]
        [Authorize(Policy = "AgePolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name;


            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            // Retrieve the necessary data from the database using the userId or userName fields

            // stockId, stockQuantity, Category, UserId/UserName

            return Ok($"Stock   =>UserName: {userName}- UserId:{userIdClaim.Value}");
        }
    }
}
