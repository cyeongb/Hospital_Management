using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Services
{
    /// <summary>
    /// 데이터베이스 접근 서비스
    /// Entity Framework를 사용하여 데이터 액세스 구현
    /// </summary>
    public class DataService
    {
        // 실제 프로젝트에서는 데이터베이스 컨텍스트를 사용하여 데이터 접근
        // 간단한 예제를 위해 메모리 내 데이터 사용
        private List<Patient> _patients;
        private List<Doctor> _doctors;
        private List<Appointment> _appointments;

        // ID 자동 증가를 위한 카운터
        private int _nextPatientId = 1;
        private int _nextDoctorId = 1;
        private int _nextAppointmentId = 1;

        /// <summary>
        /// 생성자 - 샘플 데이터 초기화
        /// </summary>
        public DataService()
        {
            InitializeSampleData();
        }

        /// <summary>
        /// 샘플 데이터 초기화
        /// </summary>
        private void InitializeSampleData()
        {
            // 환자 샘플 데이터
            _patients = new List<Patient>
            {
                new Patient
                {
                    PatientId = _nextPatientId++,
                    FirstName = "홍",
                    LastName = "길동",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Gender = "남",
                    PhoneNumber = "010-1234-5678",
                    Email = "gildong@example.com",
                    Address = "서울시 강남구 테헤란로 123",
                    InsuranceNumber = "12345-67890",
                    RegistrationDate = DateTime.Now.AddMonths(-6)
                },
                new Patient
                {
                    PatientId = _nextPatientId++,
                    FirstName = "김",
                    LastName = "영희",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    Gender = "여",
                    PhoneNumber = "010-9876-5432",
                    Email = "younghee@example.com",
                    Address = "서울시 서초구 반포대로 45",
                    InsuranceNumber = "09876-54321",
                    RegistrationDate = DateTime.Now.AddMonths(-3)
                },
                new Patient
                {
                    PatientId = _nextPatientId++,
                    FirstName = "이",
                    LastName = "철수",
                    DateOfBirth = new DateTime(1978, 3, 10),
                    Gender = "남",
                    PhoneNumber = "010-5555-6666",
                    Email = "chulsoo@example.com",
                    Address = "경기도 성남시 분당구 판교로 123",
                    InsuranceNumber = "55555-66666",
                    RegistrationDate = DateTime.Now.AddMonths(-1)
                }
            };

            // 의사 샘플 데이터
            _doctors = new List<Doctor>
            {
                new Doctor
                {
                    DoctorId = _nextDoctorId++,
                    FirstName = "박",
                    LastName = "진료",
                    Specialization = "내과",
                    LicenseNumber = "MC12345",
                    PhoneNumber = "010-1111-2222",
                    Email = "jinryo@hospital.com",
                    HireDate = DateTime.Now.AddYears(-5),
                    WorkingHours = "월-금 09:00-18:00"
                },
                new Doctor
                {
                    DoctorId = _nextDoctorId++,
                    FirstName = "최",
                    LastName = "외과",
                    Specialization = "외과",
                    LicenseNumber = "MC67890",
                    PhoneNumber = "010-3333-4444",
                    Email = "surgeon@hospital.com",
                    HireDate = DateTime.Now.AddYears(-3),
                    WorkingHours = "월-금 10:00-19:00"
                },
                new Doctor
                {
                    DoctorId = _nextDoctorId++,
                    FirstName = "정",
                    LastName = "소아",
                    Specialization = "소아과",
                    LicenseNumber = "MC24680",
                    PhoneNumber = "010-5555-7777",
                    Email = "pediatric@hospital.com",
                    HireDate = DateTime.Now.AddYears(-1),
                    WorkingHours = "화-토 09:00-18:00"
                }
            };

            // 예약 샘플 데이터
            _appointments = new List<Appointment>
            {
                new Appointment
                {
                    AppointmentId = _nextAppointmentId++,
                    PatientId = 1,
                    DoctorId = 1,
                    AppointmentDateTime = DateTime.Now.Date.AddDays(1).AddHours(10),
                    EndDateTime = DateTime.Now.Date.AddDays(1).AddHours(10).AddMinutes(30),
                    Purpose = "정기 검진",
                    Status = "예약됨",
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Appointment
                {
                    AppointmentId = _nextAppointmentId++,
                    PatientId = 2,
                    DoctorId = 3,
                    AppointmentDateTime = DateTime.Now.Date.AddDays(2).AddHours(14),
                    EndDateTime = DateTime.Now.Date.AddDays(2).AddHours(14).AddMinutes(45),
                    Purpose = "감기 증상",
                    Status = "예약됨",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new Appointment
                {
                    AppointmentId = _nextAppointmentId++,
                    PatientId = 3,
                    DoctorId = 2,
                    AppointmentDateTime = DateTime.Now.Date.AddHours(-3), // 오늘 이미 지난 시간
                    EndDateTime = DateTime.Now.Date.AddHours(-2).AddMinutes(30),
                    Purpose = "수술 후 상담",
                    Status = "완료됨",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    Notes = "경과 양호"
                }
            };

            // 관계 설정
            foreach (var appointment in _appointments)
            {
                // 환자와 의사 객체 설정
                appointment.Patient = _patients.FirstOrDefault(p => p.PatientId == appointment.PatientId);
                appointment.Doctor = _doctors.FirstOrDefault(d => d.DoctorId == appointment.DoctorId);

                // 양방향 관계 설정
                if (appointment.Patient != null)
                {
                    appointment.Patient.Appointments.Add(appointment);
                }

                if (appointment.Doctor != null)
                {
                    appointment.Doctor.Appointments.Add(appointment);
                }
            }
        }

        #region 환자 관련 메서드

        /// <summary>
        /// 모든 환자 조회
        /// </summary>
        public List<Patient> GetAllPatients()
        {
            return _patients.ToList();
        }

        /// <summary>
        /// 환자 ID로 조회
        /// </summary>
        public Patient GetPatientById(int id)
        {
            return _patients.FirstOrDefault(p => p.PatientId == id);
        }

        /// <summary>
        /// 환자 검색
        /// </summary>
        public List<Patient> SearchPatients(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllPatients();

            return _patients.Where(p =>
                p.FirstName.Contains(searchText) ||
                p.LastName.Contains(searchText) ||
                p.FullName.Contains(searchText) ||
                p.PhoneNumber.Contains(searchText) ||
                p.Email.Contains(searchText) ||
                p.Address.Contains(searchText)
            ).ToList();
        }

        /// <summary>
        /// 환자 추가
        /// </summary>
        public void AddPatient(Patient patient)
        {
            patient.PatientId = _nextPatientId++;
            _patients.Add(patient);
        }

        /// <summary>
        /// 환자 정보 수정
        /// </summary>
        public bool UpdatePatient(Patient patient)
        {
            var existingPatient = GetPatientById(patient.PatientId);
            if (existingPatient == null)
                return false;

            var index = _patients.IndexOf(existingPatient);
            _patients[index] = patient;
            return true;
        }

        /// <summary>
        /// 환자 삭제
        /// </summary>
        public bool DeletePatient(int id)
        {
            var patient = GetPatientById(id);
            if (patient == null)
                return false;

            // 환자와 관련된 예약도 삭제
            var appointmentsToRemove = _appointments.Where(a => a.PatientId == id).ToList();
            foreach (var appointment in appointmentsToRemove)
            {
                _appointments.Remove(appointment);
            }

            return _patients.Remove(patient);
        }

        #endregion

        #region 의사 관련 메서드

        /// <summary>
        /// 모든 의사 조회
        /// </summary>
        public List<Doctor> GetAllDoctors()
        {
            return _doctors.ToList();
        }

        /// <summary>
        /// 의사 ID로 조회
        /// </summary>
        public Doctor GetDoctorById(int id)
        {
            return _doctors.FirstOrDefault(d => d.DoctorId == id);
        }

        /// <summary>
        /// 의사 검색
        /// </summary>
        public List<Doctor> SearchDoctors(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllDoctors();

            return _doctors.Where(d =>
                d.FirstName.Contains(searchText) ||
                d.LastName.Contains(searchText) ||
                d.FullName.Contains(searchText) ||
                d.Specialization.Contains(searchText) ||
                d.PhoneNumber.Contains(searchText) ||
                d.Email.Contains(searchText)
            ).ToList();
        }

        /// <summary>
        /// 의사 추가
        /// </summary>
        public void AddDoctor(Doctor doctor)
        {
            doctor.DoctorId = _nextDoctorId++;
            _doctors.Add(doctor);
        }

        /// <summary>
        /// 의사 정보 수정
        /// </summary>
        public bool UpdateDoctor(Doctor doctor)
        {
            var existingDoctor = GetDoctorById(doctor.DoctorId);
            if (existingDoctor == null)
                return false;

            var index = _doctors.IndexOf(existingDoctor);
            _doctors[index] = doctor;
            return true;
        }

        /// <summary>
        /// 의사 삭제
        /// </summary>
        public bool DeleteDoctor(int id)
        {
            var doctor = GetDoctorById(id);
            if (doctor == null)
                return false;

            // 의사와 관련된 예약도 삭제
            var appointmentsToRemove = _appointments.Where(a => a.DoctorId == id).ToList();
            foreach (var appointment in appointmentsToRemove)
            {
                _appointments.Remove(appointment);
            }

            return _doctors.Remove(doctor);
        }

        #endregion

        #region 예약 관련 메서드

        /// <summary>
        /// 모든 예약 조회
        /// </summary>
        public List<Appointment> GetAllAppointments()
        {
            return _appointments.ToList();
        }

        /// <summary>
        /// 예약 ID로 조회
        /// </summary>
        public Appointment GetAppointmentById(int id)
        {
            return _appointments.FirstOrDefault(a => a.AppointmentId == id);
        }

        /// <summary>
        /// 환자 ID로 예약 조회
        /// </summary>
        public List<Appointment> GetAppointmentsByPatientId(int patientId)
        {
            return _appointments.Where(a => a.PatientId == patientId).ToList();
        }

        /// <summary>
        /// 의사 ID로 예약 조회
        /// </summary>
        public List<Appointment> GetAppointmentsByDoctorId(int doctorId)
        {
            return _appointments.Where(a => a.DoctorId == doctorId).ToList();
        }

        /// <summary>
        /// 날짜별 예약 조회
        /// </summary>
        public List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            return _appointments.Where(a => a.AppointmentDateTime.Date == date.Date).ToList();
        }

        /// <summary>
        /// 예약 추가
        /// </summary>
        public void AddAppointment(Appointment appointment)
        {
            appointment.AppointmentId = _nextAppointmentId++;

            // 환자와 의사 객체 설정
            appointment.Patient = GetPatientById(appointment.PatientId);
            appointment.Doctor = GetDoctorById(appointment.DoctorId);

            _appointments.Add(appointment);

            // 양방향 관계 설정
            if (appointment.Patient != null)
            {
                appointment.Patient.Appointments.Add(appointment);
            }

            if (appointment.Doctor != null)
            {
                appointment.Doctor.Appointments.Add(appointment);
            }
        }

        /// <summary>
        /// 예약 정보 수정
        /// </summary>
        public bool UpdateAppointment(Appointment appointment)
        {
            var existingAppointment = GetAppointmentById(appointment.AppointmentId);
            if (existingAppointment == null)
                return false;

            var index = _appointments.IndexOf(existingAppointment);

            // 기존 예약의 양방향 관계 제거
            if (existingAppointment.Patient != null)
            {
                existingAppointment.Patient.Appointments.Remove(existingAppointment);
            }

            if (existingAppointment.Doctor != null)
            {
                existingAppointment.Doctor.Appointments.Remove(existingAppointment);
            }

            // 새로운 예약 정보 적용
            _appointments[index] = appointment;

            // 환자와 의사 객체 설정
            appointment.Patient = GetPatientById(appointment.PatientId);
            appointment.Doctor = GetDoctorById(appointment.DoctorId);

            // 양방향 관계 재설정
            if (appointment.Patient != null)
            {
                appointment.Patient.Appointments.Add(appointment);
            }

            if (appointment.Doctor != null)
            {
                appointment.Doctor.Appointments.Add(appointment);
            }

            return true;
        }

        /// <summary>
        /// 예약 삭제
        /// </summary>
        public bool DeleteAppointment(int id)
        {
            var appointment = GetAppointmentById(id);
            if (appointment == null)
                return false;

            // 양방향 관계 제거
            if (appointment.Patient != null)
            {
                appointment.Patient.Appointments.Remove(appointment);
            }

            if (appointment.Doctor != null)
            {
                appointment.Doctor.Appointments.Remove(appointment);
            }

            return _appointments.Remove(appointment);
        }

        #endregion
    }
}