using System;
using System.Windows.Threading;
using HospitalManagementSystem.Services;

namespace HospitalManagementSystem.ViewModels
{
    /// <summary>
    /// 메인 화면의 ViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly DispatcherTimer _refreshTimer;

        private int _totalPatients;
        private int _totalDoctors;
        private int _todayAppointments;
        private int _pendingAppointments;

        /// <summary>
        /// 총 환자 수
        /// </summary>
        public int TotalPatients
        {
            get => _totalPatients;
            set => SetProperty(ref _totalPatients, value);
        }

        /// <summary>
        /// 총 의사 수
        /// </summary>
        public int TotalDoctors
        {
            get => _totalDoctors;
            set => SetProperty(ref _totalDoctors, value);
        }

        /// <summary>
        /// 오늘의 예약 수
        /// </summary>
        public int TodayAppointments
        {
            get => _todayAppointments;
            set => SetProperty(ref _todayAppointments, value);
        }

        /// <summary>
        /// 대기 중인 예약 수
        /// </summary>
        public int PendingAppointments
        {
            get => _pendingAppointments;
            set => SetProperty(ref _pendingAppointments, value);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public MainViewModel()
        {
            _dataService = new DataService();

            // 초기 데이터 로드
            RefreshDashboardData();

            // 주기적으로 데이터 갱신 (30초마다)
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _refreshTimer.Tick += (s, e) => RefreshDashboardData();
            _refreshTimer.Start();
        }

        /// <summary>
        /// 대시보드 데이터 갱신
        /// </summary>
        private void RefreshDashboardData()
        {
            // 환자 수 갱신
            TotalPatients = _dataService.GetAllPatients().Count;

            // 의사 수 갱신
            TotalDoctors = _dataService.GetAllDoctors().Count;

            // 오늘 예약 수 갱신
            TodayAppointments = _dataService.GetAppointmentsByDate(DateTime.Today).Count;

            // 대기 중인 예약 수 갱신 (오늘 이후의 예약)
            var allAppointments = _dataService.GetAllAppointments();
            PendingAppointments = allAppointments.Count(a =>
                a.AppointmentDateTime > DateTime.Now &&
                a.Status == "예약됨");
        }

        /// <summary>
        /// 리소스 정리
        /// </summary>
        public void Dispose()
        {
            _refreshTimer.Stop();
        }
    }
}