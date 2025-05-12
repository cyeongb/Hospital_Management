using System;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// 예약 정보를 나타내는 모델 클래스
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// 예약 고유 식별자
        /// </summary>
        public int AppointmentId { get; set; }

        /// <summary>
        /// 환자 ID (외래키)
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// 환자 참조 객체
        /// </summary>
        public Patient Patient { get; set; }

        /// <summary>
        /// 의사 ID (외래키)
        /// </summary>
        public int DoctorId { get; set; }

        /// <summary>
        /// 의사 참조 객체
        /// </summary>
        public Doctor Doctor { get; set; }

        /// <summary>
        /// 예약 날짜 및 시간
        /// </summary>
        public DateTime AppointmentDateTime { get; set; }

        /// <summary>
        /// 예약 종료 예상 시간
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 예약 생성 시간
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 예약 이유/목적
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 예약 상태 (예약됨, 완료됨, 취소됨 등)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 메모/비고
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 예약 지속 시간 (분 단위)
        /// </summary>
        public int DurationMinutes
        {
            get
            {
                return (int)(EndDateTime - AppointmentDateTime).TotalMinutes;
            }
        }

        /// <summary>
        /// 예약이 현재 시각 기준으로 지났는지 여부
        /// </summary>
        public bool IsPast
        {
            get
            {
                return DateTime.Now > EndDateTime;
            }
        }

        /// <summary>
        /// 예약이 당일인지 여부
        /// </summary>
        public bool IsToday
        {
            get
            {
                return AppointmentDateTime.Date == DateTime.Today;
            }
        }

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public Appointment()
        {
            CreatedAt = DateTime.Now;
            Status = "예약됨";

            // 기본 30분 예약
            EndDateTime = AppointmentDateTime.AddMinutes(30);
        }

        /// <summary>
        /// 예약 정보의 문자열 표현
        /// </summary>
        public override string ToString()
        {
            return $"{AppointmentDateTime:yyyy-MM-dd HH:mm} - {Patient?.FullName} ({Doctor?.FullName})";
        }
    }
}