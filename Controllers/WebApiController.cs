using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class WebApiController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> GetLocationByZip(string zipCode)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Google Geocoding API URL
                    var apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={zipCode}&key={"AIzaSyDhiaV5ANBngt5lLZTQLgA0vMDjTAzWa2w"}";
                    var response = await client.GetStringAsync(apiUrl);
                    var data = JsonConvert.DeserializeObject<dynamic>(response);

                    if (data != null && data.results.Count > 0)
                    {
                        var addressComponents = data.results[0].address_components;
                        string country = null, state = null, city = null;

                        foreach (var component in addressComponents)
                        {
                            var types = component.types.ToObject<List<string>>();
                            if (types.Contains("country"))
                                country = component.long_name;
                            if (types.Contains("administrative_area_level_1"))
                                state = component.long_name;
                            if (types.Contains("locality") || types.Contains("sublocality") || types.Contains("postal_town"))
                                city = component.long_name;
                        }

                        return Json(new { Country = country, State = state, City = city }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }
        //public ActionResult Create()
        //{
        //    var model = new Account
        //    {
        //        Countries = new List<SelectListItem>
        //{
        //     new SelectListItem { Value = "", Text = "Select Country", Selected = true }, // Placeholder option
        //    new SelectListItem { Value = "US", Text = "United States" },
        //    new SelectListItem { Value = "CA", Text = "Canada" },
        //    new SelectListItem { Value = "GB", Text = "United Kingdom" },
        //    new SelectListItem { Value = "IN", Text = "India" }, 
        //    new SelectListItem { Value = "SI", Text ="Singapore"}
        //    // Add more countries as needed
        //}
        //    };

        //    return View(model);
        //}


        [HttpPost]
        public async Task<ActionResult> Create(Account model)
        {
            if (ModelState.IsValid)
            {
                // Generate JWT token
                string token = GenerateJwtToken(model.Email);

                // Define the API endpoint
                var outreachLoginUrl = "https://localhost:44369/api/WebApi/postdata";

                // Initialize HttpClient
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(outreachLoginUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Serialize the model to JSON
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(outreachLoginUrl, jsonContent);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Form submitted successfully!";
                        return RedirectToAction("Create"); // Redirect to clear the form and display the message
                    }
                    else
                    {
                        ViewBag.ApiResponse = "Error: Unable to call the API";
                    }
                }
            }
            return View("Create", model);
        }

        private string GenerateJwtToken(string username)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("C1CF487DC4C4175B6618DE4F55CA4V1"));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("username", username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),

                claims: claims,
                signingCredentials: credentials
            );

            var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
        public JsonResult GetStatesByCountry(string country)
        {
            List<string> states = new List<string>();

            // States for the United States
            if (country == "US")
            {
                states = new List<string>
        {
            "Alabama", "Alaska", "Arizona", "Arkansas", "California",
            "Florida", "Georgia", "Hawaii", "Idaho", "Illinois",
            "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana",
            "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota",
            "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada",
            "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina",
            "North Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania",
            "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas",
            "Utah", "Vermont", "Virginia", "Washington", "West Virginia",
            "Wisconsin", "Wyoming"
        };
            }
            // States/Provinces for Canada
            else if (country == "CA")
            {
                states = new List<string>
        {
            "Alberta", "British Columbia", "Manitoba", "New Brunswick",
            "Newfoundland and Labrador", "Northwest Territories",
            "Nova Scotia", "Ontario", "Prince Edward Island",
            "Quebec", "Saskatchewan", "Yukon"
        };
            }
            // Regions for the United Kingdom (since UK doesn't have states, we can return regions)
            else if (country == "GB")
            {
                states = new List<string>
        {
            "England", "Scotland", "Wales", "Northern Ireland"
        };
            }
            else if (country == "IN")
            {
                states = new List<string>
        {
           "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chhattisgarh", "Goa", "Gujarat",
            "Haryana", "Himachal Pradesh", "Jharkhand", "Karnataka", "Kerala", "Madhya Pradesh", "Maharashtra",
            "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Odisha", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu",
            "Telangana", "Tripura", "Uttar Pradesh", "Uttarakhand", "West Bengal",
        };
            }
            // Example for Australia
            else if (country == "AU")
            {
                states = new List<string>
        {
            "Australian Capital Territory", "New South Wales", "Northern Territory",
            "Queensland", "South Australia", "Tasmania", "Victoria", "Western Australia"
        };
            }
            else if (country == "SI")
            { states = new List<string>
            {
                "Singapore"
            };
        }
            // Return the list of states/provinces/regions as JSON
            return Json(states, JsonRequestBehavior.AllowGet);
        }
        //    private string GenerateJwtToken(string username)
        //    {
        //        // Create a symmetric key to sign the JWT token
        //        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes("C1CF487DC4C4175B6618DE4F55CA4V1"));
        //        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        //        // Create claims for the JWT token
        //        var claims = new[]
        //        {
        //            new Claim(JwtRegisteredClaimNames.Sub, username),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //        };

        //        // Create the token
        //        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        //            Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
        //            Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
        //            claims,
        //            signingCredentials : credentials
        //            );

        //        // Use JwtSecurityTokenHandler to write the token as a string
        //        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //}
    }
}
