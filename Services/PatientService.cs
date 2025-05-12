using System;
using System.Collections.Generic;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Services
{
    /// <summary>
    /// 환자 관련 비즈니스 로직을 처리하는 서비스 클래스
    /// </summary>
    public class PatientService
    {
        private readonly DataService _dataService;

        /// <summary>
        /// 생성자
        /// </summary>
        public PatientService()
        {
            _dataService = new DataService();
        }

        /// <summary>
        /// 모든 환자 목록 조회
        /// </summary>
        public List<Patient> GetAllPatients()
        {
            return _dataService.GetAllPatients();
        }

        /// <summary>
        /// 환자 ID로 조회
        /// </summary>
        public Patient GetPatientById(int id)
        {
            return _dataService.GetPatientById(id);
        }

        /// <summary>
        /// 환자 검색
        /// </summary>
        public List<Patient> SearchPatients(string searchText)
        {
            return _dataService.SearchPatients(searchText);
        }

        /// <summary>
        /// 환자 추가
        /// </summary>
        public void AddPatient(Patient patient)
        {
            // 필수 필드 유효성 검사
            if (string.IsNullOrWhiteSpace(patient.FirstName) ||
                string.IsNullOrWhiteSpace(patient.LastName) ||
                patient.DateOfBirth == DateTime.MinValue)
            {
                throw new ArgumentException("환자 이름과 생년월일은 필수 입력 사항입니다.");
            }

            // 환자 나이 제한 검사 (150세 이하)
            if (patient.Age > 150)
            {
                throw new ArgumentException("유효하지 않은 생년월일입니다.");
            }

            // 기본 값 설정
            if (patient.RegistrationDate == DateTime.MinValue)
            {
                patient.RegistrationDate = DateTime.Now;
            }

            _dataService.AddPatient(patient);
        }

        /// <summary>
        /// 환자 정보 수정
        /// </summary>
        public bool UpdatePatient(Patient patient)
        {
            // 필수 필드 유효성 검사
            if (string.IsNullOrWhiteSpace(patient.FirstName) ||
                string.IsNullOrWhiteSpace(patient.LastName) ||
                patient.DateOfBirth == DateTime.MinValue)
            {
                throw new ArgumentException("환자 이름과 생년월일은 필수 입력 사항입니다.");
            }

            // 환자 나이 제한 검사 (150세 이하)
            if (patient.Age > 150)
            {
                throw new ArgumentException("유효하지 않은 생년월일입니다.");
            }

            return _dataService.UpdatePatient(patient);
        }

        /// <summary>
        /// 환자 삭제
        /// </summary>
        public bool DeletePatient(int id)
        {
            // 환자가 존재하는지 확인
            var patient = _dataService.GetPatientById(id);
            if (patient == null)
            {
                return false;
            }

            return _dataService.DeletePatient(id);
        }

        /// <summary>
        /// 환자의 예약 목록 조회
        /// </summary>
        public List<Appointment> GetPatientAppointments(int patientId)
        {
            return _dataService.GetAppointmentsByPatientId(patientId);
        }

        /// <summary>
        /// 이름으로 환자 검색
        /// </summary>
        public List<Patient> SearchPatientsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Patient>();
            }

            return _dataService.SearchPatients(name);
        }

        /// <summary>
        /// 오늘 등록된 환자 수 조회
        /// </summary>
        public int GetTodayNewPatientsCount()
        {
            var today = DateTime.Today;
            var patients = _dataService.GetAllPatients();

            return patients.Count(p => p.RegistrationDate.Date == today);
        }

        /// <summary>
        /// 환자 전체 수 조회
        /// </summary>
        public int GetTotalPatientsCount()
        {
            return _dataService.GetAllPatients().Count;
        }
    }
}