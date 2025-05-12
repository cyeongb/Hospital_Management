using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;

namespace HospitalManagementSystem.ViewModels
{
    /// <summary>
    /// 예약 관리 화면의 ViewModel
    /// </summary>
    public class AppointmentViewModel : ViewModelBase
    {
        private readonly DataService _dataService;

        private ObservableCollection<Appointment> _appointments;
        private ObservableCollection<Patient> _patients;
        private ObservableCollection<Doctor> _doctors;
        private Appointment _selectedAppointment;
        private Appointment _editingAppointment;
        private DateTime _selectedDate;
        private bool _isAdding;
        private bool _isEditing;
        private string _filterText;

        /// <summary>
        /// 예약 목록
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        /// <summary>
        /// 환자 목록 (선택용)
        /// </summary>
        public ObservableCollection<Patient> Patients
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        /// <summary>
        /// 의사 목록 (선택용)
        /// </summary>
        public ObservableCollection<Doctor> Doctors
        {
            get => _doctors;
            set => SetProperty(ref _doctors, value);
        }

        /// <summary>
        /// 선택된 예약
        /// </summary>
        public Appointment SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                if (SetProperty(ref _selectedAppointment, value))
                {
                    // 속성이 변경되면 Command 실행 가능 상태 갱신
                    EditAppointmentCommand.RaiseCanExecuteChanged();
                    DeleteAppointmentCommand.RaiseCanExecuteChanged();

                    if (value != null)
                    {
                        // 선택된 예약 정보를 편집용 객체로 복사
                        EditingAppointment = new Appointment
                        {
                            AppointmentId = value.AppointmentId,
                            PatientId = value.PatientId,
                            DoctorId = value.DoctorId,
                            AppointmentDateTime = value.AppointmentDateTime,
                            EndDateTime = value.EndDateTime,
                            Purpose = value.Purpose,
                            Status = value.Status,
                            Notes = value.Notes
                        };
                    }
                }
            }
        }

        /// <summary>
        /// 편집 중인 예약
        /// </summary>
        public Appointment EditingAppointment
        {
            get => _editingAppointment;
            set => SetProperty(ref _editingAppointment, value);
        }

        /// <summary>
        /// 선택된 날짜 (날짜별 필터링용)
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    LoadAppointmentsByDate();
                }
            }
        }

        /// <summary>
        /// 추가 모드 여부
        /// </summary>
        public bool IsAdding
        {
            get => _isAdding;
            set => SetProperty(ref _isAdding, value);
        }

        /// <summary>
        /// 편집 모드 여부
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        /// <summary>
        /// 필터 텍스트
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value);
        }

        /// <summary>
        /// 예약 필터링 명령
        /// </summary>
        public RelayCommand FilterCommand { get; }

        /// <summary>
        /// 오늘 예약 보기 명령
        /// </summary>
        public RelayCommand ViewTodayCommand { get; }

        /// <summary>
        /// 추가 모드 시작 명령
        /// </summary>
        public RelayCommand AddNewCommand { get; }

        /// <summary>
        /// 예약 추가 명령
        /// </summary>
        public RelayCommand SaveNewCommand { get; }

        /// <summary>
        /// 추가 취소 명령
        /// </summary>
        public RelayCommand CancelAddCommand { get; }

        /// <summary>
        /// 편집 모드 시작 명령
        /// </summary>
        public RelayCommand EditAppointmentCommand { get; }

        /// <summary>
        /// 예약 정보 저장 명령
        /// </summary>
        public RelayCommand SaveEditCommand { get; }

        /// <summary>
        /// 편집 취소 명령
        /// </summary>
        public RelayCommand CancelEditCommand { get; }

        /// <summary>
        /// 예약 삭제 명령
        /// </summary>
        public RelayCommand DeleteAppointmentCommand { get; }

        /// <summary>
        /// 생성자
        /// </summary>
        public AppointmentViewModel()
        {
            _dataService = new DataService();

            // 오늘 날짜로 초기화
            SelectedDate = DateTime.Today;

            // 환자 및 의사 목록 로드
            LoadPatientsAndDoctors();

            // 선택된 날짜의 예약 로드
            LoadAppointmentsByDate();

            // 편집용 예약 객체 초기화
            EditingAppointment = new Appointment
            {
                AppointmentDateTime = DateTime.Now.Date.AddHours(9).AddMinutes(Math.Ceiling(DateTime.Now.Minute / 30.0) * 30), // 30분 단위로 반올림
                EndDateTime = DateTime.Now.Date.AddHours(9).AddMinutes(Math.Ceiling(DateTime.Now.Minute / 30.0) * 30).AddMinutes(30)
            };

            // 명령 초기화
            FilterCommand = new RelayCommand(ExecuteFilter);
            ViewTodayCommand = new RelayCommand(ExecuteViewToday);
            AddNewCommand = new RelayCommand(ExecuteAddNew);
            SaveNewCommand = new RelayCommand(ExecuteSaveNew);
            CancelAddCommand = new RelayCommand(ExecuteCancelAdd);
            EditAppointmentCommand = new RelayCommand(ExecuteEditAppointment, CanEditAppointment);
            SaveEditCommand = new RelayCommand(ExecuteSaveEdit);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);
            DeleteAppointmentCommand = new RelayCommand(ExecuteDeleteAppointment, CanDeleteAppointment);
        }

        /// <summary>
        /// 환자 및 의사 목록 로드
        /// </summary>
        private void LoadPatientsAndDoctors()
        {
            Patients = new ObservableCollection<Patient>(_dataService.GetAllPatients());
            Doctors = new ObservableCollection<Doctor>(_dataService.GetAllDoctors());
        }

        /// <summary>
        /// 선택된 날짜의 예약 로드
        /// </summary>
        private void LoadAppointmentsByDate()
        {
            var appointments = _dataService.GetAppointmentsByDate(SelectedDate);
            Appointments = new ObservableCollection<Appointment>(appointments);
        }

        /// <summary>
        /// 예약 필터링 실행
        /// </summary>
        private void ExecuteFilter(object parameter)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                LoadAppointmentsByDate();
                return;
            }

            try
            {
                var allAppointments = _dataService.GetAllAppointments();

                // 환자 이름, 의사 이름, 예약 목적으로 필터링
                var filteredAppointments = allAppointments.Where(a =>
                    (a.Patient != null && a.Patient.FullName.Contains(FilterText)) ||
                    (a.Doctor != null && a.Doctor.FullName.Contains(FilterText)) ||
                    (a.Purpose != null && a.Purpose.Contains(FilterText))
                ).ToList();

                Appointments = new ObservableCollection<Appointment>(filteredAppointments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예약 필터링 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 오늘 예약 보기
        /// </summary>
        private void ExecuteViewToday(object parameter)
        {
            SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// 신규 예약 추가 모드 시작
        /// </summary>
        private void ExecuteAddNew(object parameter)
        {
            IsAdding = true;

            // 기본값으로 설정
            EditingAppointment = new Appointment
            {
                AppointmentDateTime = DateTime.Now.Date.AddHours(9).AddMinutes(Math.Ceiling(DateTime.Now.Minute / 30.0) * 30),
                EndDateTime = DateTime.Now.Date.AddHours(9).AddMinutes(Math.Ceiling(DateTime.Now.Minute / 30.0) * 30).AddMinutes(30),
                Status = "예약됨"
            };
        }

        /// <summary>
        /// 신규 예약 저장
        /// </summary>
        private void ExecuteSaveNew(object parameter)
        {
            try
            {
                // 유효성 검사
                if (EditingAppointment.PatientId == 0 || EditingAppointment.DoctorId == 0)
                {
                    MessageBox.Show("환자와 의사를 선택해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingAppointment.Purpose))
                {
                    MessageBox.Show("예약 목적을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EditingAppointment.AppointmentDateTime >= EditingAppointment.EndDateTime)
                {
                    MessageBox.Show("종료 시간은 시작 시간보다 이후여야 합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _dataService.AddAppointment(EditingAppointment);

                // 리스트 새로고침
                LoadAppointmentsByDate();

                // 추가 모드 종료
                IsAdding = false;

                MessageBox.Show("예약이 성공적으로 추가되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예약 추가 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 추가 취소
        /// </summary>
        private void ExecuteCancelAdd(object parameter)
        {
            IsAdding = false;
        }

        /// <summary>
        /// 예약 편집 모드 시작
        /// </summary>
        private void ExecuteEditAppointment(object parameter)
        {
            if (SelectedAppointment != null)
            {
                IsEditing = true;
            }
        }

        /// <summary>
        /// 예약 편집 가능 여부
        /// </summary>
        private bool CanEditAppointment(object parameter)
        {
            return SelectedAppointment != null;
        }

        /// <summary>
        /// 예약 정보 저장
        /// </summary>
        private void ExecuteSaveEdit(object parameter)
        {
            try
            {
                // 유효성 검사
                if (EditingAppointment.PatientId == 0 || EditingAppointment.DoctorId == 0)
                {
                    MessageBox.Show("환자와 의사를 선택해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingAppointment.Purpose))
                {
                    MessageBox.Show("예약 목적을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EditingAppointment.AppointmentDateTime >= EditingAppointment.EndDateTime)
                {
                    MessageBox.Show("종료 시간은 시작 시간보다 이후여야 합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _dataService.UpdateAppointment(EditingAppointment);

                // 리스트 새로고침
                LoadAppointmentsByDate();

                // 편집 모드 종료
                IsEditing = false;

                MessageBox.Show("예약 정보가 성공적으로 수정되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예약 정보 수정 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 편집 취소
        /// </summary>
        private void ExecuteCancelEdit(object parameter)
        {
            IsEditing = false;
        }

        /// <summary>
        /// 예약 삭제
        /// </summary>
        private void ExecuteDeleteAppointment(object parameter)
        {
            if (SelectedAppointment != null)
            {
                var patientName = SelectedAppointment.Patient?.FullName ?? "환자";
                var appointmentDate = SelectedAppointment.AppointmentDateTime.ToString("yyyy년 MM월 dd일 HH:mm");

                var result = MessageBox.Show(
                    $"정말로 {patientName}의 {appointmentDate} 예약을 삭제하시겠습니까?",
                    "예약 삭제",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dataService.DeleteAppointment(SelectedAppointment.AppointmentId);

                        // 리스트 새로고침
                        LoadAppointmentsByDate();

                        MessageBox.Show("예약이 성공적으로 삭제되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"예약 삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 예약 삭제 가능 여부
        /// </summary>
        private bool CanDeleteAppointment(object parameter)
        {
            return SelectedAppointment != null;
        }
    }
}