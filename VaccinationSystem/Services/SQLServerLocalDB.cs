using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Models;
using VaccinationSystem.Data;
using Microsoft.EntityFrameworkCore;
using VaccinationSystem.DTOs;

namespace VaccinationSystem.Services
{
    public class SQLServerLocalDB:IDatabase
    {
        private AppDBContext dbContext;
        public SQLServerLocalDB(AppDBContext context)
        {
            dbContext = context;
        }
        public async Task<Patient> GetPatient(int id)
        {
            return await dbContext.Patients.FindAsync(id);
        }
        public async Task<List<Patient>> GetPatients()
        {
            return await dbContext.Patients.ToListAsync();
        }
        public async Task<List<DoctorResponse>> GetDoctors()
        {
            var doctors =  dbContext.Doctors.Include(d => d.vaccinationCenter).ToList();
            var doctorsResponse = new List<DoctorResponse>();
            VaccinationCenter center;
            DoctorResponse dR;
            foreach (var doctor in doctors)
            {
                    center = await dbContext.VaccinationCenters.FindAsync(doctor.vaccinationCenter.id);
                    dR = new DoctorResponse()
                    {
                        id = doctor.id,
                        pesel = doctor.pesel,
                        firstName = doctor.firstName,
                        lastName = doctor.lastName,
                        dateOfBirth = doctor.dateOfBirth,
                        mail = doctor.mail,
                        phoneNumber = doctor.phoneNumber,
                        active = doctor.active,
                        vaccinationCenterId = doctor.vaccinationCenter.id,
                        city = center.city,
                        name = center.name,
                        street = center.address,
                    };
                    doctorsResponse.Add(dR);
            }
            return doctorsResponse;
        }
        
        public void AddPatient(RegisteringPatient patient)
        {
            Patient p = new Patient
            {
                pesel = patient.PESEL,
                firstName = patient.Names,
                lastName = patient.Password,
                mail = patient.Mail,
                password = patient.Password,
                phoneNumber = patient.PhoneNumber,
                active = true,
                certificates = { },
                vaccinationHistory = { },
                futureVaccinations = { }
            };


            dbContext.Patients.Add(p);
            dbContext.SaveChanges();

        }

        public Guid AreCredentialsValid(Login login)
        {
            var patient = dbContext.Patients.Where(p => p.mail.CompareTo(login.mail)==0).FirstOrDefault();
            if (patient != null && patient.password.CompareTo(login.password) == 0)
                return patient.id;

            return Guid.Empty;
        }

        public bool IsUserInDatabase(string email)
        {
            int emailOccurance = dbContext.Patients.Where(p => p.mail.CompareTo(email)==0).Count();

            if (emailOccurance > 0)
                return true;
            
            return false;

        }

        public async Task<List<VaccinationCenterResponse>> GetVaccinationCenters()
        {
            var centers = await dbContext.VaccinationCenters.ToListAsync();
            var centersToReturn = new List<VaccinationCenterResponse>();

            foreach(var center in centers)
            {
                var vaccines = await GetVaccinesFromVaccinationCenter(center.id);
                var hours = await GetOpeningHoursFromVaccinationCenter(center.id);

                var vC = new VaccinationCenterResponse()
                {
                    Id = center.id,
                    Name = center.name,
                    City = center.city,
                    Street = center.address,
                    Vaccines = vaccines,
                    Active = center.active,
                    OpeningHoursDays = hours.ToArray(),
                };

                centersToReturn.Add(vC);
                
            }

            return centersToReturn;
        }

        public async Task<bool> EditVaccinationCenter(EditedVaccinationCenter center)
        {

            var dbCenter = await dbContext.VaccinationCenters.SingleOrDefaultAsync(c => c.id == center.Id);

            if (dbCenter != null)
            {
                dbCenter.name = center.Name;
                dbCenter.city = center.City;
                dbCenter.address = center.Street;
                dbCenter.active = center.Active;

                var vaccines = dbContext.VaccinesInCenters.Where(w => w.vaccinationCenter.id == dbCenter.id);
                dbContext.VaccinesInCenters.RemoveRange(vaccines);

                foreach (var vId in center.VaccineIds)
                {
                    Vaccine vaccine = await GetVaccine(vId);
                    dbContext.VaccinesInCenters.Add(new VaccinesInCenters
                    {
                        vaccine = vaccine,
                        vaccinationCenter = dbCenter,
                    });
                }

                var hours = dbContext.OpeningHours.Where(h => h.vaccinationCenter.id == dbCenter.id);
                foreach(var h in hours)
                {
                    dbContext.OpeningHours.Remove(h);
                }

                int dayOfWeek = 0;
                foreach(var h in center.openingHoursDays)
                {
                    dbContext.OpeningHours.Add(new OpeningHours()
                    {
                        from = TimeSpan.Parse(h.from),
                        to = TimeSpan.Parse(h.to),
                        vaccinationCenter = dbCenter,
                        day = (WeekDay)dayOfWeek
                    }) ;

                    dayOfWeek++;
                }

                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteVaccinationCenter(Guid vaccinationCenterId)
        {
            Console.WriteLine(vaccinationCenterId);
            var dbCenter = await dbContext.VaccinationCenters.SingleAsync(c => c.id == vaccinationCenterId);

            if(dbCenter!=null)
            {
                var vaccines = dbContext.VaccinesInCenters.Where(w => w.vaccinationCenter.id == dbCenter.id);
                dbContext.VaccinesInCenters.RemoveRange(vaccines);

                var hours = dbContext.OpeningHours.Where(h => h.vaccinationCenter.id == dbCenter.id);
                dbContext.OpeningHours.RemoveRange(hours);

                await dbContext.Doctors.Where(d => d.vaccinationCenter.id == dbCenter.id)
                    .ForEachAsync(d => d.vaccinationCenter = null);

                dbContext.VaccinationCenters.Remove(dbCenter);

                await dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<Vaccine> GetVaccine(Guid vaccineId)
        {
            var vaccine = await dbContext.Vaccines.SingleAsync(v => v.id == vaccineId);

            return vaccine;
        }

        public async Task<List<Vaccine>> GetVaccinesFromVaccinationCenter(Guid vaccinationCenterId)
        {
            var vaccines = await dbContext.VaccinesInCenters.Where(vic => vic.vaccinationCenter.id == vaccinationCenterId).Select(v=>v.vaccine).ToListAsync();

            return vaccines;
        }

        public async Task<List<OpeningHoursDays>> GetOpeningHoursFromVaccinationCenter(Guid vaccinationCenterId)
        {
            var hours = await dbContext.OpeningHours.Where(h => h.vaccinationCenter.id == vaccinationCenterId)
                .Select(h => new OpeningHoursDays()
                {
                    from = $"{h.from.Hours}:{h.from.Minutes}",
                    to = $"{h.to.Hours}:{h.to.Minutes}"
                }).ToListAsync();

            return hours;
        }

        public async Task<List<Doctor>> GetDoctorsFromVaccinationCenter(Guid vaccinationCenterId)
        {
            var doctors = await dbContext.Doctors.Where(d => d.vaccinationCenter.id == vaccinationCenterId).ToListAsync();

            return doctors;
        }

        public async Task AddVaccinationCenter(AddVaccinationCenterRequest center)
        {
            var vC = new VaccinationCenter
            {
                name = center.Name,
                city = center.City,
                address = center.Street,
                active = true
            };

            var hours = new OpeningHours[7];

            int dayOfWeek = 0;
            foreach (var h in center.openingHoursDays)
            {
                dbContext.OpeningHours.Add(new OpeningHours()
                {
                    from = TimeSpan.Parse(h.from),
                    to = TimeSpan.Parse(h.to),
                    vaccinationCenter = vC,
                    day = (WeekDay)dayOfWeek
                });

                dayOfWeek++;

            }
            

            foreach (var vId in center.VaccineIds)
            {
                Vaccine vaccine = await GetVaccine(vId);
                dbContext.VaccinesInCenters.Add(new VaccinesInCenters
                {
                    vaccine = vaccine,
                    vaccinationCenter = vC,
                });
            }

            dbContext.VaccinationCenters.Add(vC);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> EditPatient(EditedPatient patient)
        {
            var dbPatient = await dbContext.Patients.SingleAsync(pat => pat.id == patient.id);
            if (dbPatient != null)
            {
                dbPatient.dateOfBirth = patient.dateOfBirth;
                dbPatient.firstName = patient.firstName;
                dbPatient.lastName = patient.lastName;
                dbPatient.mail = patient.mail;
                dbPatient.pesel = patient.pesel;
                dbPatient.phoneNumber = patient.phoneNumber;
                dbPatient.active = patient.active;

                return true;
            }
            return false;
        }

        public async Task<bool> DeletePatient(Guid patientId)
        {
            var dbPatient = await dbContext.Patients.SingleAsync(patient => patient.id == patientId);
            if (dbPatient != null)
            {
                var counts = dbContext.VaccinationCounts.Where(c => c.patient.id == patientId);
                dbContext.VaccinationCounts.RemoveRange(counts);
                var appointments = dbContext.Appointments.Where(c => c.patient.id == patientId);
                dbContext.Appointments.RemoveRange(appointments);
                var certificates = dbContext.Certificates.Where(c => c.patientId == patientId);
                dbContext.Certificates.RemoveRange(certificates);
                dbContext.Patients.Remove(dbPatient);
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> AddDoctor(RegisteringDoctor doctor)
        {
            var patient = await dbContext.Patients.SingleAsync(patient => patient.pesel == doctor.pesel);
            var center = await dbContext.VaccinationCenters.SingleAsync(center => center.id == doctor.vaccinationCenterId);
            Doctor doc = new Doctor
            {
                pesel = doctor.pesel,
                firstName = doctor.firstName,
                lastName = doctor.lastName,
                dateOfBirth = doctor.dateOfBirth,
                mail = doctor.mail,
                password = doctor.password,
                phoneNumber = doctor.phoneNumber,
                vaccinationCenter = center,
                active = true,
                vaccinationsArchive = { },
                futureVaccinations = { },
                patientAccount = patient
            };

            dbContext.Doctors.Add(doc);
            var saved = dbContext.SaveChanges();
            if (saved > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> EditDoctor(EditedDoctor doctor)
        {
            var dbDoctor = await dbContext.Doctors.SingleAsync(doc => doc.id == doctor.id);
            var center = await dbContext.VaccinationCenters.SingleAsync(center => center.id == doctor.vaccinationCenterId);
            if (dbDoctor != null)
            {
                dbDoctor.pesel = doctor.pesel;
                dbDoctor.firstName = doctor.firstName;
                dbDoctor.lastName = doctor.lastName;
                dbDoctor.vaccinationCenter = center;
                dbDoctor.dateOfBirth = doctor.dateOfBirth;
                dbDoctor.mail = doctor.mail;
                dbDoctor.phoneNumber = doctor.phoneNumber;

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteDoctor(Guid doctorId)
        {
            var dbDoctor = await dbContext.Doctors.SingleAsync(doc => doc.id == doctorId);
            if (dbDoctor != null)
            {
                var appointments = dbContext.Appointments.Where(d => d.timeSlot.doctor.id == doctorId);
                dbContext.Appointments.RemoveRange(appointments);
                var times = dbContext.TimeSlots.Where(t => t.doctor.id == doctorId);
                dbContext.TimeSlots.RemoveRange(times);
                //var center = dbContext.VaccinationCenters.Where(c => c.id == dbDoctor.vaccinationCenter.id);

                dbContext.Doctors.Remove(dbDoctor);
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}