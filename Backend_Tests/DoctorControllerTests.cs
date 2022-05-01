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
using VaccinationSystem.Contollers;

namespace Backend_Tests
{
    public class DoctorControllerTests
    {
        private Guid timeSlotID = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE");
        private Guid timeSlotID2 = new Guid("55A2BBCE-E031-4931-E751-08DA13EF87A5");
        private Guid doctorID = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE");
        private Guid appointmentID = new Guid("33E18E13-8F45-4766-A0C0-08DA13EF5847");
        private string url = "jakistamurl";
        [Fact]
        public async Task GetTimeSlotsReturnsTImeSlots()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetTimeSlots(doctorID)).ReturnsAsync(GetTimeSlots);
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var slots = await controller.GetTimeSlots(doctorID);


            var okResult = Assert.IsType<OkObjectResult>(slots);


            var returnValue = Assert.IsType<List<TimeSlotsResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

        }

        [Fact]
        public async Task GetTimeSlotsReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetTimeSlots(doctorID)).ReturnsAsync(new List<TimeSlotsResponse>());
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.GetTimeSlots(doctorID);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(timeSlots);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }


        [Fact]
        public async Task GetTimeSlotsReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            mockDB.Setup(dB => dB.GetTimeSlots(doctorID)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.GetTimeSlots(doctorID);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(timeSlots);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task CreateTimeSlotsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slots = GetCreateNewVisitRequest();
            mockDB.Setup(dB => dB.CreateTimeSlots(doctorID, slots));
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var result = await controller.CreateTimeSlots(doctorID, slots);

            Assert.IsType<OkObjectResult>(result);

        }

        [Fact]
        public async Task CreateTimeSlotsReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slots = GetCreateNewVisitRequest();
            mockDB.Setup(dB => dB.CreateTimeSlots(doctorID, slots)).ThrowsAsync(new ArgumentException());
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.CreateTimeSlots(doctorID, slots);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(timeSlots);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }


        [Fact]
        public async Task CreateTimeSlotsReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slots = GetCreateNewVisitRequest();
            mockDB.Setup(dB => dB.CreateTimeSlots(doctorID, slots)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.CreateTimeSlots(doctorID, slots);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(timeSlots);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task CreateTimeSLotsReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slots = GetCreateNewVisitRequest();
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.CreateTimeSlots(doctorID, slots);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EditTimeSlotOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slot = GetEditedTimeSlot();
            mockDB.Setup(dB => dB.EditTimeSlot(doctorID, timeSlotID, slot)).ReturnsAsync(() => true);
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var result = await controller.ModifyTimeSlot(doctorID, timeSlotID, slot);

            Assert.IsType<OkObjectResult>(result);

        }

        [Fact]
        public async Task EditTimeSlotReturnsForbidden()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slot = GetEditedTimeSlot();
            mockDB.Setup(dB => dB.EditTimeSlot(doctorID, timeSlotID, slot)).ThrowsAsync(new ArgumentException());
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.ModifyTimeSlot(doctorID, timeSlotID, slot);

            var result = Assert.IsType<ObjectResult>(timeSlots);
            Assert.Equal(403, result.StatusCode);
        }


        [Fact]
        public async Task EditTimeSlotReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slot = GetEditedTimeSlot();
            mockDB.Setup(dB => dB.EditTimeSlot(doctorID, timeSlotID, slot)).ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var timeSlots = await controller.ModifyTimeSlot(doctorID, timeSlotID, slot);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(timeSlots);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task EditTimeSotReturnsBadRequestInvalidModel()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var slot = GetEditedTimeSlot();
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);
            controller.ModelState.AddModelError("id", "Bad format");

            var result = await controller.ModifyTimeSlot(doctorID, timeSlotID, slot);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        private List<TimeSlotsResponse> GetTimeSlots()
        {
            var timeSlots = new List<TimeSlotsResponse>()
            {
                new TimeSlotsResponse()
                {
                    id = timeSlotID,
                    from = "2022-01-29T08:00",
                    to = "2022-01-29T09:00"
                },
                new TimeSlotsResponse()
                {
                    id = timeSlotID2,
                    from = "2022-01-29T09:00",
                    to = "2022-01-29T10:00"
                }
            };

            return timeSlots;
        }

        private CreateNewVisitRequest GetCreateNewVisitRequest()
        {
            return new CreateNewVisitRequest()
            {
                from = DateTime.Parse("2022-01-29T08:00"),
                to = DateTime.Parse("2022-01-29T09:00"),
                timeSlotDurationInMinutes = 15
            };
        }

        private EditedTimeSlot GetEditedTimeSlot()
        {
            return new EditedTimeSlot()
            {
                from = DateTime.Parse("2022-01-29T08:00"),
                to = DateTime.Parse("2022-01-29T09:00")
            };
        }
        [Fact]
        public async Task GetPatientReturnsPatient()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();

            var patient = GetPatient();

            mockDB.Setup(dB => dB.GetPatient(patient.id)).ReturnsAsync(GetPatient);
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

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
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object);

            var patientFromController = await controller.GetPatient(patient.id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(patientFromController);
            Assert.Equal("Data not found", notFoundResult.Value.ToString());
        }
        [Fact]
        public async Task CertifyReturnsOk()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var mockCertGen = new Mock<ICertificateGenerator>();

            mockDB.Setup(dB => dB.CreateCertificate(It.IsAny<Guid>(),It.IsAny<Guid>(),It.IsAny<string>())).ReturnsAsync(()=>true);
            mockDB.Setup(db => db.GetAppointment(appointmentID)).ReturnsAsync(GetAppointment);
            mockDB.Setup(db => db.GetDoctor(doctorID)).ReturnsAsync(GetDoctor);
            mockCertGen.Setup(gen => gen.Generate(It.IsAny<string>(),It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(() => "https://abc.com");
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object, mockCertGen.Object);


            var result = await controller.Certify(doctorID, appointmentID);

            var okResult = Assert.IsType<OkObjectResult>(result);

        }
        
        [Fact]
        public async Task CertifyReturnsNotFound()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var mockCertGen = new Mock<ICertificateGenerator>();

            mockDB.Setup(dB => dB.CreateCertificate(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(() => false);
            mockDB.Setup(db => db.GetAppointment(appointmentID)).ReturnsAsync(GetAppointment);
            mockDB.Setup(db => db.GetDoctor(doctorID)).ReturnsAsync(GetDoctor);
            mockCertGen.Setup(gen => gen.Generate(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(() => "https://abc.com");
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object, mockCertGen.Object);


            var result = await controller.Certify(doctorID, appointmentID);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Data not found", notFoundResult.Value);
        }


        [Fact]
        public async Task CertifyReturnsBadRequestDatabaseException()
        {
            var mockDB = new Mock<IDatabase>();
            var mockSignIn = new Mock<IUserSignInManager>();
            var mockCertGen = new Mock<ICertificateGenerator>();

            mockDB.Setup(dB => dB.CreateCertificate(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new System.Data.DeletedRowInaccessibleException());
            mockDB.Setup(db => db.GetAppointment(appointmentID)).ReturnsAsync(GetAppointment);
            mockDB.Setup(db => db.GetDoctor(doctorID)).ReturnsAsync(GetDoctor);
            mockCertGen.Setup(gen => gen.Generate(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(() => "https://abc.com");
            var controller = new DoctorController(mockSignIn.Object, mockDB.Object, mockCertGen.Object);


            var result = await controller.Certify(doctorID, appointmentID);

            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something went wrong", notFoundResult.Value.ToString());
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

        private Appointment GetAppointment()
        {
            return new Appointment()
            {
                id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                whichDose = 1,
                timeSlot = It.IsAny<TimeSlot>(),
                patient = new Patient
                {
                    id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                    pesel = "82121211111",
                    dateOfBirth = new DateTime(1982, 12, 12),
                    firstName = "Jan",
                    lastName = "Nowak",
                    mail = "j.nowak@mail.com",
                    phoneNumber = "+48555221331",
                    password = "password123()",
                    active = true
                },
                vaccine = new Vaccine
                {
                    id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                    company = "Moderna",
                    name = "Moderna vaccine",
                    numberOfDoses = 2,
                    minDaysBetweenDoses = 30,
                    minPatientAge = 18,
                    maxPatientAge = 99,
                    virus = Virus.Coronavirus,
                    active = true
                },
                state = AppointmentState.Finished,
                vaccineBatchNumber = "AB-123-nie-wiem"
            };
        }
        private Doctor GetDoctor()
        {
            return new Doctor
            {
                id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                pesel = "59062011333",
                dateOfBirth = new DateTime(1959, 06, 20),
                firstName = "Robert",
                lastName = "Weide",
                mail = "robert.b.weide@mail.com",
                phoneNumber = "+48125200331",
                password = "123abc!@#",
                patientAccount = It.IsAny<Patient>(),
                vaccinationCenter = new VaccinationCenter
                {
                    id = new Guid("255E18E1-8FF7-4766-A0C0-08DA13EF87AE"),
                    name = "Punkt Szczepień Populacyjnych",
                    city = "Warszawa",
                    address = "Żwirki i Wigury 95/97",
                    active = true
                },
                active = true
            };
        }



    }
}
