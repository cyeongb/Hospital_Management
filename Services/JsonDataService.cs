using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Services
{
    /// <summary>
    /// JSON 파일에서 데이터를 읽고 쓰는 서비스 클래스
    /// </summary>
    public class JsonDataService
    {
        private string _patientsFilePath;
        private string _doctorsFilePath;
        private string _appointmentsFilePath;

        private List<Patient> _patients;
        private List<Doctor> _doctors;
        private List<Appointment> _appointments;

        // ID 자동 증가를 위한 카운터
        private int _nextPatientId = 1;
        private int _nextDoctorId = 1;
        private int _nextAppointmentId = 1;

        /// <summary>
        /// 생성자 - 파일 경로 설정 및 데이터 로드
        /// </summary>
        public JsonDataService()
        {
            // 파일 경로 설정 (현재 실행 디렉토리 기준)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _patientsFilePath = Path.Combine(baseDir, "Data", "SamplePatients.json");
            _doctorsFilePath = Path.Combine(baseDir, "Data", "SampleDoctors.json");
            _appointmentsFilePath = Path.Combine(baseDir, "Data", "SampleAppointments.json");

            // 데이터 폴더가 없으면 생성
            Directory.CreateDirectory(Path.Combine(baseDir, "Data"));

            // 데이터 로드
            LoadData();
        }

        /// <summary>
        /// JSON 파일에서 데이터 로드
        /// </summary>
        private void LoadData()
        {
            try
            {
                // 환자 데이터 로드
                if (File.Exists(_patientsFilePath))
                {
                    string patientsJson = File.ReadAllText(_patientsFilePath);
                    _patients = JsonConvert.DeserializeObject<List<Patient>>(patientsJson);

                    // 다음 ID 설정
                    if (_patients != null && _patients.Count > 0)
                    {
                        _nextPatientId = _patients.Max(p => p.PatientId) + 1;
                    }
                    else
                    {
                        _patients = new List<Patient>();
                    }
                }
                else
                {
                    _patients = new List<Patient>();
                }

                // 의사 데이터 로드
                if (File.Exists(_doctorsFilePath))
                {
                    string doctorsJson = File.ReadAllText(_doctorsFilePath);
                    _doctors = JsonConvert.DeserializeObject<List<Doctor>>(doctorsJson);

                    // 다음 ID 설정
                    if (_doctors != null && _doctors.Count > 0)
                    {
                        _nextDoctorId = _doctors.Max(d => d.DoctorId) + 1;
                    }
                    else
                    {
                        _doctors = new List<Doctor>();
                    }
                }
                else
                {
                    _doctors = new List<Doctor>();
                }

                // 예약 데이터 로드
                if (File.Exists(_appointmentsFilePath))
                {
                    string appointmentsJson = File.ReadAllText(_appointmentsFilePath);
                    _appointments = JsonConvert.DeserializeObject<List<Appointment>>(appointmentsJson);

                    // 다음 ID 설정
                    if (_appointments != null && _appointments.Count > 0)
                    {
                        _nextAppointmentId = _appointments.Max(a => a.AppointmentId) + 1;
                    }
                    else
                    {
                        _appointments = new List<Appointment>();
                    }
                }
                else
                {
                    _appointments = new List<Appointment>();
                }

                // 관계 설정
                foreach (var appointment in _appointments)
                {
                    // 환자와 의사 객체 설정
                    appointment.Patient = _patients.FirstOrDefault(p => p.PatientId == appointment.PatientId);
                    appointment.Doctor = _doctors.FirstOrDefault(d => d.DoctorId == appointment.DoctorId);

                    // 양방향 관계 설정
                    if (appointment.Patient != null)
                    {
                        if (appointment.Patient.Appointments == null)
                        {
                            appointment.Patient.Appointments = new List<Appointment>();
                        }
                        appointment.Patient.Appointments.Add(appointment);
                    }

                    if (appointment.Doctor != null)
                    {
                        if (appointment.Doctor.Appointments == null)
                        {
                            appointment.Doctor.Appointments = new List<Appointment>();
                        }
                        appointment.Doctor.Appointments.Add(appointment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 로드 중 오류 발생: {ex.Message}");

                // 실패 시 빈 리스트로 초기화
                _patients = new List<Patient>();
                _doctors = new List<Doctor>();
                _appointments = new List<Appointment>();
            }
        }

        /// <summary>
        /// 데이터를 JSON 파일로 저장
        /// </summary>
        private void SaveData()
        {
            try
            {
                // 환자 데이터 저장
                string patientsJson = JsonConvert.SerializeObject(_patients, Formatting.Indented);
                File.WriteAllText(_patientsFilePath, patientsJson);

                // 의사 데이터 저장
                string doctorsJson = JsonConvert.SerializeObject(_doctors, Formatting.Indented);
                File.WriteAllText(_doctorsFilePath, doctorsJson);

                // 양방향 참조를 일시적으로 제거하여 직렬화 시 순환 참조 방지
                foreach (var appointment in _appointments)
                {
                    appointment.Patient = null;
                    appointment.Doctor = null;
                }

                // 예약 데이터 저장
                string appointmentsJson = JsonConvert.SerializeObject(_appointments, Formatting.Indented);
                File.WriteAllText(_appointmentsFilePath, appointmentsJson);

                // 관계 복원
                foreach (var appointment in _appointments)
                {
                    appointment.Patient = _patients.FirstOrDefault(p => p.PatientId == appointment.PatientId);
                    appointment.Doctor = _doctors.FirstOrDefault(d => d.DoctorId == appointment.DoctorId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 저장 중 오류 발생: {ex.Message}");
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
                (p.FirstName != null && p.FirstName.Contains(searchText)) ||
                (p.LastName != null && p.LastName.Contains(searchText)) ||
                (p.FullName != null && p.FullName.Contains(searchText)) ||
                (p.PhoneNumber != null && p.PhoneNumber.Contains(searchText)) ||
                (p.Email != null && p.Email.Contains(searchText)) ||
                (p.Address != null && p.Address.Contains(searchText))
            ).ToList();
        }

        /// <summary>
        /// 환자 추가
        /// </summary>
        public void AddPatient(Patient patient)
        {
            patient.PatientId = _nextPatientId++;

            if (patient.Appointments == null)
            {
                patient.Appointments = new List<Appointment>();
            }

            _patients.Add(patient);
            SaveData();
        }

        /// <summary>
        /// 환자 정보 수정
        /// </summary>
        public bool UpdatePatient(Patient patient)
        {
            var existingPatient = GetPatientById(patient.PatientId);
            if (existingPatient == null)
                return false;

            // 기존 환자의 예약 정보 보존
            patient.Appointments = existingPatient.Appointments;

            var index = _patients.IndexOf(existingPatient);
            _patients[index] = patient;

            SaveData();
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

            bool result = _patients.Remove(patient);
            SaveData();

            return result;
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
                (d.FirstName != null && d.FirstName.Contains(searchText)) ||
                (d.LastName != null && d.LastName.Contains(searchText)) ||
                (d.FullName != null && d.FullName.Contains(searchText)) ||
                (d.Specialization != null && d.Specialization.Contains(searchText)) ||
                (d.PhoneNumber != null && d.PhoneNumber.Contains(searchText)) ||
                (d.Email != null && d.Email.Contains(searchText))
            ).ToList();
        }

        /// <summary>
        /// 의사 추가
        /// </summary>
        public void AddDoctor(Doctor doctor)
        {
            doctor.DoctorId = _nextDoctorId++;

            if (doctor.Appointments == null)
            {
                doctor.Appointments = new List<Appointment>();
            }

            _doctors.Add(doctor);
            SaveData();
        }

        /// <summary>
        /// 의사 정보 수정
        /// </summary>
        public bool UpdateDoctor(Doctor doctor)
        {
            var existingDoctor = GetDoctorById(doctor.DoctorId);
            if (existingDoctor == null)
                return false;

            // 기존 의사의 예약 정보 보존
            doctor.Appointments = existingDoctor.Appointments;

            var index = _doctors.IndexOf(existingDoctor);
            _doctors[index] = doctor;

            SaveData();
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

            bool result = _doctors.Remove(doctor);
            SaveData();

            return result;
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

            if (appointment.CreatedAt == DateTime.MinValue)
            {
                appointment.CreatedAt = DateTime.Now;
            }

            _appointments.Add(appointment);

            // 환자와 의사 객체 설정
            appointment.Patient = GetPatientById(appointment.PatientId);
            appointment.Doctor = GetDoctorById(appointment.DoctorId);

            // 양방향 관계 설정
            if (appointment.Patient != null)
            {
                if (appointment.Patient.Appointments == null)
                {
                    appointment.Patient.Appointments = new List<Appointment>();
                }
                appointment.Patient.Appointments.Add(appointment);
            }

            if (appointment.Doctor != null)
            {
                if (appointment.Doctor.Appointments == null)
                {
                    appointment.Doctor.Appointments = new List<Appointment>();
                }
                appointment.Doctor.Appointments.Add(appointment);
            }

            SaveData();
        }

        /// <summary>
        /// 예약 정보 수정
        /// </summary>
        public bool UpdateAppointment(Appointment appointment)
        {
            var existingAppointment = GetAppointmentById(appointment.AppointmentId);
            if (existingAppointment == null)
                return false;

            // 기존 예약에서 참조 제거
            if (existingAppointment.Patient != null)
            {
                existingAppointment.Patient.Appointments.Remove(existingAppointment);
            }

            if (existingAppointment.Doctor != null)
            {
                existingAppointment.Doctor.Appointments.Remove(existingAppointment);
            }

            var index = _appointments.IndexOf(existingAppointment);
            _appointments[index] = appointment;

            // 환자와 의사 객체 설정
            appointment.Patient = GetPatientById(appointment.PatientId);
            appointment.Doctor = GetDoctorById(appointment.DoctorId);

            // 양방향 관계 설정
            if (appointment.Patient != null)
            {
                if (appointment.Patient.Appointments == null)
                {
                    appointment.Patient.Appointments = new List<Appointment>();
                }
                appointment.Patient.Appointments.Add(appointment);
            }

            if (appointment.Doctor != null)
            {
                if (appointment.Doctor.Appointments == null)
                {
                    appointment.Doctor.Appointments = new List<Appointment>();
                }
                appointment.Doctor.Appointments.Add(appointment);
            }

            SaveData();
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

            bool result = _appointments.Remove(appointment);
            SaveData();

            return result;
        }

        #endregion
    }
}