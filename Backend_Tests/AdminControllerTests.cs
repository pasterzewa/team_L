﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using VaccinationSystem.Services;
using VaccinationSystem.Models;
using VaccinationSystem.DTOs;
using VaccinationSystem.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Tests
{
    public class AdminControllerTests
    {

        private Guid vCenterId = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f");
        private Guid patientID = new Guid("55A2BBCE-E031-4931-E751-08DA13EF87A5");
        private Guid doctorID = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE");
        private Guid vaccineID = new Guid("6DD0DA08-4C9E-4606-A797-08DA3C1159E9");
        private List<DeleteTimeSlot> listDeleteTimeSlots = new List<DeleteTimeSlot>() {
                    new DeleteTimeSlot(){ id=new Guid("56FD372F-237C-4D87-87C5-08DA3C232B6F") },
                    new DeleteTimeSlot(){ id = new Guid("4F442870-B313-4DB8-87C7-08DA3C232B6F")}
            };
        [Fact]
        public async Task ShowVaccinationCentersReturnsCenters()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccinationCenters()).ReturnsAsync(GetCenters);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var centers = await controller.ShowVaccinationCenters();


            var okResult = Assert.IsType<OkObjectResult>(centers);


            var returnValue = Assert.IsType<List<VaccinationCenterResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

        }

        [Fact]
        public async Task ShowVaccinationCentersReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccinationCenters()).ReturnsAsync(new List<VaccinationCenterResponse>());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var centers = await controller.ShowVaccinationCenters();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(centers);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }


        [Fact]
        public async Task ShowVaccinationCentersReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccinationCenters()).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var centers = await controller.ShowVaccinationCenters();

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(centers);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddVaccinationCentersReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.AddVaccinationCenter(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task AddVaccinationCentersReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetAddingVaccinationCenter();

            mockDB.Setup(dB => dB.AddVaccinationCenter(center)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.AddVaccinationCenter(center);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddVaccinationCentersReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetAddingVaccinationCenter();
            mockDB.Setup(dB => dB.AddVaccinationCenter(center));
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.AddVaccinationCenter(center);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditVaccinationCentersReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.EditVaccinationCenter(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task EditVaccinationCentersReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetEditingVaccinationCenter();

            mockDB.Setup(dB => dB.EditVaccinationCenter(center)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditVaccinationCenter(center);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task EditVaccinationCentersReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetEditingVaccinationCenter();
            mockDB.Setup(dB => dB.EditVaccinationCenter(center)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditVaccinationCenter(center);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditVaccinationCentersReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetEditingVaccinationCenter();
            mockDB.Setup(dB => dB.EditVaccinationCenter(center)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditVaccinationCenter(center);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteVaccinationCentersReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.DeleteVaccinationCenter(vCenterId)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteVaccinationCenter(vCenterId);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task DeleteVaccinationCentersReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.DeleteVaccinationCenter(vCenterId))
                .ThrowsAsync(new ArgumentException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteVaccinationCenter(vCenterId);

            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbiddenResult.StatusCode);
        }
        [Fact]
        public async Task DeleteVaccinationCentersReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetEditingVaccinationCenter();
            mockDB.Setup(dB => dB.DeleteVaccinationCenter(vCenterId)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteVaccinationCenter(vCenterId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteVaccinationCentersReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var center = GetEditingVaccinationCenter();
            mockDB.Setup(dB => dB.DeleteVaccinationCenter(vCenterId)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteVaccinationCenter(vCenterId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
        private List<VaccinationCenterResponse> GetCenters()
        {

            var centers = new List<VaccinationCenterResponse>();

            centers.Add(new VaccinationCenterResponse()
            {
                id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                name = "Punkt Szczepień Populacyjnych",
                city = "Warszawa",
                street = "Żwirki i Wigury 95/97",
                active = true,
                openingHoursDays = new[]
                 {
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "10:00",
                        }

                    },
                vaccines = new List<Vaccine>()
                    {
                        new Vaccine()
                        {
                             id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                            company = "Pfeizer",
                            name = "Pfeizer vaccine",
                            numberOfDoses = 2,
                            minDaysBetweenDoses = 30,
                            minPatientAge = 12,
                            virus = Virus.Coronavirus,
                            active = true
                        },
                       new Vaccine()
                        {
                             id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                            company = "Moderna",
                            name = "Moderna vaccine",
                            numberOfDoses = 2,
                            minDaysBetweenDoses = 30,
                            minPatientAge = 18,
                            maxPatientAge = 99,
                            virus = Virus.Coronavirus,
                            active = true
                        },
                    }

            });
            centers.Add(new VaccinationCenterResponse()
            {
                id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                name = "Apteczny Punkt Szczepień",
                city = "Warszawa",
                street = "Mokotowska 27/Lok.1 i 4",
                openingHoursDays = new[]
                 {
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "10:00",
                        }

                    },
                vaccines = new List<Vaccine>()
                    {
                        new Vaccine()
                        {
                            id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                            company = "Pfeizer",
                            name = "Pfeizer vaccine",
                            numberOfDoses = 2,
                            minDaysBetweenDoses = 30,
                            minPatientAge = 12,
                            virus = Virus.Coronavirus,
                            active = true
                        },
                       new Vaccine()
                        {
                            id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                            company = "Moderna",
                            name = "Moderna vaccine",
                            numberOfDoses = 2,
                            minDaysBetweenDoses = 30,
                            minPatientAge = 18,
                            maxPatientAge = 99,
                            virus = Virus.Coronavirus,
                            active = true
                        },
                    }

            });


            return centers;
        }

        private AddVaccinationCenterRequest GetAddingVaccinationCenter()
        {
            return new AddVaccinationCenterRequest
            {
                name = "Punkt Szczepień Populacyjnych",
                city = "Warszawa",
                street = "Żwirki i Wigury 95/97",
                active = true,
                openingHoursDays = new[]
                 {
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "10:00",
                        }

                    },
                vaccineIds = new List<Guid>()
                    {
                        new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                        new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f")

                    }
            };
        }
        private EditedVaccinationCenter GetEditingVaccinationCenter()
        {
            return new EditedVaccinationCenter
            {
                id = new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                name = "Punkt Szczepień Populacyjnych",
                city = "Warszawa",
                street = "Żwirki i Wigury 95/97",
                active = true,
                openingHoursDays = new[]
                 {
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "13:00",
                        },
                        new OpeningHoursDays()
                        {
                            from = "8:00",
                            to = "10:00",
                        }

                    },
                vaccineIds = new List<Guid>()
                    {
                        new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f"),
                                new Guid("1c8ddbb7-06c8-44ec-893e-f936607aa36f")

                    }
            };
        }

        private EditedPatient GetEditedPatient()
        {
            return new EditedPatient
            {
                id = patientID,
                firstName = "Jan",
                lastName = "Nowakowy",
                dateOfBirth = "1982-12-12",
                PESEL = "82121211111",
                mail = "j.nowak@zmienionymail.com",
                phoneNumber = "+48555221331",
                active = true
            };
        }

        [Fact]
        public async Task EditPatientReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var patient = GetEditedPatient();
            mockDB.Setup(dB => dB.EditPatient(patient)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditPatient(patient);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task EditPatientReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var patient = GetEditedPatient();
            mockDB.Setup(dB => dB.EditPatient(patient)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditPatient(patient);

            Assert.IsType<NotFoundObjectResult>(result);
        }
        [Fact]
        public async Task EditPatientReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var patient = GetEditedPatient();

            mockDB.Setup(dB => dB.EditPatient(patient)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditPatient(patient);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task EditPatientReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.EditPatient(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeletePatientReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeletePatient(patientID)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeletePatient(patientID);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task DeletePatientReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.DeletePatient(patientID)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeletePatient(patientID);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task DeletePatientReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.DeletePatient(patientID))
                .ThrowsAsync(new ArgumentException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeletePatient(patientID);

            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbiddenResult.StatusCode);
        }
        [Fact]
        public async Task DeletePatientReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeletePatient(patientID)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeletePatient(patientID);

            Assert.IsType<NotFoundObjectResult>(result);
        }
        private EditedDoctor GetEditedDoctor()
        {
            return new EditedDoctor
            {
                doctorId = doctorID,
                firstName = "Robert",
                lastName = "Weide",
                dateOfBirth = "1959-06-20",
                PESEL = "59062011333",
                mail = "robert.b.weide@mail.com",
                phoneNumber = "+48125200331",
                vaccinationCenterId = new Guid("56B289F4-FF8F-41D6-50B2-08DA13EF879D")
            };
        }
        private RegisteringDoctor GetNewDoctor()
        {
            return new RegisteringDoctor
            {
                patientId = patientID,
                vaccinationCenterId = vCenterId
            };
        }

        [Fact]
        public async Task AddDoctorReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var doctor = GetNewDoctor();
            mockDB.Setup(dB => dB.AddDoctor(doctor));
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.AddDoctor(doctor);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task AddDoctorReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var doctor = GetNewDoctor();

            mockDB.Setup(dB => dB.AddDoctor(doctor)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.AddDoctor(doctor);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task AddDoctorReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.AddDoctor(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EditDoctorReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var doctor = GetEditedDoctor();
            mockDB.Setup(dB => dB.EditDoctor(doctor)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditDoctor(doctor);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task EditDoctorReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var doctor = GetEditedDoctor();
            mockDB.Setup(dB => dB.EditDoctor(doctor)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditDoctor(doctor);

            Assert.IsType<NotFoundObjectResult>(result);
        }
        [Fact]
        public async Task EditDoctorReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var doctor = GetEditedDoctor();

            mockDB.Setup(dB => dB.EditDoctor(doctor)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditDoctor(doctor);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task EditDoctorReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.EditDoctor(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteDoctorReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteDoctor(doctorID)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteDoctor(doctorID);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task DeleteDoctorReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.DeleteDoctor(doctorID)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteDoctor(doctorID);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task DeleteDoctorReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.DeleteDoctor(doctorID))
                .ThrowsAsync(new ArgumentException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteDoctor(doctorID);

            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbiddenResult.StatusCode);
        }
        [Fact]
        public async Task DeleteDoctorReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteDoctor(doctorID)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.DeleteDoctor(doctorID);

            Assert.IsType<NotFoundObjectResult>(result);
        }
        [Fact]
        public async Task GetDoctorsReturnsDoctors()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetDoctors()).ReturnsAsync(GetDoctors);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var doctors = await controller.GetDoctors();

            var okResult = Assert.IsType<OkObjectResult>(doctors);

            var returnValue = Assert.IsType<List<DoctorResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetDoctorsReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetDoctors()).ReturnsAsync(new List<DoctorResponse>());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var doctors = await controller.GetDoctors();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(doctors);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        private List<DoctorResponse> GetDoctors()
        {
            var doctors = new List<DoctorResponse>();
            doctors.Add(new DoctorResponse()
            {
                id = new Guid("98A1B9A6-0E4E-46C7-443A-08DA1B08BAE6"),
                PESEL = "59062011333",
                firstName = "Robert",
                lastName = "Weide",
                dateOfBirth = "1959-06-20",
                mail = "robert.b.weide@mail.com",
                phoneNumber = "+48125200331",
                active = true,
                vaccinationCenterId = new Guid("99766467-2246-4DE2-FF97-08DA1B08BA99"),
                name = "Punkt Szczepień Populacyjnych",
                city = "Warszawa",
                street = "Żwirki i Wigury 95/97",
            });
            doctors.Add(new DoctorResponse()
            {
                PESEL = "74011011111",
                dateOfBirth = "1974-01-10",
                firstName = "Monika",
                lastName = "Kowalska",
                mail = "m.kowalska@mail.com",
                phoneNumber = "+48349824991",
                vaccinationCenterId = new Guid("B8FA9079-6FD0-4759-FF98-08DA1B08BA99"),
                name = "Apteczny Punkt Szczepień",
                city = "Warszawa",
                street = "Mokotowska 27/Lok.1 i 4",
            });
            return doctors;
        }
        [Fact]
        public async Task GetPatientsReturnsPatients()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetPatients()).ReturnsAsync(GetPatients);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var patients = await controller.GetPatients();

            var okResult = Assert.IsType<OkObjectResult>(patients);

            var returnValue = Assert.IsType<List<PatientResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetPatientsReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetPatients()).ReturnsAsync(new List<PatientResponse>());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var patients = await controller.GetPatients();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(patients);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        private List<PatientResponse> GetPatients()
        {
            var patients = new List<PatientResponse>();
            patients.Add(new PatientResponse()
            {
                id = new Guid("522A0EC0-1727-44C9-A308-08DA1B08BABF"),
                PESEL = "82121211111",
                dateOfBirth = "1982-12-12",
                firstName = "Jan",
                lastName = "Nowak",
                mail = "j.nowak@mail.com",
                phoneNumber = "+48555221331",
                active = true,
            });
            patients.Add(new PatientResponse()
            {
                id = new Guid("35D520DC-16AF-48E6-A309-08DA1B08BABF"),
                PESEL = "92120211122",
                dateOfBirth = "1992-12-02",
                firstName = "Janina",
                lastName = "Nowakowa",
                mail = "j.nowakowa@mail.com",
                phoneNumber = "+48576221390",
                active = true,
            });
            return patients;
        }
        [Fact]
        public async Task GetPatientReturnsPatient()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            var patient = GetPatient();

            mockDB.Setup(dB => dB.GetPatient(patient.id)).ReturnsAsync(GetPatient);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var patientFromController = await controller.GetPatient(patient.id);
            var okResult = Assert.IsType<OkObjectResult>(patientFromController);

            var returnValue = Assert.IsType<PatientResponse>(okResult.Value);
            Assert.Equal(patient.id, returnValue.id);
        }

        [Fact]
        public async Task GetPatientReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            var patient = GetPatient();
            PatientResponse patientRNull = null;

            mockDB.Setup(dB => dB.GetPatient(patient.id)).ReturnsAsync(patientRNull);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var patientFromController = await controller.GetPatient(patient.id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(patientFromController);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        private PatientResponse GetPatient()
        {
            return new PatientResponse()
            {
                id = new Guid("522A0EC0-1727-44C9-A308-08DA1B08BABF"),
                PESEL = "82121211111",
                dateOfBirth = "1982-12-12",
                firstName = "Jan",
                lastName = "Nowak",
                mail = "j.nowak@mail.com",
                phoneNumber = "+48555221331",
                active = true,
            };
        }
        [Fact]
        public async Task GetTimeSlotsReturnsTimeSlots()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetAllTimeSlots(doctorID)).ReturnsAsync(GetAllTimeSlots);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var slots = await controller.GetTimeSlots(doctorID);


            var okResult = Assert.IsType<OkObjectResult>(slots);


            var returnValue = Assert.IsType<List<WholeTimeSlotsResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

        }

        [Fact]
        public async Task GetTimeSlotsReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetTimeSlots(doctorID)).ReturnsAsync(new List<TimeSlotsResponse>());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.GetTimeSlots(doctorID);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(timeSlots);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        private List<TimeSlotsResponse> GetTimeSlots()
        {
            var timeSlots = new List<TimeSlotsResponse>()
            {
                new TimeSlotsResponse()
                {
                    id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                    from = "2022-01-29T08:00",
                    to = "2022-01-29T09:00",
                    isFree = true
                },
                new TimeSlotsResponse()
                {
                    id = new Guid("55A2BBCE-E031-4931-E751-08DA13EF87A5"),
                    from = "2022-01-29T09:00",
                    to = "2022-01-29T10:00",
                    isFree = true
                }
            };
            return timeSlots;
        }
        private List<WholeTimeSlotsResponse> GetAllTimeSlots()
        {
            var timeSlots = new List<WholeTimeSlotsResponse>()
            {
                new WholeTimeSlotsResponse()
                {
                    id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                    from = "2022-01-29T08:00",
                    to = "2022-01-29T09:00",
                    isFree = true,
                    active = true
                },
                new WholeTimeSlotsResponse()
                {
                    id = new Guid("55A2BBCE-E031-4931-E751-08DA13EF87A5"),
                    from = "2022-01-29T09:00",
                    to = "2022-01-29T10:00",
                    isFree = true,
                    active = true
                }
            };
            return timeSlots;
        }
        [Fact]
        public async Task EditVaccineReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.EditVaccine(It.IsAny<EditVaccine>())).ReturnsAsync(() => true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditVaccine(It.IsAny<EditVaccine>());

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task EditVaccineReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.EditVaccine(It.IsAny<EditVaccine>())).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditVaccine(It.IsAny<EditVaccine>());

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task EditVaccineReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.EditVaccine(It.IsAny<EditVaccine>())).ReturnsAsync(() => false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.EditVaccine(It.IsAny<EditVaccine>());

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Error, no vaccine found to edit", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task AddVaccineReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.AddVaccine(It.IsAny<AddVaccine>()));
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.AddVaccine(It.IsAny<AddVaccine>());

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task AddVaccineReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            mockDB.Setup(dB => dB.AddVaccine(It.IsAny<AddVaccine>())).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);

            var result = await controller.AddVaccine(It.IsAny<AddVaccine>());

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task GetVaccinesReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccines()).ReturnsAsync(Vaccines());
            var contoller = new AdminController(mockSignIn.Object, mockDB.Object);
            var vaccines = await contoller.GetVaccines();
            var okResult = Assert.IsType<OkObjectResult>(vaccines);
            var returnValue = Assert.IsType<List<VaccineResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async Task GetVaccinesReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccines()).ReturnsAsync(new List<VaccineResponse>());
            var contoller = new AdminController(mockSignIn.Object, mockDB.Object);
            var vaccines = await contoller.GetVaccines();
            var notFound = Assert.IsType<NotFoundObjectResult>(vaccines);
            Assert.Equal("Data not found", notFound.Value.ToString());
        }
        [Fact]
        public async Task GetVaccinesReturnsBadRequest()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccines())
                .ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var contoller = new AdminController(mockSignIn.Object, mockDB.Object);
            var vaccines = await contoller.GetVaccines();
            var badRequst = Assert.IsType<BadRequestObjectResult>(vaccines);
            Assert.Equal("Something went wrong", badRequst.Value.ToString());
        }
        [Fact]
        public async Task GetVaccinesReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetVaccines())
                .ThrowsAsync(new ArgumentException());
            var contoller = new AdminController(mockSignIn.Object, mockDB.Object);
            var vaccines = await contoller.GetVaccines();
            var forbiddenResult = Assert.IsType<ObjectResult>(vaccines);
            Assert.Equal(403, forbiddenResult.StatusCode);
        }
        private List<VaccineResponse> Vaccines()
        {
            var list = new List<VaccineResponse>();
            list.Add(new VaccineResponse()
            {
                vaccineId = new Guid("452aa3ed-8154-438f-6dfe-08da3663531b"),
                company = "Pfeizer",
                name = "Pfeizer vaccine",
                numberOfDoses = 2,
                minDaysBetweenDoses = 30,
                maxDaysBetweenDoses = 0,
                virus = "Coronavirus",
                minPatientAge = 12,
                maxPatientAge = 0,
                active = true
            });
            list.Add(new VaccineResponse()
            {
                vaccineId = new Guid("093e8807-f30e-4d60-6e00-08da3663531b"),
                company = "Johnson and Johnson",
                name = "J&J vaccine",
                numberOfDoses = 1,
                minDaysBetweenDoses = 0,
                maxDaysBetweenDoses = 0,
                virus = "Coronavirus",
                minPatientAge = 18,
                maxPatientAge = 0,
                active = true
            });
            return list;
        }
        [Fact]
        public async Task DeleteVaccineReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteVaccine(vaccineID)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteVaccine(vaccineID);
            var okResult = Assert.IsType<OkObjectResult>(deleted);
            Assert.Equal("Deleted Vaccine", okResult.Value.ToString());
        }
        [Fact]
        public async Task DeleteVaccineReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteVaccine(vaccineID)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteVaccine(vaccineID);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(deleted);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task DeleteVaccineReturnsBadRequest()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteVaccine(vaccineID))
                .ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteVaccine(vaccineID);
            var badRequest = Assert.IsType<BadRequestObjectResult>(deleted);
            Assert.Equal("Something went wrong", badRequest.Value.ToString());
        }
        [Fact]
        public async Task DeleteVaccineReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteVaccine(vaccineID))
                .ThrowsAsync(new ArgumentException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteVaccine(vaccineID);
            var forbiddenResult = Assert.IsType<ObjectResult>(deleted);
            Assert.Equal(403, forbiddenResult.StatusCode);
        }
        [Fact]
        public async Task DeleteTimeSlotsReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteTimeSlots(listDeleteTimeSlots)).ReturnsAsync(true);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteTimeSlots(listDeleteTimeSlots);
            var okResult = Assert.IsType<OkObjectResult>(deleted);
            Assert.Equal("Deleted time slots", okResult.Value.ToString());
        }
        [Fact]
        public async Task DeleteTimeSlotsReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteTimeSlots(listDeleteTimeSlots)).ReturnsAsync(false);
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteTimeSlots(listDeleteTimeSlots);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(deleted);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task DeleteTimeSlotsReturnsBadRequest()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteTimeSlots(listDeleteTimeSlots))
                .ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteTimeSlots(listDeleteTimeSlots);
            var badRequest = Assert.IsType<BadRequestObjectResult>(deleted);
            Assert.Equal("Something went wrong", badRequest.Value.ToString());
        }
        [Fact]
        public async Task DeleteTimeSlotsReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.DeleteTimeSlots(listDeleteTimeSlots))
                .ThrowsAsync(new ArgumentException());
            var controller = new AdminController(mockSignIn.Object, mockDB.Object);
            var deleted = await controller.DeleteTimeSlots(listDeleteTimeSlots);
            var forbiddenResult = Assert.IsType<ObjectResult>(deleted);
            Assert.Equal(403, forbiddenResult.StatusCode);
        }
    }
}