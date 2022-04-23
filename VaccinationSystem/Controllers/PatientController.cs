﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Models;
using VaccinationSystem.Services;
using VaccinationSystem.DTOs;

namespace VaccinationSystem.Controllers
{
    [Route("patient")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private IUserSignInManager signInManager;
        private IDatabase dbManager;
        public PatientController(IUserSignInManager signInManager, IDatabase db)
        {
            this.signInManager = signInManager;
            dbManager = db;
        }

        [Route("timeSlots/book/{patientId}/{timeSlotId}/{vaccineId}")]
        [HttpPost]
        public async Task<IActionResult> MakeAppointment([FromRoute] Guid patientId, [FromRoute] Guid timeSlotId, [FromRoute] Guid vaccineId)
        {
            bool made = false;
            try
            {
                made = await dbManager.MakeAppointment(patientId, timeSlotId, vaccineId);
            }
            catch (ArgumentException)
            {
                return StatusCode(403,"User forbidden from booking");
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }

            if (made)
                return Ok("Booked a time slot");

            return NotFound("Data not found");
        }

        [HttpGet]
        [Route("timeSlots/Filter")]
        public async Task<IActionResult> GetTimeSlots(string city, string virus, string dateFrom, string dateTo)
        {
            if (city == null || virus == null || dateFrom == null || dateTo == null)
                return BadRequest("Invalid model");

            var filter = new TimeSlotsFilter()
            {
                city = city,
                virus = virus,
                dateFrom = dateFrom,
                dateTo = dateTo
            };
            List<FilterTimeSlotResponse> timeSlots;
            try
            {
                timeSlots = await dbManager.GetTimeSlotsWithFiltration(filter);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest("Something went wrong");
            }

            System.IO.File.WriteAllText("C:\\Users\\agowo\\Desktop\\file.txt", timeSlots.Count.ToString());

            if (timeSlots == null || timeSlots.Count == 0)
                return NotFound("Data not found");

            var response = new FilterTimeSlotsControllerResponse() { data = timeSlots };
            return Ok(response);
        }
    }
}
