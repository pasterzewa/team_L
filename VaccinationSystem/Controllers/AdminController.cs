﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Services;
using VaccinationSystem.Models;
using VaccinationSystem.DTOs;

namespace VaccinationSystem.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IDatabase dbManager;
        public AdminController(IUserSignInManager signInManager, IDatabase db)
        {
            dbManager = db;
        }

        [HttpGet]
        [Route("vaccinationCenters")]
        public async Task<IActionResult> ShowVaccinationCenters()
        {


            List<VaccinationCenterResponse> centers;
            try
            {
                centers = await dbManager.GetVaccinationCenters();

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (centers == null || centers.Count == 0)
                return NotFound("Data not found");

            return Ok(centers);
        }

        [HttpPost]
        [Route("vaccinationCenters/addVaccinationCenter")]
        public async Task<IActionResult> AddVaccinationCenter([FromBody] AddVaccinationCenterRequest center)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            try
            {
                await dbManager.AddVaccinationCenter(center);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest("Something went wrong");
            }


            return Ok("Vaccination center added");
        }

        [HttpPost]
        [Route("vaccinationCenters/editVaccinationCenter")]
        public async Task<IActionResult> EditVaccinationCenter([FromBody] EditedVaccinationCenter center)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            bool edited = false;
            try
            {
                edited = await dbManager.EditVaccinationCenter(center);

            }
            catch (Exception e)
            {
                return BadRequest("Something went wrong");
            }

            if(edited)
                return Ok("Vaccination center edited");

            return NotFound("Data not found");
        }
        [HttpDelete]
        [Route("vaccinationCenters/deleteVaccinationCenter/{vaccinationCenterId}")]
        public async Task<IActionResult> DeleteVaccinationCenter([FromRoute] Guid vaccinationCenterId)
        {

            bool deleted = false;
            try
            {
                deleted = await dbManager.DeleteVaccinationCenter(vaccinationCenterId);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (deleted)
                return Ok("Vaccination center deleted");

            return NotFound("Data not found");
        }
        [HttpPost]
        [Route("patients/editPatient")]
        public async Task<IActionResult> EditPatient([FromBody] EditedPatient patient)
        {
            //autoryzacja

            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            bool edited = false;
            try
            {
                edited = await dbManager.EditPatient(patient);
            }
            catch (Exception e)
            {
                return BadRequest("Something went wrong");
            }

            if (edited)
                return Ok("Patient edited");

            return NotFound("Data not found");
        }
        [HttpDelete]
        [Route("patients/deletePatient/{patientId}")]
        public async Task<IActionResult> DeletePatient([FromRoute] Guid patientId)
        {
            //autoryzacja

            bool deleted = false;
            try
            {
                deleted = await dbManager.DeletePatient(patientId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (deleted)
                return Ok("Patient deleted");

            return NotFound("Data not found");
        }
        [HttpPost]
        [Route("doctors/addDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] RegisteringDoctor doctor)
        {
            //autoryzacja

            bool edited = false;
            try
            {
                edited = await dbManager.AddDoctor(doctor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (edited)
                return Ok();

            return NotFound("Data not found");
        }
        [HttpPost]
        [Route("doctors/editDoctor")]
        public async Task<IActionResult> EditDoctor([FromBody] EditedDoctor doctor)
        {
            //autoryzacja

            bool edited = false;
            try
            {
                edited = await dbManager.EditDoctor(doctor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (edited)
                return Ok();

            return NotFound("Data not found");
        }
        [HttpDelete]
        [Route("doctors/deleteDoctor/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] Guid doctorId)
        {
            //autoryzacja

            bool deleted = false;
            try
            {
                deleted = await dbManager.DeleteDoctor(doctorId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (deleted)
                return Ok();

            return NotFound("Data not found");
        }
    }
}
