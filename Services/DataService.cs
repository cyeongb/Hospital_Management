using System;
using System.Collections.Generic;
using System.Linq;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Services
{
    /// <summary>
    /// 데이터베이스 접근 서비스
    /// 실제 구현에서는 JsonDataService를 호출하도록 수정
    /// </summary>
    public class DataService
    {
        private readonly JsonDataService _jsonDataService;

        /// <summary>
        /// 생성자
        /// </summary>
        public DataService()
        {
            _jsonDataService = new JsonDataService();
        }

        #region 환자 관련 메서드

        /// <summary>
        /// 모든 환자 조회
        /// </summary>
        public List<Patient> GetAllPatients()
        {
            return _jsonDataService.GetAllPatients();
        }

        /// <summary>
        /// 환자 ID로 조회
        /// </summary>
        public Patient GetPatientById(int id)
        {
            return _jsonDataService.GetPatientById(id);
        }

        /// <summary>
        /// 환자 검색
        /// </summary>
        public List<Patient> SearchPatients(string searchText)
        {
            return _jsonDataService.SearchPatients(searchText);
        }

        /// <summary>
        /// 환자 추가
        /// </summary>
        public void AddPatient(Patient patient)
        {
            _jsonDataService.AddPatient(patient);
        }

        /// <summary>
        /// 환자 정보 수정
        /// </summary>
        public bool UpdatePatient(Patient patient)
        {
            return _jsonDataService.UpdatePatient(patient);
        }

        /// <summary>
        /// 환자 삭제
        /// </summary>
        public bool DeletePatient(int id)
        {
            return _jsonDataService.DeletePatient(id);
        }

        #endregion

        #region 의사 관련 메서드

        /// <summary>
        /// 모든 의사 조회
        /// </summary>
        public List<Doctor> GetAllDoctors()
        {
            return _jsonDataService.GetAllDoctors();
        }

        /// <summary>
        /// 의사 ID로 조회
        /// </summary>
        public Doctor GetDoctorById(int id)
        {
            return _jsonDataService.GetDoctorById(id);
        }

        /// <summary>
        /// 의사 검색
        /// </summary>
        public List<Doctor> SearchDoctors(string searchText)
        {
            return _jsonDataService.SearchDoctors(searchText);
        }

        /// <summary>
        /// 의사 추가
        /// </summary>
        public void AddDoctor(Doctor doctor)
        {
            _jsonDataService.AddDoctor(doctor);
        }

        /// <summary>
        /// 의사 정보 수정
        /// </summary>
        public bool UpdateDoctor(Doctor doctor)
        {
            return _jsonDataService.UpdateDoctor(doctor);
        }

        /// <summary>
        /// 의사 삭제
        /// </summary>
        public bool DeleteDoctor(int id)
        {
            return _jsonDataService.DeleteDoctor(id);
        }

        #endregion

        #region 예약 관련 메서드

        /// <summary>
        /// 모든 예약 조회
        /// </summary>
        public List<Appointment> GetAllAppointments()
        {
            return _jsonDataService.GetAllAppointments();
        }

        /// <summary>
        /// 예약 ID로 조회
        /// </summary>
        public Appointment GetAppointmentById(int id)
        {
            return _jsonDataService.GetAppointmentById(id);
        }

        /// <summary>
        /// 환자 ID로 예약 조회
        /// </summary>
        public List<Appointment> GetAppointmentsByPatientId(int patientId)
        {
            return _jsonDataService.GetAppointmentsByPatientId(patientId);
        }

        /// <summary>
        /// 의사 ID로 예약 조회
        /// </summary>
        public List<Appointment> GetAppointmentsByDoctorId(int doctorId)
        {
            return _jsonDataService.GetAppointmentsByDoctorId(doctorId);
        }

        /// <summary>
        /// 날짜별 예약 조회
        /// </summary>
        public List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            return _jsonDataService.GetAppointmentsByDate(date);
        }

        /// <summary>
        /// 예약 추가
        /// </summary>
        public void AddAppointment(Appointment appointment)
        {
            _jsonDataService.AddAppointment(appointment);
        }

        /// <summary>
        /// 예약 정보 수정
        /// </summary>
        public bool UpdateAppointment(Appointment appointment)
        {
            return _jsonDataService.UpdateAppointment(appointment);
        }

        /// <summary>
        /// 예약 삭제
        /// </summary>
        public bool DeleteAppointment(int id)
        {
            return _jsonDataService.DeleteAppointment(id);
        }

        #endregion
    }
}