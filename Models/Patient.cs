using System;
using System.Collections.Generic;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// 환자 정보를 나타내는 모델 클래스
    /// </summary>
    public class Patient
    {
        /// <summary>
        /// 환자 고유 식별자
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// 환자 성
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 환자 이름
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 생년월일
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// 성별 (남/여)
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 연락처
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 이메일 주소
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 의료 보험 번호
        /// </summary>
        public string InsuranceNumber { get; set; }

        /// <summary>
        /// 알레르기 정보
        /// </summary>
        public string Allergies { get; set; }

        /// <summary>
        /// 등록일
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// 이 환자의 모든 예약 목록
        /// </summary>
        public List<Appointment> Appointments { get; set; }

        /// <summary>
        /// 환자의 전체 이름 (성 + 이름)
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// 환자의 나이 계산
        /// </summary>
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public Patient()
        {
            Appointments = new List<Appointment>();
            RegistrationDate = DateTime.Now;
        }
    }
}