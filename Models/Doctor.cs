using System;
using System.Collections.Generic;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// 의사 정보를 나타내는 모델 클래스
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// 의사 고유 식별자
        /// </summary>
        public int DoctorId { get; set; }

        /// <summary>
        /// 의사 성
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 의사 이름
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 전문 분야 (예: 내과, 외과, 소아과 등)
        /// </summary>
        public string Specialization { get; set; }

        /// <summary>
        /// 면허 번호
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        /// 연락처
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 이메일 주소
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 고용일
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// 근무 시간 (텍스트 형식, 예: "월-금 9:00-18:00")
        /// </summary>
        public string WorkingHours { get; set; }

        /// <summary>
        /// 이 의사의 모든 예약 목록
        /// </summary>
        public List<Appointment> Appointments { get; set; }

        /// <summary>
        /// 의사의 전체 이름 (성 + 이름)
        /// </summary>
        public string FullName => $"Dr. {FirstName} {LastName}";

        /// <summary>
        /// 현재 날짜 기준 근속 기간 (연 단위)
        /// </summary>
        public int YearsOfService
        {
            get
            {
                var today = DateTime.Today;
                var years = today.Year - HireDate.Year;
                if (HireDate.Date > today.AddYears(-years)) years--;
                return years;
            }
        }

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public Doctor()
        {
            Appointments = new List<Appointment>();
            HireDate = DateTime.Now;
        }
    }
}